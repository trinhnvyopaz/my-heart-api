using AutoMapper;
using MyHeart.Data.Models;
using MyHeart.DTO.Enums;
using MyHeart.DTO.User;
using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHeart.Data.Models.ProfessionInformation;
using MyHeart.DTO.ProfessionInformation;
using MyHeart.DTO.ProfessionInformation.MedicamentGroupBonds;
using MyHeart.DTO;
using MyHeart.DTO.Questions;
using Stripe;
using MyHeart.DTO.Notification;
using Newtonsoft.Json;
using MyHeart.DTO.MedicalReport;

namespace MyHeart.Services.Mapping
{
    public class ModelToViewModelProfile : Profile
    {
        public ModelToViewModelProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            //CreateMap<UserType, UserTypeDTO>();
            CreateMap<PatientMedicalExaminationDTO, PatientMedicalExamination>()
                .ReverseMap(); // Optional: To enable two-way mapping

            CreateMap<PatientMedicalRecordDTO, PatientMedicalRecord>().ReverseMap();
            CreateMap<PatientMedicalRecord, PatientMedicalRecordDTO>()
                .ForMember(
                    dto => dto.UserReport,
                    opt =>
                        opt.MapFrom(
                            db =>
                                new UserReport
                                {
                                    Id = db.UserReportFile.UserReport.Id,
                                    UserId = db.UserReportFile.UserReport.UserId,
                                    User = db.UserReportFile.UserReport.User,
                                    UserReportFiles = db.UserReportFile.UserReport.UserReportFiles,
                                }
                        )
                )
                .ReverseMap();
            CreateMap<Disease, DiseaseDTO>()
                .ForMember(d => d.Symptoms, d => d.MapFrom(x => x.Symptoms))
                .ForMember(d => d.Causes, d => d.MapFrom(x => x.Causes))
                .ForMember(d => d.PreventiveMeasures, d => d.MapFrom(x => x.PreventiveMeasures))
                .ForMember(d => d.NonpharmaticTherapy, d => d.MapFrom(x => x.NonpharmaticTherapy))
                .ForMember(d => d.MedicamentGroup, d => d.MapFrom(x => x.MedicamentGroup))
                .ReverseMap();

            CreateMap<Symptoms, SymptomDTO>()
                .ForMember(d => d.Diseases, d => d.MapFrom(x => x.Diseases))
                .ReverseMap();

            CreateMap<Pharmacy, PharmacyDTO>()
                .ForMember(d => d.ActiveSubstance, d => d.MapFrom(x => x.ActiveSubstance))
                .ForMember(d => d.ContraIndication, d => d.MapFrom(x => x.ContraIndication))
                .ForMember(d => d.Indication, d => d.MapFrom(x => x.Indication))
                .ForMember(d => d.SideEffects, d => d.MapFrom(x => x.SideEffects))
                .ReverseMap();
            CreateMap<NonpharmaticTherapy, NonpharmacyDTO>()
                .ForMember(d => d.Complication, d => d.MapFrom(x => x.Complication))
                .ForMember(d => d.Indication, d => d.MapFrom(x => x.Indication))
                .ForMember(d => d.MedicalIntervention, d => d.MapFrom(x => x.MedicalIntervention));
            CreateMap<MedicalFacilities, MedicalFacilitiesDTO>()
                .ForMember(d => d.NonpharmaticTherapy, d => d.MapFrom(x => x.NonpharmaticTherapy))
                .ForMember(d => d.PreventiveMeasures, d => d.MapFrom(x => x.PreventiveMeasures))
                .ReverseMap();
            CreateMap<MedicamentGroup, MedicamentGroupDTO>()
                .ForMember(d => d.ActiveSubstance, d => d.MapFrom(x => x.ActiveSubstance))
                .ForMember(d => d.Indication, d => d.MapFrom(x => x.Indication))
                .ForMember(d => d.Contraindication, d => d.MapFrom(x => x.Contraindication))
                .ForMember(d => d.SideEffects, d => d.MapFrom(x => x.SideEffects))
                .ForMember(d => d.Atcs, d => d.MapFrom(x => x.Atcs));
            CreateMap<PreventiveMeasures, PreventiveMeasuresDTO>()
                .ForMember(d => d.WorkspaceList, d => d.MapFrom(x => x.WorkspaceList))
                .ForMember(d => d.Indication, d => d.MapFrom(x => x.Indication));
            CreateMap<TherapyNews, TherapyNewsDTO>()
                .ForMember(d => d.Diseases, d => d.MapFrom(x => x.Diseases));
            CreateMap<SymptomQuestions, SymptomQuestionsDTO>()
                .ForMember(d => d.Symptoms, d => d.MapFrom(x => x.Symptoms))
                .ForMember(d => d.Diseases, d => d.MapFrom(x => x.Diseases));
            CreateMap<CommercialName, CommercialNameDTO>()
                .ForMember(d => d.Pharmacy, d => d.MapFrom(x => x.Pharmacy));
            CreateMap<Question, QuestionListDTO>();
            CreateMap<Question, QuestionFullDto>()
                .ForMember(x => x.CreatedAt, opts => opts.MapFrom(x => x.CreationDate));

