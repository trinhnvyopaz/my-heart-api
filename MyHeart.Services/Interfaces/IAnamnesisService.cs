using Microsoft.AspNetCore.Mvc;
using MyHeart.Data.Models;
using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IAnamnesisService
    {
        Task<IEnumerable<UserAnamnesisDTO>> GetPersonalPharmacy();
        Task<IEnumerable<UserAnamnesisDTO>> GetPersonalPharmacy(int id);
        Task<UserAnamnesisDTO> SavePharmacyAnamnesis(UserAnamnesisDTO model, string userName);
        Task<IEnumerable<PharmacyDTO>> GetPharmacyByNameReg(string nameReg);
        Task<IEnumerable<PharmacyDTO>> GetPharmacyByNameRegStrength(string nameReg, string strength);
        Task<bool> DeletePharmacyAnamnesis(int id);

        Task<IEnumerable<UserAnamnesisDTO>> GetPersonalAnamnesis();
        Task<IEnumerable<UserAnamnesisDTO>> GetPersonalAnamnesis(int id);
        Task<UserAnamnesisDTO> SavePersonalAnamnesis(UserAnamnesisDTO model, string userName);
        Task<UserAnamnesisDTO> UpdatePersonalAnamnesis(UserAnamnesisDTO model, string userName);
        Task<bool> DeletePersonalAnamnesis(int id);

        Task<UserAnamnesisDTO> GetAbususAnamnesis();
        Task<UserAnamnesisDTO> GetAbususAnamnesis(int userId);
        Task<UserAnamnesisDTO> SaveAbususAnamnesis(UserAnamnesisDTO model, string userName);

        Task<UserAnamnesisDTO> GetFamilyAnamnesis();
        Task<UserAnamnesisDTO> GetFamilyAnamnesis(int id);
        Task<UserAnamnesisDTO> SaveFamilyAnamnesis(UserAnamnesisDTO model, string userName);

        Task<UserAnamnesisDTO> GetSocialAnamnesis();
        Task<UserAnamnesisDTO> SaveSocialAnamnesis(UserAnamnesisDTO model, string userName);

        Task<IEnumerable<UserAnamnesisDTO>> GetAllergyAnamneses();
        Task<IEnumerable<UserAnamnesisDTO>> GetAllergyAnamneses(int id);
        Task<UserAnamnesisDTO> SaveAllergyAnamneses(UserAnamnesisDTO model, string userName);
        Task<bool> DeleteAllergyAnamnesis(int id);

        Task<IEnumerable<DiseaseDTO>> GetDiseasesByName(string searchString);
        Task<IEnumerable<DiseaseDTO>> GetAllDieseases();
        Task<DataTableResponse<DiseaseDTO>> GetDiseases(DataTableRequest request);
        Task<IEnumerable<UserDiseaseDto>> GetDiseaseAnamneses();
        Task<IEnumerable<UserDiseaseDto>> GetDiseaseAnamneses(int id);
        Task<IEnumerable<UserDiseaseDto>> GetAllDiseaseAnamneses();
        Task<UserDiseaseDto> SaveDiseaseAnamnesis(UserDiseaseDto model, string userName);
        Task<bool> DeleteDiseaseAnamnesis(int id);

        Task<byte[]> GetPersonalAnamnesisCsv();
        Task<IEnumerable<UserAnamnesisDTO>> GetAllAnamneses();
        Task<bool> DoesAnamnesisBelongToUser(int userId, int AnamnesisId);

        Task<UserAnamnesisDTO> GetGeneralAnamnesis();
        Task<UserAnamnesisDTO> SaveGeneralAnamnesis(UserAnamnesisDTO model, string userName);
        Task<IEnumerable<UserAnamnesisDTO>> GetPersonalAnamnesisByType(PersonalAnamnesisType type);
        Task<IEnumerable<UserAnamnesisDTO>> GetAllPharmacyAnamnesis();
    }
}
