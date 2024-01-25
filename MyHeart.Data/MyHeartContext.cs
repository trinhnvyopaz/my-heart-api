using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyHeart.Data.Models;
using MyHeart.Data.Models.PdfTemplate;
using MyHeart.Data.ModelExtensions;
using System.Linq.Expressions;
using MyHeart.Data.Models.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyHeart.Data
{
    public class MyHeartContext : DbContext
    {
        public MyHeartContext(DbContextOptions<MyHeartContext> dbContextOptions)
            : base(dbContextOptions) { }

        public MyHeartContext()
            : base() { }

        public DbSet<User> Users { get; set; }

        //public DbSet<UserType> UserTypes { get; set; }
        public DbSet<PasswordResetTicket> PasswordResetTickets { get; set; }
        public DbSet<HospitalizationLength> HospitalizationLengths { get; set; }
        public DbSet<MedicalFacilities> MedicalFacilities { get; set; }
        public DbSet<MedicalFacilities_PreventiveMeasures_Preventive> MedicalFacilities_PreventiveMeasures_Preventive { get; set; }
        public DbSet<MedicamentGroup> MedicamentGroup { get; set; }
        public DbSet<MedicamentGroup_Pharmacy_ActiveSubstance> MedicamentGroup_Pharmacy_ActiveSubstance { get; set; }
        public DbSet<MedicamentGroup_Disease_Contraindication> MedicamentGroup_Disease_Contraindication { get; set; }
        public DbSet<MedicamentGroup_Disease_Indication> MedicamentGroup_Disease_Indication { get; set; }
        public DbSet<MedicamentGroup_Symptoms_SideEffects> MedicamentGroup_Symptoms_SideEffects { get; set; }
        public DbSet<MedicamentGroup_Pharmacy_Discard> MedicamentGroup_Pharmacy_Discard { get; set; }
        public DbSet<MedicamentGroup_Atc> MedicamentGroup_Atc { get; set; }
        public DbSet<NonpharmaticTherapy> NonpharmaticTherapy { get; set; }
        public DbSet<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention> NonpharmaticTherapy_MedicalFacilities_MedicalIntervention { get; set; }
        public DbSet<NonpharmaticTherapy_Disease_Contraindication> NonpharmaticTherapy_Disease_Contraindication { get; set; }
        public DbSet<NonpharmaticTherapy_Disease_Indication> NonpharmaticTherapy_Disease_Indication { get; set; }
        public DbSet<Pharmacy> Pharmacy { get; set; }
        public DbSet<Pharmacy_CommercialNames> Pharmacy_CommercialNames { get; set; }
        public DbSet<Pharmacy_Disease_ContraIndication> Pharmacy_Disease_ContraIndication { get; set; }
        public DbSet<Pharmacy_Disease_Indication> Pharmacy_Disease_Indication { get; set; }
        public DbSet<Pharmacy_Symptoms_SideEffects> Pharmacy_Symptoms_SideEffects { get; set; }
        public DbSet<Pil> Pils { get; set; }
        public DbSet<PreventiveMeasures> PreventiveMeasures { get; set; }
        public DbSet<Disease> Disease { get; set; }
        public DbSet<Disease_Disease_Causes> Disease_Disease_Causes { get; set; }
        public DbSet<Disease_Symptoms_Symptoms> Disease_Symptoms_Symptoms { get; set; }
        public DbSet<Symptoms> Symptoms { get; set; }
        public DbSet<TherapyNews> TherapyNews { get; set; }
        public DbSet<TherapyNews_Disease_Sick> TherapyNews_Disease_Sick { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionComment> QuestionComments { get; set; }
        public DbSet<VideoRoom> VideoRooms { get; set; }
        public DbSet<UserDetail> UserDetail { get; set; }

        public DbSet<UserPersonalDisease> UserPersonalDisease { get; set; }
        public DbSet<UserPersonalNonpharmaceutical> UserPersonalNonpharmaceutical { get; set; }
        public DbSet<UserPharmacy> UserPharmacy { get; set; }

        public DbSet<UserAnamnesis> UserAnamnesis { get; set; }
        public DbSet<DoctorDetail> DoctorDetails { get; set; }
        public DbSet<Disease_NonpharmaticTherapy_NonpharmaticTherapy> Disease_NonpharmaticTherapy_NonpharmaticTherapy { get; set; }
        public DbSet<Disease_PreventiveMeasures_PreventiveMeasures> Disease_PreventiveMeasures_PreventiveMeasures { get; set; }
        public DbSet<SymptomQuestions> SymptomQuestions { get; set; }
        public DbSet<SymptomQuestions_Symptom> SymptomQuestions_Symptom { get; set; }
        public DbSet<SymptomQuestions_Disease> SymptomQuestions_Disease { get; set; }
        public DbSet<CommercialName> CommercialName { get; set; }
        public DbSet<CommercialName_Pharmacy> CommercialName_Pharmacy { get; set; }

        public DbSet<Client_Disease> Client_Disease { get; set; }
        public DbSet<Client_Document> Client_Document { get; set; }
        public DbSet<Client_QuestionAnswer> Client_QuestionAnswers { get; set; }
        public DbSet<Client_Therapy> Client_Therapy { get; set; }
        public DbSet<EmailTemplate> EmailTemplate { get; set; }

        public DbSet<Atc> Atc { get; set; }
        public DbSet<UserReport> UserReport { get; set; }
        public DbSet<UserReportFile> UserReportFile { get; set; }
        public DbSet<UserNonpharmacy> UserNonpharmacy { get; set; }
        public DbSet<UserDisease> UserDiseases { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<UserTrustedLogin> TrustedLogins { get; set; }

        public DbSet<UserPhoneAuthRequest> PhoneAuth { get; set; }
        public DbSet<Client_Questionaire> Client_Questionaire { get; set; }
        public DbSet<PdfTemplate> PdfTemplate { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<PaymentLog> PaymentLog { get; set; }
        public DbSet<NotificationQueue> NotificationQueue { get; set; }
        public DbSet<NotificationQueueLog> NotificationQueueLog { get; set; }
        public DbSet<UserFmcToken> UserFmcTokens { get; set; }
        public DbSet<Disease_Synonyms> DiseaseSynonyms { get; set; }

        // Medical report
        public DbSet<OCRResult> OCRResults { get; set; }
        public DbSet<ProcessedTextContent> ProcessedTextContents { get; set; }
        public DbSet<PatientMedicalRecord> PatientMedicalRecords { get; set; }
        public DbSet<PatientMedicalExamination> PatientMedicalExaminations { get; set; }
        public DbSet<Parameter> Parameter { get; set; }
        public DbSet<Marker> Marker { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Disease_Symptoms_Symptoms>()
                .HasKey(sc => new { sc.DiseaseId, sc.SymptomsId });
            modelBuilder
                .Entity<Disease_Disease_Causes>()
                .HasKey(sc => new { sc.DiseaseId, sc.CauseId });
            modelBuilder
                .Entity<Disease_NonpharmaticTherapy_NonpharmaticTherapy>()
                .HasKey(sc => new { sc.DiseaseId, sc.NonpharmaticTherapyId });
            modelBuilder
                .Entity<Disease_PreventiveMeasures_PreventiveMeasures>()
                .HasKey(sc => new { sc.DiseaseId, sc.PreventiveMeasuresId });

            modelBuilder
                .Entity<MedicalFacilities_PreventiveMeasures_Preventive>()
                .HasKey(sc => new { sc.MedicalFacilitiesId, sc.PreventiveMeasureId });

            modelBuilder
                .Entity<MedicamentGroup_Disease_Contraindication>()
                .HasKey(sc => new { sc.MedicamentGroupId, sc.DiseaseId });
            modelBuilder
                .Entity<MedicamentGroup_Disease_Indication>()
                .HasKey(sc => new { sc.MedicamentGroupId, sc.DiseaseId });
            modelBuilder
                .Entity<MedicamentGroup_Pharmacy_ActiveSubstance>()
                .HasKey(sc => new { sc.MedicamentGroupId, sc.PharmacyId });
            modelBuilder
                .Entity<MedicamentGroup_Symptoms_SideEffects>()
                .HasKey(sc => new { sc.MedicamentGroupId, sc.SymptomsId });
            modelBuilder
                .Entity<MedicamentGroup_Pharmacy_Discard>()
                .HasKey(sc => new { sc.MedicamentGroupId, sc.PharmacyId });
            modelBuilder
                .Entity<MedicamentGroup_Atc>()
                .HasKey(sc => new { sc.MedicamentGroupId, sc.AtcId });

            modelBuilder
                .Entity<NonpharmaticTherapy_Disease_Contraindication>()
                .HasKey(sc => new { sc.NonpharmaticTherapyId, sc.DiseaseId });
            modelBuilder
                .Entity<NonpharmaticTherapy_Disease_Indication>()
                .HasKey(sc => new { sc.NonpharmaticTherapyId, sc.DiseaseId });
            modelBuilder
                .Entity<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention>()
                .HasKey(sc => new { sc.NonpharmaticTherapyId, sc.MedicalFacilitiesId });

            modelBuilder.Entity<Pharmacy_CommercialNames>().HasKey(sc => new { sc.PharmacyId });
            modelBuilder
                .Entity<Pharmacy_Disease_ContraIndication>()
                .HasKey(sc => new { sc.PharmacyId, sc.DiseaseId });
            modelBuilder
                .Entity<Pharmacy_Disease_Indication>()
                .HasKey(sc => new { sc.PharmacyId, sc.DiseaseId });
            modelBuilder
                .Entity<Pharmacy_Symptoms_SideEffects>()
                .HasKey(sc => new { sc.PharmacyId, sc.SymptomId });

            modelBuilder
                .Entity<TherapyNews_Disease_Sick>()
                .HasKey(sc => new { sc.TherapyNewsId, sc.DiseaseId });

            modelBuilder
                .Entity<SymptomQuestions_Symptom>()
                .HasKey(sc => new { sc.SymptomsId, sc.SymptomQuestionsId });

            modelBuilder
                .Entity<SymptomQuestions_Disease>()
                .HasKey(sc => new { sc.DiseaseId, sc.SymptomsQuestionsId });

            modelBuilder
                .Entity<SymptomQuestions>()
                .HasMany(x => x.Diseases)
                .WithOne(y => y.DiseaseQuestions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<CommercialName_Pharmacy>()
                .HasKey(sc => new { sc.PharmacyId, sc.CommercialNamesId });

            modelBuilder.Entity<Client_Disease>().HasKey(sc => new { sc.DiseaseId, sc.ClientId });
            modelBuilder.Entity<Client_Document>().HasKey(sc => new { sc.ClientId });
            modelBuilder.Entity<Client_QuestionAnswer>().HasKey(sc => new { sc.Id });
            modelBuilder.Entity<Client_Therapy>().HasKey(sc => new { sc.PharmacyId, sc.ClientId });

            //modelBuilder.Entity<UserType>().HasData(new UserType { Id = 1, Name = "Client" });
            modelBuilder
                .Entity<User>()
                .HasData(
                    new User
                    {
                        Email = "info@memos.cz",
                        FirstName = "Memos",
                        Id = 1,
                        LastName = "Memos",
                        UserType = DTO.User.UType.SuperAdmin,
                        PasswordHash = "Qz0BICLJeokFZ5xZVcz4ZxtvFs/xvCYxp/+2yZKSBjfKVER7"
                    }
                );
            modelBuilder.Entity<User>().HasMany(u => u.TrustedLogins).WithOne(l => l.User);

            modelBuilder.Entity<UserPersonalDisease>().HasKey(upd => new { upd.UserId, upd.Name });
            modelBuilder
                .Entity<UserPersonalNonpharmaceutical>()
                .HasKey(upn => new { upn.UserId, upn.Name });

            modelBuilder.Entity<UserPharmacy>().HasKey(up => new { up.UserId, up.Name });

            modelBuilder.Entity<UserReport>().HasKey(ur => ur.Id);
            modelBuilder
                .Entity<UserReport>()
                .HasMany(ur => ur.UserReportFiles)
                .WithOne(ur => ur.UserReport);
            modelBuilder.Entity<UserReport>().HasOne(ur => ur.User).WithMany(u => u.UserReports);

            modelBuilder.Entity<UserReportFile>().HasKey(urf => urf.Id);

            modelBuilder.Entity<VideoRoom>().HasKey(v => v.Id);

            modelBuilder.Entity<Atc>(x =>
            {
                x.HasKey(y => y.Id);
                x.Property(y => y.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<MedicalFacilities>(x =>
            {
                x.HasKey(y => y.Id);
                x.Property(y => y.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Pharmacy>(x =>
            {
                x.HasKey(y => y.Id);
                x.Property(y => y.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Pil>(x =>
            {
                x.HasKey(y => y.Id);
                x.Property(y => y.Id).ValueGeneratedOnAdd();
            });

            foreach (
                var entityType in modelBuilder.Model
                    .GetEntityTypes()
                    .Where(e => e.ClrType.IsSubclassOf(typeof(BaseModel)))
            )
            {
                modelBuilder.Entity(
                    entityType.Name,
                    x =>
                    {
                        x.Property("LastUpdateUser").HasDefaultValue("UNDEFINED");
                    }
                );
            }

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder
                        .Entity(entityType.ClrType)
                        .HasQueryFilter(GetIsNotDeletedExpression(entityType.ClrType));
                }
            }

            modelBuilder
                .Entity<Subscription>()
                .HasData(new Subscription { Id = 1 }, new Subscription { Id = 2 });
            modelBuilder.Entity<UserReportFile>().HasOne(e => e.UserReport);
        }

        private static LambdaExpression GetIsNotDeletedExpression(Type type)
        {
            var param = Expression.Parameter(type, "t");
            var body = Expression.Equal(
                Expression.PropertyOrField(param, "IsDeleted"),
                Expression.Constant(false)
            );
            return Expression.Lambda(body, param);
        }

        public void UpdateCollection<TEntity, Tkey>(
            ICollection<TEntity> dbCollection,
            IEnumerable<TEntity> newValues,
            Func<TEntity, Tkey> keySelector
        )
        {
            var leftJoined =
                from db in dbCollection
                join nv in newValues
                    on new { Tkey = keySelector(db) } equals new { Tkey = keySelector(nv) }
                    into j
                from dto in j.DefaultIfEmpty()
                select (db, dto);

            foreach (var (db, dto) in leftJoined.ToList())
            {
                if (dto != null)
                {
                    foreach (
                        var prop in this.Entry(db).Properties.Where(p => !p.Metadata.IsPrimaryKey())
                    )
                    {
                        prop.CurrentValue = prop.Metadata.PropertyInfo.GetValue(dto);
                    }
                }
            }

            var rightJoined =
                from dto in newValues
                join db in dbCollection on keySelector(dto) equals keySelector(db) into j
                from db in j.DefaultIfEmpty()
                select (db, dto);

            foreach (var (db, dto) in rightJoined.Where(p => p.db == null).ToList())
            {
                dbCollection.Add(dto);
            }
        }

        public override int SaveChanges()
        {
            var entities = ChangeTracker
                .Entries()
                .Where(
                    e =>
                        e.Entity is BaseModel
                        && (e.State == EntityState.Modified || e.State == EntityState.Added)
                );

            foreach (var entity in entities)
            {
                var be = ((BaseModel)entity.Entity).LastUpdateDate = DateTime.Now;
            }

            ProcessSoftDelete();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default
        )
        {
            ProcessSoftDelete();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            var entities = ChangeTracker
                .Entries()
                .Where(
                    e =>
                        e.Entity is BaseModel
                        && (e.State == EntityState.Modified || e.State == EntityState.Added)
                );

            foreach (var entity in entities)
            {
                var be = ((BaseModel)entity.Entity).LastUpdateDate = DateTime.Now;
                if (entity.Entity is UserAnamnesis anamnesis && entity.State == EntityState.Added)
                {
                    anamnesis.CreatedAt = DateTime.Now;
                }
            }

            ProcessSoftDelete();

            return base.SaveChangesAsync(true, cancellationToken);
        }

        protected void ProcessSoftDelete()
        {
            // 1. for each newly deleted entity
            foreach (var item in ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted))
            {
                // 2. if it is Soft Deletable
                if (item.Entity is ISoftDeletable entity)
                {
                    // 3. revert deletion
                    item.State = EntityState.Unchanged;
                    // 4. set IsDeleted flag instead
                    entity.IsDeleted = true;
                }
            }
        }

        public async Task SetIdentityInsertAsync<TEnt>(bool enable)
        {
            var entityType = Model.FindEntityType(typeof(TEnt));
            var value = enable ? "ON" : "OFF";
            string query = $"SET IDENTITY_INSERT dbo.{entityType.GetTableName()} {value}";
            await Database.ExecuteSqlRawAsync(query);
        }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();
        }*/
        public void MigrateDataUserToRole(DbContextOptions<MyHeartContext> dbContextOptions)
        {
            using (var context = new MyHeartContext(dbContextOptions))
            {
                var countRole = context.Roles.Count();

                if (countRole == 0)
                {
                    var listUser = context.Users.ToList();
                    var listRole = new List<Role>();

                    foreach (var user in listUser)
                    {
                        listRole.Add(new Role { UserId = user.Id, RoleType = user.UserType });

                        if (user.UserType == DTO.User.UType.Doctor)
                        {
                            listRole.Add(
                                new Role { UserId = user.Id, RoleType = DTO.User.UType.DataManager }
                            );
                        }
                    }
                    context.Roles.AddRange(listRole);
                    context.SaveChanges();
                }
            }
        }
    }
}
