using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class freshstart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Atc",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    AtcCode = table.Column<string>(nullable: true),
                    NT = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NameEn = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommercialName",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialName", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Disease",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TargetWeight = table.Column<double>(nullable: false),
                    TargetSystolicPressure = table.Column<int>(nullable: false),
                    TargetDiastolicPressure = table.Column<int>(nullable: false),
                    TargetHeartRate = table.Column<int>(nullable: false),
                    TargetLdl = table.Column<double>(nullable: false),
                    SystemicMeasures = table.Column<string>(nullable: true),
                    Frequency = table.Column<decimal>(type: "decimal(18, 6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disease", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Subject = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HospitalizationLengths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    length = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HospitalizationLengths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalFacilities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    FacilityId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    FacilityCode = table.Column<string>(nullable: true),
                    FacilityTypeCode = table.Column<int>(nullable: false),
                    Character = table.Column<string>(nullable: false),
                    CharacterSecondary = table.Column<string>(nullable: true),
                    ICO = table.Column<int>(nullable: false),
                    PCZ = table.Column<int>(nullable: false),
                    PCDP = table.Column<int>(nullable: false),
                    Region = table.Column<string>(nullable: true),
                    RegionCode = table.Column<string>(nullable: true),
                    District = table.Column<string>(nullable: true),
                    DistrictCode = table.Column<string>(nullable: true),
                    AdministrativeDistrict = table.Column<string>(nullable: true),
                    Municipality = table.Column<string>(nullable: true),
                    ZIP = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    BuildingNumber = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Telephone = table.Column<string>(nullable: true),
                    Fax = table.Column<string>(nullable: true),
                    Website = table.Column<string>(nullable: true),
                    ProviderType = table.Column<string>(nullable: true),
                    ProviderName = table.Column<string>(nullable: true),
                    CareField = table.Column<string>(nullable: true),
                    CareForm = table.Column<string>(nullable: true),
                    CareType = table.Column<string>(nullable: true),
                    Representative = table.Column<string>(nullable: true),
                    GPS = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalFacilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicamentGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicamentGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonpharmaticTherapy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Efficiency = table.Column<decimal>(type: "decimal(18, 6)", nullable: false),
                    HospitalizationLength = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonpharmaticTherapy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTickets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Email = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    TokenUsed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Dosage = table.Column<string>(nullable: false),
                    MinimumDose = table.Column<string>(nullable: false),
                    MaximumDose = table.Column<string>(nullable: false),
                    SuklCode = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Strength = table.Column<string>(nullable: true),
                    Form = table.Column<string>(nullable: true),
                    Package = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    Supplement = table.Column<string>(nullable: true),
                    AtcWho = table.Column<string>(nullable: true),
                    DddamntWho = table.Column<double>(nullable: false),
                    DddunWho = table.Column<string>(nullable: true),
                    NameReg = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhoneAuth",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    LoginSecret = table.Column<Guid>(nullable: false),
                    PhoneAuthorized = table.Column<bool>(nullable: false),
                    AuthorisedUserId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneAuth", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pils",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    SuklCode = table.Column<int>(nullable: false),
                    OnWeb = table.Column<bool>(nullable: false),
                    FilePath = table.Column<string>(nullable: true),
                    DataUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pils", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreventiveMeasures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreventiveMeasures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SymptomQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SymptomQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Symptoms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Symptoms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TherapyNews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Text = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    WebLink = table.Column<string>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TherapyNews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPharmacy",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Dosing = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPharmacy", x => new { x.UserId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: false),
                    MFASecret = table.Column<string>(nullable: true),
                    MFATimeSlice = table.Column<long>(nullable: false),
                    MFARecovery = table.Column<string>(nullable: true),
                    SMSMFA = table.Column<string>(nullable: true),
                    UserType = table.Column<int>(nullable: false),
                    EmailComfirmed = table.Column<bool>(nullable: false),
                    MFAConfirmed = table.Column<bool>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    DeleteDate = table.Column<DateTime>(nullable: true),
                    SubscriptionPreferences = table.Column<int>(nullable: false),
                    TherapyNewsEmailNotification = table.Column<bool>(nullable: false),
                    ReplyEmailNotification = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoRooms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    QuestionID = table.Column<int>(nullable: false),
                    RoomId = table.Column<Guid>(nullable: false),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoRooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Disease_Disease_Causes",
                columns: table => new
                {
                    DiseaseId = table.Column<int>(nullable: false),
                    CauseId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disease_Disease_Causes", x => new { x.DiseaseId, x.CauseId });
                    table.ForeignKey(
                        name: "FK_Disease_Disease_Causes_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DiseaseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Disease_MedicamentGroup_MedicamentGroup",
                columns: table => new
                {
                    DiseaseId = table.Column<int>(nullable: false),
                    MedicamentGroupId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disease_MedicamentGroup_MedicamentGroup", x => new { x.DiseaseId, x.MedicamentGroupId });
                    table.ForeignKey(
                        name: "FK_Disease_MedicamentGroup_MedicamentGroup_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Disease_MedicamentGroup_MedicamentGroup_MedicamentGroup_MedicamentGroupId",
                        column: x => x.MedicamentGroupId,
                        principalTable: "MedicamentGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicamentGroup_Atc",
                columns: table => new
                {
                    MedicamentGroupId = table.Column<int>(nullable: false),
                    AtcId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicamentGroup_Atc", x => new { x.MedicamentGroupId, x.AtcId });
                    table.ForeignKey(
                        name: "FK_MedicamentGroup_Atc_Atc_AtcId",
                        column: x => x.AtcId,
                        principalTable: "Atc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicamentGroup_Atc_MedicamentGroup_MedicamentGroupId",
                        column: x => x.MedicamentGroupId,
                        principalTable: "MedicamentGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicamentGroup_Disease_Contraindication",
                columns: table => new
                {
                    MedicamentGroupId = table.Column<int>(nullable: false),
                    DiseaseId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicamentGroup_Disease_Contraindication", x => new { x.MedicamentGroupId, x.DiseaseId });
                    table.ForeignKey(
                        name: "FK_MedicamentGroup_Disease_Contraindication_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicamentGroup_Disease_Contraindication_MedicamentGroup_MedicamentGroupId",
                        column: x => x.MedicamentGroupId,
                        principalTable: "MedicamentGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicamentGroup_Disease_Indication",
                columns: table => new
                {
                    MedicamentGroupId = table.Column<int>(nullable: false),
                    DiseaseId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicamentGroup_Disease_Indication", x => new { x.MedicamentGroupId, x.DiseaseId });
                    table.ForeignKey(
                        name: "FK_MedicamentGroup_Disease_Indication_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicamentGroup_Disease_Indication_MedicamentGroup_MedicamentGroupId",
                        column: x => x.MedicamentGroupId,
                        principalTable: "MedicamentGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Disease_NonpharmaticTherapy_NonpharmaticTherapy",
                columns: table => new
                {
                    DiseaseId = table.Column<int>(nullable: false),
                    NonpharmaticTherapyId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disease_NonpharmaticTherapy_NonpharmaticTherapy", x => new { x.DiseaseId, x.NonpharmaticTherapyId });
                    table.ForeignKey(
                        name: "FK_Disease_NonpharmaticTherapy_NonpharmaticTherapy_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Disease_NonpharmaticTherapy_NonpharmaticTherapy_NonpharmaticTherapy_NonpharmaticTherapyId",
                        column: x => x.NonpharmaticTherapyId,
                        principalTable: "NonpharmaticTherapy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NonpharmaticTherapy_Disease_Contraindication",
                columns: table => new
                {
                    NonpharmaticTherapyId = table.Column<int>(nullable: false),
                    DiseaseId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonpharmaticTherapy_Disease_Contraindication", x => new { x.NonpharmaticTherapyId, x.DiseaseId });
                    table.ForeignKey(
                        name: "FK_NonpharmaticTherapy_Disease_Contraindication_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NonpharmaticTherapy_Disease_Contraindication_NonpharmaticTherapy_NonpharmaticTherapyId",
                        column: x => x.NonpharmaticTherapyId,
                        principalTable: "NonpharmaticTherapy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NonpharmaticTherapy_Disease_Indication",
                columns: table => new
                {
                    NonpharmaticTherapyId = table.Column<int>(nullable: false),
                    DiseaseId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonpharmaticTherapy_Disease_Indication", x => new { x.NonpharmaticTherapyId, x.DiseaseId });
                    table.ForeignKey(
                        name: "FK_NonpharmaticTherapy_Disease_Indication_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NonpharmaticTherapy_Disease_Indication_NonpharmaticTherapy_NonpharmaticTherapyId",
                        column: x => x.NonpharmaticTherapyId,
                        principalTable: "NonpharmaticTherapy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NonpharmaticTherapy_MedicalFacilities_MedicalIntervention",
                columns: table => new
                {
                    NonpharmaticTherapyId = table.Column<int>(nullable: false),
                    MedicalFacilitiesId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonpharmaticTherapy_MedicalFacilities_MedicalIntervention", x => new { x.NonpharmaticTherapyId, x.MedicalFacilitiesId });
                    table.ForeignKey(
                        name: "FK_NonpharmaticTherapy_MedicalFacilities_MedicalIntervention_MedicalFacilities_MedicalFacilitiesId",
                        column: x => x.MedicalFacilitiesId,
                        principalTable: "MedicalFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NonpharmaticTherapy_MedicalFacilities_MedicalIntervention_NonpharmaticTherapy_NonpharmaticTherapyId",
                        column: x => x.NonpharmaticTherapyId,
                        principalTable: "NonpharmaticTherapy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommercialName_Pharmacy",
                columns: table => new
                {
                    CommercialNamesId = table.Column<int>(nullable: false),
                    PharmacyId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialName_Pharmacy", x => new { x.PharmacyId, x.CommercialNamesId });
                    table.ForeignKey(
                        name: "FK_CommercialName_Pharmacy_CommercialName_CommercialNamesId",
                        column: x => x.CommercialNamesId,
                        principalTable: "CommercialName",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommercialName_Pharmacy_Pharmacy_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicamentGroup_Pharmacy_ActiveSubstance",
                columns: table => new
                {
                    MedicamentGroupId = table.Column<int>(nullable: false),
                    PharmacyId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicamentGroup_Pharmacy_ActiveSubstance", x => new { x.MedicamentGroupId, x.PharmacyId });
                    table.ForeignKey(
                        name: "FK_MedicamentGroup_Pharmacy_ActiveSubstance_MedicamentGroup_MedicamentGroupId",
                        column: x => x.MedicamentGroupId,
                        principalTable: "MedicamentGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicamentGroup_Pharmacy_ActiveSubstance_Pharmacy_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicamentGroup_Pharmacy_Discard",
                columns: table => new
                {
                    MedicamentGroupId = table.Column<int>(nullable: false),
                    PharmacyId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicamentGroup_Pharmacy_Discard", x => new { x.MedicamentGroupId, x.PharmacyId });
                    table.ForeignKey(
                        name: "FK_MedicamentGroup_Pharmacy_Discard_MedicamentGroup_MedicamentGroupId",
                        column: x => x.MedicamentGroupId,
                        principalTable: "MedicamentGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicamentGroup_Pharmacy_Discard_Pharmacy_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_CommercialNames",
                columns: table => new
                {
                    PharmacyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Name = table.Column<string>(nullable: false),
                    MedicamentGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacy_CommercialNames", x => x.PharmacyId);
                    table.ForeignKey(
                        name: "FK_Pharmacy_CommercialNames_Pharmacy_MedicamentGroupId",
                        column: x => x.MedicamentGroupId,
                        principalTable: "Pharmacy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_Disease_ContraIndication",
                columns: table => new
                {
                    PharmacyId = table.Column<int>(nullable: false),
                    DiseaseId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacy_Disease_ContraIndication", x => new { x.PharmacyId, x.DiseaseId });
                    table.ForeignKey(
                        name: "FK_Pharmacy_Disease_ContraIndication_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pharmacy_Disease_ContraIndication_Pharmacy_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_Disease_Indication",
                columns: table => new
                {
                    PharmacyId = table.Column<int>(nullable: false),
                    DiseaseId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacy_Disease_Indication", x => new { x.PharmacyId, x.DiseaseId });
                    table.ForeignKey(
                        name: "FK_Pharmacy_Disease_Indication_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pharmacy_Disease_Indication_Pharmacy_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Disease_PreventiveMeasures_PreventiveMeasures",
                columns: table => new
                {
                    DiseaseId = table.Column<int>(nullable: false),
                    PreventiveMeasuresId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disease_PreventiveMeasures_PreventiveMeasures", x => new { x.DiseaseId, x.PreventiveMeasuresId });
                    table.ForeignKey(
                        name: "FK_Disease_PreventiveMeasures_PreventiveMeasures_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Disease_PreventiveMeasures_PreventiveMeasures_PreventiveMeasures_PreventiveMeasuresId",
                        column: x => x.PreventiveMeasuresId,
                        principalTable: "PreventiveMeasures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalFacilities_PreventiveMeasures_Preventive",
                columns: table => new
                {
                    MedicalFacilitiesId = table.Column<int>(nullable: false),
                    PreventiveMeasureId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalFacilities_PreventiveMeasures_Preventive", x => new { x.MedicalFacilitiesId, x.PreventiveMeasureId });
                    table.ForeignKey(
                        name: "FK_MedicalFacilities_PreventiveMeasures_Preventive_MedicalFacilities_MedicalFacilitiesId",
                        column: x => x.MedicalFacilitiesId,
                        principalTable: "MedicalFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicalFacilities_PreventiveMeasures_Preventive_PreventiveMeasures_PreventiveMeasureId",
                        column: x => x.PreventiveMeasureId,
                        principalTable: "PreventiveMeasures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SymptomQuestions_Disease",
                columns: table => new
                {
                    SymptomsQuestionsId = table.Column<int>(nullable: false),
                    DiseaseId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SymptomQuestions_Disease", x => new { x.DiseaseId, x.SymptomsQuestionsId });
                    table.ForeignKey(
                        name: "FK_SymptomQuestions_Disease_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SymptomQuestions_Disease_SymptomQuestions_SymptomsQuestionsId",
                        column: x => x.SymptomsQuestionsId,
                        principalTable: "SymptomQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Disease_Symptoms_Symptoms",
                columns: table => new
                {
                    DiseaseId = table.Column<int>(nullable: false),
                    SymptomsId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disease_Symptoms_Symptoms", x => new { x.DiseaseId, x.SymptomsId });
                    table.ForeignKey(
                        name: "FK_Disease_Symptoms_Symptoms_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Disease_Symptoms_Symptoms_Symptoms_SymptomsId",
                        column: x => x.SymptomsId,
                        principalTable: "Symptoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicamentGroup_Symptoms_SideEffects",
                columns: table => new
                {
                    MedicamentGroupId = table.Column<int>(nullable: false),
                    SymptomsId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicamentGroup_Symptoms_SideEffects", x => new { x.MedicamentGroupId, x.SymptomsId });
                    table.ForeignKey(
                        name: "FK_MedicamentGroup_Symptoms_SideEffects_MedicamentGroup_MedicamentGroupId",
                        column: x => x.MedicamentGroupId,
                        principalTable: "MedicamentGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicamentGroup_Symptoms_SideEffects_Symptoms_SymptomsId",
                        column: x => x.SymptomsId,
                        principalTable: "Symptoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_Symptoms_SideEffects",
                columns: table => new
                {
                    PharmacyId = table.Column<int>(nullable: false),
                    SymptomId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    SymptomsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacy_Symptoms_SideEffects", x => new { x.PharmacyId, x.SymptomId });
                    table.ForeignKey(
                        name: "FK_Pharmacy_Symptoms_SideEffects_Pharmacy_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pharmacy_Symptoms_SideEffects_Symptoms_SymptomsId",
                        column: x => x.SymptomsId,
                        principalTable: "Symptoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SymptomQuestions_Symptom",
                columns: table => new
                {
                    SymptomQuestionsId = table.Column<int>(nullable: false),
                    SymptomsId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    BondStrength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SymptomQuestions_Symptom", x => new { x.SymptomsId, x.SymptomQuestionsId });
                    table.ForeignKey(
                        name: "FK_SymptomQuestions_Symptom_SymptomQuestions_SymptomQuestionsId",
                        column: x => x.SymptomQuestionsId,
                        principalTable: "SymptomQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SymptomQuestions_Symptom_Symptoms_SymptomsId",
                        column: x => x.SymptomsId,
                        principalTable: "Symptoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TherapyNews_Disease_Sick",
                columns: table => new
                {
                    TherapyNewsId = table.Column<int>(nullable: false),
                    DiseaseId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TherapyNews_Disease_Sick", x => new { x.TherapyNewsId, x.DiseaseId });
                    table.ForeignKey(
                        name: "FK_TherapyNews_Disease_Sick_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TherapyNews_Disease_Sick_TherapyNews_TherapyNewsId",
                        column: x => x.TherapyNewsId,
                        principalTable: "TherapyNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client_Disease",
                columns: table => new
                {
                    ClientId = table.Column<int>(nullable: false),
                    DiseaseId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client_Disease", x => new { x.DiseaseId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_Client_Disease_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Client_Disease_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client_Document",
                columns: table => new
                {
                    ClientId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    FileUrl = table.Column<string>(nullable: true),
                    UploadedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    ClientId1 = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client_Document", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Client_Document_Users_ClientId1",
                        column: x => x.ClientId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client_Questionaire",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    UserId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client_Questionaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Client_Questionaire_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client_Therapy",
                columns: table => new
                {
                    ClientId = table.Column<int>(nullable: false),
                    PharmacyId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client_Therapy", x => new { x.PharmacyId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_Client_Therapy_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Client_Therapy_Pharmacy_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    UserId = table.Column<int>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    WorkPlace = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorDetails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Subject = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrustedLogins",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    UserId = table.Column<int>(nullable: false),
                    SharedSecret = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrustedLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrustedLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAnamnesis",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    UserId = table.Column<int>(nullable: false),
                    IsFamilyAnamnesis = table.Column<bool>(nullable: false),
                    IsPersonalAnamnesis = table.Column<bool>(nullable: false),
                    IsPharmacyAnamnesis = table.Column<bool>(nullable: false),
                    IsAllergyAnamnesis = table.Column<bool>(nullable: false),
                    IsAbususAnamnesis = table.Column<bool>(nullable: false),
                    IsSocialAnamnesis = table.Column<bool>(nullable: false),
                    IsGeneralAnamnesis = table.Column<bool>(nullable: false),
                    IsFamily_ICHS = table.Column<bool>(nullable: false),
                    IsFamily_ValveDefect = table.Column<bool>(nullable: false),
                    IsFamily_AtrialFibrillation = table.Column<bool>(nullable: false),
                    IsFamily_SuddenDeath = table.Column<bool>(nullable: false),
                    IsFamily_Pacemaker = table.Column<bool>(nullable: false),
                    IsFamily_HeartAttack = table.Column<bool>(nullable: false),
                    IsPersonal_DiseaseId = table.Column<int>(nullable: false),
                    IsPersonal_NonpharmaticId = table.Column<int>(nullable: false),
                    IsPersonal_Type = table.Column<int>(nullable: false),
                    IsPersonal_Date = table.Column<DateTime>(nullable: false),
                    IsPersonal_Value = table.Column<string>(nullable: true),
                    IsPharmacy_PharmacyId = table.Column<int>(nullable: false),
                    IsPharmacy_BoldStr = table.Column<int>(nullable: false),
                    IsPharmacy_Name = table.Column<string>(nullable: true),
                    IsPharmacy_Dose = table.Column<string>(nullable: true),
                    IsPharmacy_MorningDose = table.Column<string>(nullable: true),
                    IsPharmacy_AfternoonDose = table.Column<string>(nullable: true),
                    IsPharmacy_EveningDose = table.Column<string>(nullable: true),
                    IsPharmacy_Note = table.Column<string>(nullable: true),
                    IsAllergy_Name = table.Column<string>(nullable: true),
                    IsAbusus_Alcohol = table.Column<bool>(nullable: false),
                    IsAbusus_Exsmoker = table.Column<bool>(nullable: false),
                    IsAbusus_Smoker = table.Column<bool>(nullable: false),
                    IsSocial_LivingWithPartner = table.Column<bool>(nullable: false),
                    IsSocial_Working = table.Column<bool>(nullable: false),
                    IsSocial_Pension = table.Column<bool>(nullable: false),
                    IsSocial_PartialDisabilityPension = table.Column<bool>(nullable: false),
                    IsSocial_DisabilityPension = table.Column<bool>(nullable: false),
                    IsGeneral_PhysicalActivityFrequencyType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnamnesis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAnamnesis_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    UserId = table.Column<int>(nullable: false),
                    PIN = table.Column<string>(nullable: false),
                    DoctorId = table.Column<int>(nullable: false),
                    IsSubscription = table.Column<bool>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    StreetNumber = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Zip = table.Column<string>(nullable: true),
                    InsuranceCompanyCode = table.Column<int>(nullable: false),
                    InsuranceNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDetail_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDiseases",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    UserId = table.Column<int>(nullable: false),
                    DiseaseId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    StartDateString = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDiseases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDiseases_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDiseases_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNonpharmacy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    UserId = table.Column<int>(nullable: false),
                    NonpharmaticTherapyId = table.Column<int>(nullable: false),
                    FacilityName = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNonpharmacy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNonpharmacy_NonpharmaticTherapy_NonpharmaticTherapyId",
                        column: x => x.NonpharmaticTherapyId,
                        principalTable: "NonpharmaticTherapy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNonpharmacy_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPersonalDisease",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPersonalDisease", x => new { x.UserId, x.Name });
                    table.ForeignKey(
                        name: "FK_UserPersonalDisease_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPersonalNonpharmaceutical",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPersonalNonpharmaceutical", x => new { x.UserId, x.Name });
                    table.ForeignKey(
                        name: "FK_UserPersonalNonpharmaceutical_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserReport",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Title = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    ReportType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReport_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client_QuestionAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    SymptomQuestionId = table.Column<int>(nullable: false),
                    Approved = table.Column<bool>(nullable: false),
                    Client_QuestionaireId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client_QuestionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Client_QuestionAnswers_Client_Questionaire_Client_QuestionaireId",
                        column: x => x.Client_QuestionaireId,
                        principalTable: "Client_Questionaire",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Client_QuestionAnswers_SymptomQuestions_SymptomQuestionId",
                        column: x => x.SymptomQuestionId,
                        principalTable: "SymptomQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionComments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Text = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionComments_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_QuestionComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "UserReportFile",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    UserReportId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Extension = table.Column<string>(nullable: false),
                    Content = table.Column<byte[]>(nullable: false),
                    MimeType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReportFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReportFile_UserReport_UserReportId",
                        column: x => x.UserReportId,
                        principalTable: "UserReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Created", "DeleteDate", "Email", "EmailComfirmed", "FirstName", "Guid", "LastName", "LastUpdateDate", "MFAConfirmed", "MFARecovery", "MFASecret", "MFATimeSlice", "PasswordHash", "ReplyEmailNotification", "SMSMFA", "SubscriptionPreferences", "TherapyNewsEmailNotification", "UserType" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "info@memos.cz", false, "Memos", new Guid("00000000-0000-0000-0000-000000000000"), "Memos", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, 0L, "Qz0BICLJeokFZ5xZVcz4ZxtvFs/xvCYxp/+2yZKSBjfKVER7", false, null, 0, false, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Client_Disease_ClientId",
                table: "Client_Disease",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_Document_ClientId1",
                table: "Client_Document",
                column: "ClientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Client_Questionaire_UserId",
                table: "Client_Questionaire",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_QuestionAnswers_Client_QuestionaireId",
                table: "Client_QuestionAnswers",
                column: "Client_QuestionaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_QuestionAnswers_SymptomQuestionId",
                table: "Client_QuestionAnswers",
                column: "SymptomQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_Therapy_ClientId",
                table: "Client_Therapy",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialName_Pharmacy_CommercialNamesId",
                table: "CommercialName_Pharmacy",
                column: "CommercialNamesId");

            migrationBuilder.CreateIndex(
                name: "IX_Disease_MedicamentGroup_MedicamentGroup_MedicamentGroupId",
                table: "Disease_MedicamentGroup_MedicamentGroup",
                column: "MedicamentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Disease_NonpharmaticTherapy_NonpharmaticTherapy_NonpharmaticTherapyId",
                table: "Disease_NonpharmaticTherapy_NonpharmaticTherapy",
                column: "NonpharmaticTherapyId");

            migrationBuilder.CreateIndex(
                name: "IX_Disease_PreventiveMeasures_PreventiveMeasures_PreventiveMeasuresId",
                table: "Disease_PreventiveMeasures_PreventiveMeasures",
                column: "PreventiveMeasuresId");

            migrationBuilder.CreateIndex(
                name: "IX_Disease_Symptoms_Symptoms_SymptomsId",
                table: "Disease_Symptoms_Symptoms",
                column: "SymptomsId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorDetails_UserId",
                table: "DoctorDetails",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalFacilities_PreventiveMeasures_Preventive_PreventiveMeasureId",
                table: "MedicalFacilities_PreventiveMeasures_Preventive",
                column: "PreventiveMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicamentGroup_Atc_AtcId",
                table: "MedicamentGroup_Atc",
                column: "AtcId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicamentGroup_Disease_Contraindication_DiseaseId",
                table: "MedicamentGroup_Disease_Contraindication",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicamentGroup_Disease_Indication_DiseaseId",
                table: "MedicamentGroup_Disease_Indication",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicamentGroup_Pharmacy_ActiveSubstance_PharmacyId",
                table: "MedicamentGroup_Pharmacy_ActiveSubstance",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicamentGroup_Pharmacy_Discard_PharmacyId",
                table: "MedicamentGroup_Pharmacy_Discard",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicamentGroup_Symptoms_SideEffects_SymptomsId",
                table: "MedicamentGroup_Symptoms_SideEffects",
                column: "SymptomsId");

            migrationBuilder.CreateIndex(
                name: "IX_NonpharmaticTherapy_Disease_Contraindication_DiseaseId",
                table: "NonpharmaticTherapy_Disease_Contraindication",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_NonpharmaticTherapy_Disease_Indication_DiseaseId",
                table: "NonpharmaticTherapy_Disease_Indication",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_NonpharmaticTherapy_MedicalFacilities_MedicalIntervention_MedicalFacilitiesId",
                table: "NonpharmaticTherapy_MedicalFacilities_MedicalIntervention",
                column: "MedicalFacilitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DiseaseId",
                table: "Notifications",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_CommercialNames_MedicamentGroupId",
                table: "Pharmacy_CommercialNames",
                column: "MedicamentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Disease_ContraIndication_DiseaseId",
                table: "Pharmacy_Disease_ContraIndication",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Disease_Indication_DiseaseId",
                table: "Pharmacy_Disease_Indication",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_Symptoms_SideEffects_SymptomsId",
                table: "Pharmacy_Symptoms_SideEffects",
                column: "SymptomsId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionComments_QuestionId",
                table: "QuestionComments",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionComments_UserId",
                table: "QuestionComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_UserId",
                table: "Questions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SymptomQuestions_Disease_SymptomsQuestionsId",
                table: "SymptomQuestions_Disease",
                column: "SymptomsQuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_SymptomQuestions_Symptom_SymptomQuestionsId",
                table: "SymptomQuestions_Symptom",
                column: "SymptomQuestionsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TherapyNews_Disease_Sick_DiseaseId",
                table: "TherapyNews_Disease_Sick",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TrustedLogins_UserId",
                table: "TrustedLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnamnesis_UserId",
                table: "UserAnamnesis",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetail_UserId",
                table: "UserDetail",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDiseases_DiseaseId",
                table: "UserDiseases",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiseases_UserId",
                table: "UserDiseases",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNonpharmacy_NonpharmaticTherapyId",
                table: "UserNonpharmacy",
                column: "NonpharmaticTherapyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNonpharmacy_UserId",
                table: "UserNonpharmacy",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReport_UserId",
                table: "UserReport",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReportFile_UserReportId",
                table: "UserReportFile",
                column: "UserReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Client_Disease");

            migrationBuilder.DropTable(
                name: "Client_Document");

            migrationBuilder.DropTable(
                name: "Client_QuestionAnswers");

            migrationBuilder.DropTable(
                name: "Client_Therapy");

            migrationBuilder.DropTable(
                name: "CommercialName_Pharmacy");

            migrationBuilder.DropTable(
                name: "Disease_Disease_Causes");

            migrationBuilder.DropTable(
                name: "Disease_MedicamentGroup_MedicamentGroup");

            migrationBuilder.DropTable(
                name: "Disease_NonpharmaticTherapy_NonpharmaticTherapy");

            migrationBuilder.DropTable(
                name: "Disease_PreventiveMeasures_PreventiveMeasures");

            migrationBuilder.DropTable(
                name: "Disease_Symptoms_Symptoms");

            migrationBuilder.DropTable(
                name: "DoctorDetails");

            migrationBuilder.DropTable(
                name: "EmailTemplate");

            migrationBuilder.DropTable(
                name: "HospitalizationLengths");

            migrationBuilder.DropTable(
                name: "MedicalFacilities_PreventiveMeasures_Preventive");

            migrationBuilder.DropTable(
                name: "MedicamentGroup_Atc");

            migrationBuilder.DropTable(
                name: "MedicamentGroup_Disease_Contraindication");

            migrationBuilder.DropTable(
                name: "MedicamentGroup_Disease_Indication");

            migrationBuilder.DropTable(
                name: "MedicamentGroup_Pharmacy_ActiveSubstance");

            migrationBuilder.DropTable(
                name: "MedicamentGroup_Pharmacy_Discard");

            migrationBuilder.DropTable(
                name: "MedicamentGroup_Symptoms_SideEffects");

            migrationBuilder.DropTable(
                name: "NonpharmaticTherapy_Disease_Contraindication");

            migrationBuilder.DropTable(
                name: "NonpharmaticTherapy_Disease_Indication");

            migrationBuilder.DropTable(
                name: "NonpharmaticTherapy_MedicalFacilities_MedicalIntervention");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PasswordResetTickets");

            migrationBuilder.DropTable(
                name: "Pharmacy_CommercialNames");

            migrationBuilder.DropTable(
                name: "Pharmacy_Disease_ContraIndication");

            migrationBuilder.DropTable(
                name: "Pharmacy_Disease_Indication");

            migrationBuilder.DropTable(
                name: "Pharmacy_Symptoms_SideEffects");

            migrationBuilder.DropTable(
                name: "PhoneAuth");

            migrationBuilder.DropTable(
                name: "Pils");

            migrationBuilder.DropTable(
                name: "QuestionComments");

            migrationBuilder.DropTable(
                name: "SymptomQuestions_Disease");

            migrationBuilder.DropTable(
                name: "SymptomQuestions_Symptom");

            migrationBuilder.DropTable(
                name: "TherapyNews_Disease_Sick");

            migrationBuilder.DropTable(
                name: "TrustedLogins");

            migrationBuilder.DropTable(
                name: "UserAnamnesis");

            migrationBuilder.DropTable(
                name: "UserDetail");

            migrationBuilder.DropTable(
                name: "UserDiseases");

            migrationBuilder.DropTable(
                name: "UserNonpharmacy");

            migrationBuilder.DropTable(
                name: "UserPersonalDisease");

            migrationBuilder.DropTable(
                name: "UserPersonalNonpharmaceutical");

            migrationBuilder.DropTable(
                name: "UserPharmacy");

            migrationBuilder.DropTable(
                name: "UserReportFile");

            migrationBuilder.DropTable(
                name: "VideoRooms");

            migrationBuilder.DropTable(
                name: "Client_Questionaire");

            migrationBuilder.DropTable(
                name: "CommercialName");

            migrationBuilder.DropTable(
                name: "PreventiveMeasures");

            migrationBuilder.DropTable(
                name: "Atc");

            migrationBuilder.DropTable(
                name: "MedicamentGroup");

            migrationBuilder.DropTable(
                name: "MedicalFacilities");

            migrationBuilder.DropTable(
                name: "Pharmacy");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "SymptomQuestions");

            migrationBuilder.DropTable(
                name: "Symptoms");

            migrationBuilder.DropTable(
                name: "TherapyNews");

            migrationBuilder.DropTable(
                name: "Disease");

            migrationBuilder.DropTable(
                name: "NonpharmaticTherapy");

            migrationBuilder.DropTable(
                name: "UserReport");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
