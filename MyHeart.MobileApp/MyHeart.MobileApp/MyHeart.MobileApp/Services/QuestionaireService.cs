using MyHeart.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.MobileApp.Services
{
    public class QuestionaireService : BaseService
    {
        private string EndPoint = "Questionaire/";

        public async Task<Client_QuestionaireDTO> CreateQuestionaire(Client_QuestionaireDTO questionaire)
        {
            var response = await restService.SendRequest<Client_QuestionaireDTO>(EndPoint, HttpMethod.Post, questionaire);

            if(response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<Client_QuestionaireResultDTO> GetResult(int questionaireid)
        {
            var response = await restService.SendRequest<Client_QuestionaireResultDTO>($"{EndPoint}result/{questionaireid}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
    }
}
