using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string email;
        private string firstName;
        private string lastName;
        private string password;
        private string passwordAgain;
        private string emailValidationMessage;

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }
        public string EmailValidationMessage
        {
            get => emailValidationMessage;
            set => SetProperty(ref emailValidationMessage, value);
        }
        public string FirstName
        {
            get => firstName;
            set => SetProperty(ref firstName, value);
        }
        public string FirstNameValidationMessage
        {
            get => emailValidationMessage;
            set => SetProperty(ref emailValidationMessage, value);
        }
        public string LastName
        {
            get => lastName;
            set => SetProperty(ref lastName, value);
        }
        public string LastNameValidationMessage
        {
            get => emailValidationMessage;
            set => SetProperty(ref emailValidationMessage, value);
        }
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }
        public string PasswordValidationMessage
        {
            get => emailValidationMessage;
            set => SetProperty(ref emailValidationMessage, value);
        }
        public string PasswordAgain
        {
            get => passwordAgain;
            set => SetProperty(ref passwordAgain, value);
        }
        public string PasswordAgainValidationMessage
        {
            get => emailValidationMessage;
            set => SetProperty(ref emailValidationMessage, value);
        }

        public bool Validate()
        {
            EmailValidationMessage = null;
            FirstNameValidationMessage = null;
            LastNameValidationMessage = null;
            PasswordValidationMessage = null;
            PasswordAgainValidationMessage = null;

            bool valid = true;

            if (string.IsNullOrEmpty(Email))
            {
                EmailValidationMessage = "Email je povinný";
                valid = false;
            }
            if (string.IsNullOrEmpty(FirstName))
            {
                FirstNameValidationMessage = "Jméno je povinné";
                valid = false;
            }
            if (string.IsNullOrEmpty(LastName))
            {
                LastNameValidationMessage = "Příjmení je povinné";
                valid = false;
            }
            if (string.IsNullOrEmpty(Password))
            {
                PasswordValidationMessage = "Heslo je povinné";
                valid = false;
            }
            if(Password == null && PasswordAgain == null)
            {
                if (string.IsNullOrEmpty(PasswordAgain))
                {
                    PasswordAgainValidationMessage = "Hesla znovu je povinné";
                    valid = false;
                }
            }
            else
            {
                if (Password != PasswordAgain)
                {
                    PasswordAgainValidationMessage = "Hesla se neshodují";
                    valid = false;
                }
            }
       
            return valid;
        }
    }
}
