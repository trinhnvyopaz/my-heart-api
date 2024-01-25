using MyHeart.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio.Jwt.AccessToken;

namespace MyHeart.Services
{
    public class TwilioService : IVideoProviderService
    {
        private IUserService _userService;
        private ITwilioConfiguration _twilioConfiguration;
        private IQuestionService _questionService;

        public TwilioService(ITwilioConfiguration twilioConfiguration, IUserService userService, IQuestionService questionService)
        {
            _userService = userService;
            _twilioConfiguration = twilioConfiguration;
            _questionService = questionService;
        }

        public async Task<RoomDetail> ConnectToRoom(int questionId)
        {
            var user = await _userService.CurrentUserAsync();
            var room = await _questionService.GetVideoRoomDetails(questionId);

            var grant = new VideoGrant
            {
                Room = room.RoomId.ToString(),
            };

            var token = new Token(_twilioConfiguration.AccountSID, _twilioConfiguration.ApiKeySID, _twilioConfiguration.ApiKeySecret, $"{user.Email}:{user.FirstName}:{user.LastName}" , grants: new HashSet<IGrant> { grant });

            return new RoomDetail
            {
                Name = room.RoomId.ToString(),
                AccessToken = token.ToJwt()
            };
        }
    }

    public class RoomDetail
    {
        public string Name { get; set; }
        public string AccessToken { get; set; }
    }
}