            CreateMap<QuestionComment, QuestionCommentDTO>()
                .ForMember(dto => dto.SenderId, opts => opts.MapFrom(src => src.UserId));
            CreateMap<QuestionComment, QuestionCommentFullDTO>()
                .ForMember(dto => dto.Sender, opts => opts.MapFrom(src => src.User));
            CreateMap<UserDetail, UserDetailDTO>()
                .ForMember(dto => dto.SubscriptionCancelAtPeriodEnd, opts => opts.Ignore());

            CreateMap<UserAnamnesis, UserAnamnesisDTO>();
            CreateMap<DoctorDetail, DoctorDetailDTO>();
            CreateMap<Atc, AtcDTO>()
                .ForMember(d => d.MedicamentGroups, d => d.MapFrom(x => x.MedicamentGroups))
                .ReverseMap();
            CreateMap<Pil, PilDTO>().ReverseMap();
            CreateMap<UserNonpharmacy, UserNonpharmacyDto>()
                .ForMember(d => d.User, d => d.MapFrom(x => x.User))
                .ForMember(d => d.NonpharmaticTherapy, d => d.MapFrom(x => x.NonpharmaticTherapy));

            CreateMap<UserDisease, UserDiseaseDto>()
                .ForMember(d => d.User, d => d.MapFrom(x => x.User))
                .ForMember(d => d.Disease, d => d.MapFrom(x => x.Disease));

            CreateMap<UserReport, UserReportDTO>()
                .ForMember(d => d.Files, d => d.MapFrom(x => x.UserReportFiles));

            CreateMap<UserReportFile, UserReportFileDTO>();

            CreateMap<VideoRoom, VideoRoomDTO>();

            CreateMap<Notification, NotificationDto>()
                .ForMember(d => d.Disease, d => d.MapFrom(x => x.Disease));

            CreateMap<Client_QuestionAnswer, Client_QuestionAnswerDTO>().ReverseMap();
            CreateMap<Client_Questionaire, Client_QuestionaireDTO>().ReverseMap();

            CreateMap<Data.Models.Subscription, SubscriptionDTO>().ReverseMap();

            CreateMap<User, CustomerCreateOptions>()
                .ForMember(c => c.Email, opts => opts.MapFrom(src => src.Email))
                .ForMember(
                    c => c.Name,
                    opts => opts.MapFrom(src => $"{src.FirstName} {src.LastName}")
                )
                .ReverseMap();

            CreateMap<User, CustomerUpdateOptions>()
                .ForMember(c => c.Email, opts => opts.MapFrom(src => src.Email))
                .ForMember(
                    c => c.Name,
                    opts => opts.MapFrom(src => $"{src.FirstName} {src.LastName}")
                )
                .ReverseMap();

            CreateMap<UserFmcToken, UserFmcTokenDTO>().ReverseMap();
            CreateMap<NotificationQueue, NotificationQueueDTO>()
                .ForMember(
                    x => x.CustomData,
                    opts =>
                        opts.MapFrom(
                            src =>
                                JsonConvert.DeserializeObject<Dictionary<string, string>>(
                                    src.CustomData
                                )
                        )
                )
                .ReverseMap();
            CreateMap<User, UserFmcDTO>().ReverseMap();
            CreateMap<User, UserFullDTO>().ReverseMap();

            CreateMap<SynonymBaseModel, SynonymBaseDTO>().ReverseMap();
            CreateMap<Disease_Synonyms, Disease_SynonymsDTO>().ReverseMap();
            CreateMap<Symptom_Synonyms, Symptom_SynonymsDTO>().ReverseMap();
            CreateMap<NonpharmaticTherapy_Synonyms, NonpharmaticTherapy_SynonymsDTO>().ReverseMap();
            CreateMap<PreventiveMeasures_Synonyms, PreventiveMeasures_SynonymDTO>().ReverseMap();

