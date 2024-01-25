using MyHeart.DTO;
using MyHeart.DTO.Questions;
using MyHeart.DTO.User;
using MyHeart.MobileApp.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MyHeart.MobileApp.Services
{
    public class QuestionsService : BaseService
    {
        public const string Endpoint = "Question/";
        public async Task<QuestionListDTO> AskQuestion(QuestionListViewModel vm)
        {
            var dto = MapVmToDto(vm);
            var response = await restService.SendRequest<QuestionListDTO>($"{Endpoint}ask", HttpMethod.Post, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<DataTableResponse<QuestionListDTO>> GetOpenQuestion(DataTableRequest request)
        {
            var response = await restService.SendRequest<DataTableResponse<QuestionListDTO>>($"{Endpoint}open", HttpMethod.Post, request);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<DataTableResponse<QuestionListDTO>> GetClosedQuestion(DataTableRequest request)
        {
            var response = await restService.SendRequest<DataTableResponse<QuestionListDTO>>($"{Endpoint}closed", HttpMethod.Post, request);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<DataTableResponse<QuestionCommentDTO>> GetLastReplies(QuestionCommentRequest request)
        {
            var response = await restService.SendRequest<DataTableResponse<QuestionCommentDTO>>($"{Endpoint}lastReplies", HttpMethod.Post, request);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<VideoRoomDTO> GetVideoRoomDetail(int id)
        {
            var response = await restService.SendRequest<VideoRoomDTO>($"{Endpoint}video/{id}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<VideoRoom> JoinRoom(int questionId)
        {
            var response = await restService.SendRequest<VideoRoom>($"videoprovider/{questionId}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteQuestion(int id)
        {
            var response = await restService.SendRequest($"{Endpoint}{id}", HttpMethod.Delete, null);

            return response.IsSuccess;
        }

        public async Task<QuestionListDTO> GetQuestion(int id)
        {
            var response = await restService.SendRequest<QuestionListDTO>($"{Endpoint}{id}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        private QuestionListDTO MapVmToDto(QuestionListViewModel vm)
        {
            return new QuestionListDTO
            {
                Id = vm.Id,
                Body = vm.Body,
                CreationDate = vm.CreationDate,
                Name = vm.Name,
                Status = vm.Status,
                Subject = vm.Subject,
                UserId = vm.UserId
            };
        }
    }

    public class VideoRoom : ObservableObject
    {
        private string accessToken;
        private string name;

        public string AccessToken
        {
            get => accessToken;
            set => SetProperty(ref accessToken, value);
        }
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
    }
}
