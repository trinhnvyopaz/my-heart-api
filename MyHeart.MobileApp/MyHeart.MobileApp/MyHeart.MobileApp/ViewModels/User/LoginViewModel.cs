using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MyHeart.MobileApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string invalidMessage;
        private string username;

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string InvalidMessage
        {
            get => invalidMessage;
            set => SetProperty(ref invalidMessage, value);
        }
        public string MfaCode { get; set; }
    }
}
