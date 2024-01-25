using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class RegistrationPageViewModel : BaseViewModel
    {
        private RegisterViewModel register;
        private UserService userService;

        public RegisterViewModel Register
        {
            get => register;
            set => SetProperty(ref register, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand GoBackCommand { get; }

        public RegistrationPageViewModel()
        {
            userService = DependencyService.Resolve<UserService>();

            RegisterCommand = new Command(() => RegisterUser());
            GoBackCommand = new Command(() => Application.Current.MainPage.Navigation.PopModalAsync());

            Register = new RegisterViewModel();
        }

        private async Task RegisterUser()
        {
            bool isValid = Register.Validate();

            if (!isValid)
                return;

            var registeredUser = await userService.RegisterUser(Register);
            if (registeredUser != null)
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
                toastService.ShowSuccess("Na Váš Účet byl zaslán odkaz s aktivačním odkazem");
            }
            else
            {
                toastService.ShowError("Registrace se nezdařila");
            }
        }
    }
}
