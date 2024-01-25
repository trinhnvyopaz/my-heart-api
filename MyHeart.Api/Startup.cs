using AspNetCoreRateLimit;
using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyHeart.Api.Hubs;
using MyHeart.Api.Util;
using MyHeart.Core.Helpers;
using MyHeart.Data;
using MyHeart.DTO.User;
using MyHeart.Services;
using MyHeart.Services.Azure;
using MyHeart.Services.Interfaces;
using MyHeart.Services.Interfaces.Azure;
using MyHeart.Services.Interfaces.Client;
using MyHeart.Services.Interfaces.ProfessionInfo;
using MyHeart.Services.Mapping;
using MyHeart.Services.OpenData;
using MyHeart.Services.ProfessionInfoServices;
using Stripe;
using Stripe.Checkout;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SubscriptionService = MyHeart.Services.SubscriptionService;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Azure;
using Azure.Storage.Queues;
using Azure.Storage.Blobs;
using Azure.Core.Extensions;

namespace MyHeart.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });
            // services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "*",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    }
                );
            });
            // services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                //var secRequirement = new OpenApiSecurityRequirement();
                var secScheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description =
                        "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                };
                //secRequirement.Add(secScheme, new List<string>());

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyHeart API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", secScheme);
                //c.AddSecurityRequirement(secRequirement);
                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] { }
                        }
                    }
                );
            });

            var securitySettingsSection = Configuration.GetSection("Security");
            services.Configure<SecuritySettings>(securitySettingsSection);

            var securitySettings = securitySettingsSection.Get<SecuritySettings>();
            var key = Encoding.UTF8.GetBytes(securitySettings.TokenSecret);

            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    // SignalR bearer token
                    x.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            var path = context.HttpContext.Request.Path;

                            if (
                                path.StartsWithSegments(
                                    "/hub",
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                            )
                            {
                                // Read the token out of the query string
                                var accessToken = context.Request.Query["access_token"];

                                if (!string.IsNullOrEmpty(accessToken))
                                {
                                    context.Token = accessToken;
                                }
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            //services.AddAuthorization();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    Policies.MinAdmin,
                    policy =>
                        policy.RequireClaim(
                            ClaimTypes.Role,
                            UType.SuperAdmin.ToString(),
                            UType.Admin.ToString()
                        )
                );
                options.AddPolicy(
                    Policies.MinDoctor,
                    policy =>
                        policy.RequireClaim(
                            ClaimTypes.Role,
                            UType.SuperAdmin.ToString(),
                            UType.Admin.ToString(),
                            UType.Doctor.ToString()
                        )
                );
                options.AddPolicy(
                    Policies.MinPatient,
                    policy =>
                        policy.RequireClaim(
                            ClaimTypes.Role,
                            UType.SuperAdmin.ToString(),
                            UType.Admin.ToString(),
                            UType.Doctor.ToString(),
                            UType.Patient.ToString()
                        )
                );
                options.AddPolicy(
                    Policies.MinDataManager,
                    policy =>
                        policy.RequireClaim(
                            ClaimTypes.Role,
                            UType.SuperAdmin.ToString(),
                            UType.Admin.ToString(),
                            UType.Doctor.ToString(),
                            UType.DataManager.ToString()
                        )
                );
            });

            services
                .AddControllers()
                .AddNewtonsoftJson(
                    options =>
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
                            .Json
                            .ReferenceLoopHandling
                            .Ignore
                );

            string conString = Configuration.GetConnectionString("MyHeartContext");
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddHostedService<ScanUserReportFileHostedService>();

            services.AddAutoMapper(typeof(ModelToViewModelProfile));
            services.AddDbContext<MyHeartContext>(opt => opt.UseSqlServer(conString));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IUserTypeService, UserTypeService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IImportService, ImportService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOCRWebService, OCRWebService>();

            // professionInfo
            services.AddScoped<IDiseaseService, DiseaseService>();
            services.AddScoped<ISymptomService, SymptomService>();
            services.AddScoped<IMedicamentGroupService, MedicamentGroupService>();
            services.AddScoped<IPharmacyService, PharmacyService>();
            services.AddScoped<INonpharmacyService, NonpharmacyService>();
            services.AddScoped<IMedicalFacilitieService, MedicalFacilitieService>();
            services.AddScoped<IPreventiveMeasureService, PreventiveMeasureService>();
            services.AddScoped<ITherapyNewsService, TherapyNewsService>();
            services.AddScoped<ISymptomQuestionsService, SymptomQuestionsService>();
            services.AddScoped<ICommercialNameService, CommercialNameService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IAtcService, AtcService>();
            services.AddScoped<IPilservice, PilService>();
            services.AddScoped<IOpenData, OpenData>();
            services.AddScoped<IAnamnesisService, AnamnesisService>();
            services.AddScoped<IUserNonpharmacyService, UserNonpharmacyService>();
            services.AddScoped<IUserReportsService, UserReportsService>();
            services.AddScoped<IBlob, Blob>();
            services.AddScoped<IMFAService, MFAService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISMSService, SMSService>();
            services.AddScoped<IQuestionaireService, QuestionaireService>();
            services.AddScoped<IPdfReportService, PdfReportService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IPaymentGatewayProvider, StripeService>();
            services.AddScoped<CustomerService>();
            services.AddScoped<SessionService>();
            services.AddScoped<ProductService>();
            services.AddScoped<InvoiceService>();
            services.AddScoped<ChargeService>();
            services.AddScoped<Stripe.SubscriptionService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<INotificationQueueService, NotificationQueueService>();
            services.AddScoped<IVideoProviderService, TwilioService>();
            services.AddScoped<IPatientMedicalRecordService, PatientMedicalRecordService>();
            services.AddScoped<IParameterService, ParameterService>();

            services.AddSingleton<IEmailConfiguration>(
                Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>()
            );
            services.AddSingleton<IOcrWebServiceConfiguration>(
                Configuration
                    .GetSection("OcrWebServiceConfiguration")
                    .Get<OcrWebServiceConfiguration>()
            );
            services.AddSingleton<IOpenDataConfiguration>(
                Configuration.GetSection("OpenData").Get<OpenDataConfiguration>()
            );
            services.AddSingleton<IBlobConfiguration>(
                Configuration.GetSection("Blob").Get<BlobConfiguration>()
            );
            services.AddSingleton<ISanitizer>(new Sanitizer());
            services.AddSingleton<ISMSConfiguration>(
                Configuration.GetSection("SMSConfiguration").Get<SMSConfiguration>()
            );

            services.AddSingleton<IStripeConfiguration>(Configuration.GetSection("Stripe").Get<Services.StripeConfiguration>());
            services.AddSingleton<ITwilioConfiguration>(Configuration.GetSection("Twilio").Get<Services.TwilioConfiguration>());
            services.AddSingleton<IStripeConfiguration>(
                Configuration.GetSection("Stripe").Get<Services.StripeConfiguration>()
            );

            var firebaseCOnfig = Configuration
                .GetSection("Firebase")
                .Get<Services.FirebaseConfiguration>();
            services.AddSingleton<IFirebaseConfiguration>(firebaseCOnfig);

            services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));

            services.AddSingleton(sp => FirebaseMessaging.DefaultInstance);

            FirebaseApp.Create(
                new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(GetFirebaseConfigAsJson(firebaseCOnfig))
                }
            );

            services.AddHangfire(config =>
            {
                config.UseMemoryStorage(
                    new MemoryStorageOptions { FetchNextJobTimeout = TimeSpan.FromHours(24) }
                );
            });
            services.AddHangfireServer();

            //signalR
            services.AddSignalR(opts =>
            {
#if DEBUG
                opts.EnableDetailedErrors = true;
#endif
            });

            //rate limiting
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(
                Configuration.GetSection("IpRateLimitPolicies")
            );
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        private string GetFirebaseConfigAsJson(IFirebaseConfiguration config)
        {
            var firebaseConfig = new
            {
                type = config.FireBaseType,
                project_id = config.FirebaseProjectId,
                private_key_id = config.FirebasePrivateKeyId,
                private_key = config.FirebasePrivateKey.Replace("\\n", "\n"),
                client_email = config.FirebaseClientEmail,
                client_id = config.FirebaseClientId,
                auth_uri = config.FirebaseAuthUri,
                token_uri = config.FirebaseTokenUri,
                auth_provider_x509_cert_url = config.FirebaseAuthProviderCertUrl,
                client_x509_cert_url = config.FirebaseClientCertUrl,
            };

            return JsonConvert.SerializeObject(firebaseConfig);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IOpenDataConfiguration configuration,
            IRecurringJobManager recurringJobs,
            IBackgroundJobClient background,
            IOpenData openData,
            IStripeConfiguration stripeConfig
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseForwardedHeaders();
            app.UseHttpsRedirection();
            app.UseRouting();

            // app.UseCors(x => {
            //     var apiCors = Configuration["Cors:ApiAllowedOrigins"];
            //     var allowedOrigins = apiCors.Split(';', StringSplitOptions.RemoveEmptyEntries);
            //     x.WithOrigins(allowedOrigins);
            //     x.AllowAnyMethod();
            //     x.AllowAnyHeader();
            //     x.AllowCredentials();
            // });

            app.UseCors("*");

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseIpRateLimiting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHub<ChatHub>("/hub/chat");
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyHeart API V1");
            });

            using var scope = app.ApplicationServices.CreateScope();
            var provider = scope.ServiceProvider;
            var context = provider.GetRequiredService<MyHeartContext>();
            context.Database.SetCommandTimeout(TimeSpan.FromMinutes(30));
            // context.Database.Migrate();
            var options = new DbContextOptionsBuilder<MyHeartContext>()
                .UseSqlServer(Configuration.GetConnectionString("MyHeartContext"))
                .Options;
            context.MigrateDataUserToRole(options);

            recurringJobs.AddOrUpdate("openData", () => openData.UpdateData(), Cron.Monthly(1, 23));

            if (configuration.FirstFill && !System.IO.File.Exists(".openDataInit"))
            {
                background.Enqueue(() => openData.UpdateData());
                System.IO.File.Create(".openDataInit");
            }

            Stripe.StripeConfiguration.ApiKey = stripeConfig.ApiSecret;
        }
    }
}
