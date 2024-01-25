using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.MobileApp.Services
{
    public class SymtomQuestionService : BaseService
    {
        private const string EndPoint = "SymptomQuestions/";

        public async Task<IEnumerable<SymptomQuestionsDTO>> GetSymptomQuestionListBySymptomIds(IEnumerable<int> ids)
        {
            var response = await restService.SendRequest<IEnumerable<SymptomQuestionsDTO>>($"{EndPoint}getQuestions", HttpMethod.Post, ids);

            if(response.IsSuccess)
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