            #region bonds
            //disease
            CreateMap<Disease_Symptoms_Symptoms, Disease_Symptoms_SymptomsDTO>().ReverseMap();
            CreateMap<Disease_Disease_Causes, Disease_Disease_CausesDTO>().ReverseMap();
            CreateMap<
                Disease_NonpharmaticTherapy_NonpharmaticTherapy,
                Disease_NonpharmaticTherapy_NonpharmaticTherapyDTO
            >()
                .ReverseMap();
            CreateMap<
                Disease_PreventiveMeasures_PreventiveMeasures,
                Disease_PreventiveMeasures_PreventiveMeasuresDTO
            >()
                .ReverseMap();
            //medicamentGroup
            CreateMap<MedicamentGroup_Disease_Indication, MedicamentGroup_Disease_IndicationDTO>()
                .ReverseMap();
            CreateMap<
                MedicamentGroup_Disease_Contraindication,
                MedicamentGroup_Disease_ContraindicationDTO
            >()
                .ReverseMap();
            CreateMap<
                MedicamentGroup_Pharmacy_ActiveSubstance,
                MedicamentGroup_Pharmacy_ActiveSubstanceDTO
            >()
                .ReverseMap();
            CreateMap<
                MedicamentGroup_Symptoms_SideEffects,
                MedicamentGroup_Symptoms_SideEffectsDTO
            >()
                .ReverseMap();
            CreateMap<MedicamentGroup_Atc, MedicamentGroup_AtcDTO>().ReverseMap();
            //pharmacy
            CreateMap<Pharmacy_CommercialNames, Pharmacy_CommercialNamesDTO>()
                .ReverseMap();
            CreateMap<Pharmacy_Disease_ContraIndication, Pharmacy_Disease_ContraIndicationDTO>()
                .ReverseMap();
            CreateMap<Pharmacy_Disease_Indication, Pharmacy_Disease_IndicationDTO>().ReverseMap();
            CreateMap<Pharmacy_Symptoms_SideEffects, Pharmacy_Symptoms_SideEffectsDTO>()
                .ReverseMap();
            //nonpharmacy
            CreateMap<
                NonpharmaticTherapy_Disease_Contraindication,
                NonpharmaticTherapy_Disease_ContraindicationDTO
            >()
                .ReverseMap();
            CreateMap<
                NonpharmaticTherapy_Disease_Indication,
                NonpharmaticTherapy_Disease_IndicationDTO
            >()
                .ReverseMap();
            CreateMap<
                NonpharmaticTherapy_MedicalFacilities_MedicalIntervention,
                NonpharmaticTherapy_MedicalFacilities_MedicalInterventionDTO
            >()
                .ReverseMap();
            //MedicalFacilities
            CreateMap<
                MedicalFacilities_PreventiveMeasures_Preventive,
                MedicalFacilities_PreventiveMeasures_PreventiveDTO
            >()
                .ReverseMap();
            CreateMap<TherapyNews_Disease_Sick, TherapyNews_Disease_SickDTO>().ReverseMap();
            //SymptomQuestions
            CreateMap<SymptomQuestions_Symptom, SymptomQuestions_SymptomDTO>()
                .ForMember(dto => dto.SymptomId, opt => opt.MapFrom(db => db.SymptomsId))
                .ForMember(dto => dto.Symptom, opt => opt.MapFrom(db => db.Symptoms))
                .ReverseMap()
                .ForMember(x => x.Symptoms, x => x.Ignore());
            //SymptomQuestions_Disease
            CreateMap<SymptomQuestions_Disease, SymptomQuestions_DiseaseDTO>()
                .ForMember(x => x.SymptomQuestionsId, x => x.MapFrom(y => y.SymptomsQuestionsId))
                .ReverseMap()
                .ForMember(x => x.Disease, x => x.Ignore());
            //CommercialName
            CreateMap<CommercialName_Pharmacy, CommercialName_PharmacyDTO>()
                .ReverseMap();
            CreateMap<CommercialName_Pharmacy, CommercialName_PharmacyDTO>().ReverseMap();
            CreateMap<Parameter, ParameterDTO>().ReverseMap();
            CreateMap<Marker, MarkerDTO>().ReverseMap();

            #endregion
        }
    }
}
