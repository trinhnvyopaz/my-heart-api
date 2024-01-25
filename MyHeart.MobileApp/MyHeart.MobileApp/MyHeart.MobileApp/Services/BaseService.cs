using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Services
{
    public class BaseService
    {
        protected RestService restService;

        public BaseService()
        {
            restService = DependencyService.Resolve<RestService>();
        }
    }
}
