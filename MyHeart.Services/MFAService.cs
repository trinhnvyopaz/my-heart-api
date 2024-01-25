using Microsoft.EntityFrameworkCore;
using MyHeart.Data;
using MyHeart.DTO;
using MyHeart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoFactorAuthNet;

namespace MyHeart.Services {
    public class MFAService : IMFAService {
        private readonly MyHeartContext _context;
        private TwoFactorAuth _auth => new TwoFactorAuth("MyHeart");
        private Random _rnd => new Random();
        private readonly ISMSService _sms;

        public MFAService(MyHeartContext context, ISMSService sms) {
            _context = context;
            _sms = sms;
        }

        public async Task<MFAResponseDTO> CreateNewCodeForUser(int userId) {
            var secret = _auth.CreateSecret(160);

            var user = await _context.Users.
                Where(x => x.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null) {
                return new MFAResponseDTO() {
                    Link = "NoUser",
                    SingleUse = "NoUser"
                };
            }

            user.MFASecret = secret;
            user.MFATimeSlice = 0;
            user.MFARecovery = AuthenticationService.CryptoSafeGuid().ToString().Replace("-", "").Substring(0, 16);

            await _context.SaveChangesAsync();

            var qrtext = _auth.GetQrText("MyHeart", secret);

            return new MFAResponseDTO() {
                Link = AuthenticationService.MakeQrImageStringFromText(qrtext),
                SingleUse = user.MFARecovery
            };
        }

        public async Task<bool> ValidateCode(int userId, string code) {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Include(x => x.TrustedLogins)
                .FirstOrDefaultAsync();

            if (user == null) {
                return false;
            }

            var sanitisedCode = code.Replace(" ", "");

            if (sanitisedCode.Length == 6) {
                bool check = false;
                long slice = 0;
                if(user.MFASecret != null && user.MFASecret != "") {
                    check = _auth.VerifyCode(user.MFASecret, sanitisedCode, out slice);
                }

                if (check && slice > user.MFATimeSlice) {
                    user.MFATimeSlice = slice;
                    await _context.SaveChangesAsync();
                    return true;
                }

                if (user.SMSMFA == code) {
                    user.SMSMFA = "";
                    await _context.SaveChangesAsync();
                    return true;
                }

            } else if (sanitisedCode.Length == 16) {
                if (sanitisedCode == user.MFARecovery) {
                    await Remove2FA(userId);
                    return true;
                }
            } else if (Guid.TryParse(sanitisedCode, out var guid)) {
                var trustedLogin = user.TrustedLogins
                    .Where(x => x.UserId == user.Id && x.SharedSecret == guid)
                    .FirstOrDefault();

                if (trustedLogin != null) {
                    _context.Remove(trustedLogin);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> FirstValidation(int id) {
            var user = await _context.Users
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if(user == null) {
                return false;
            }

            user.MFAConfirmed = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DoesUserHaveActiveMFA(int userId) {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) {
                return false;
            }

            return !string.IsNullOrEmpty(user.MFASecret) && user.MFATimeSlice > 0;
        }

        public async Task<bool> Remove2FA(int userId) {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) {
                return false;
            }

            user.MFATimeSlice = 0;
            user.MFASecret = "";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AlreadyValidated(int userId) {
            var user = await _context.Users.FindAsync(userId);

            return !string.IsNullOrEmpty(user.MFASecret) && user.MFATimeSlice > 0;
        }

        public async Task<bool> SendMFASMS(int userId) {
            var user = await _context.Users
                .Where(x => x.Id == userId)
                .Include(x => x.UserDetail)
                .FirstOrDefaultAsync();

            if (user == null || user.UserDetail == null) {
                return false;
            }

            user.SMSMFA = _rnd.Next(0, 1000000).ToString("D6");

            await _sms.SendSMS("MyHeart MFA kod: " + user.SMSMFA, user.UserDetail.Phone);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
