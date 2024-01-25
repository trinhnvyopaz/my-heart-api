using MyHeart.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces {
    public interface IMFAService {
        public Task<MFAResponseDTO> CreateNewCodeForUser(int userId);
        public Task<bool> ValidateCode(int userId, string code);
        public Task<bool> DoesUserHaveActiveMFA(int userId);
        public Task<bool> AlreadyValidated(int userId);
        public Task<bool> Remove2FA(int userId);
        public Task<bool> SendMFASMS(int userId);
        Task<bool> FirstValidation(int id);
    }
}
