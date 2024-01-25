using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyHeart.Data;
using MyHeart.Services;
using MyHeart.Services.Interfaces;
using MyHeart.Services.Mapping;
using AutoMapper;
using System;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Messaging;

using Newtonsoft.Json;

[assembly: FunctionsStartup(typeof(MyHeart.Functions.Startup))]
namespace MyHeart.Functions
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            builder.Services.AddDbContext<MyHeartContext>(options =>
            options.UseSqlServer(connectionString));

            builder.Services.AddAutoMapper(typeof(ModelToViewModelProfile));
            builder.Services.AddScoped<IAnamnesisService, AnamnesisService>();
            builder.Services.AddScoped<INotificationQueueService, NotificationQueueService>();

            builder.Services.AddSingleton(sp => FirebaseMessaging.DefaultInstance);

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromJson(GetFirebaseConfigAsJson())
            });
        }

        private string GetFirebaseConfigAsJson()
        {
            var config = Environment.GetEnvironmentVariables();
            var firebaseConfig = new
            {
                type = config["FireBaseType"],
                project_id = config["FirebaseProjectId"],
                private_key_id = config["FirebasePrivateKeyId"],
                private_key = config["FirebasePrivateKey"].ToString().Replace("\\n", "\n"),
                client_email = config["FirebaseClientEmail"],
                client_id = config["FirebaseClientId"],
                auth_uri = config["FirebaseAuthUri"],
                token_uri = config["FirebaseTokenUri"],
                auth_provider_x509_cert_url = config["FirebaseAuthProviderCertUrl"],
                client_x509_cert_url = config["FirebaseClientCertUrl"]
            };

            return JsonConvert.SerializeObject(firebaseConfig);
        }
    }
}
