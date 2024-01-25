using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MyHeart.MobileApp.Droid.Helpers;
using MyHeart.MobileApp.Droid.PlatformServices;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;


[assembly: Xamarin.Forms.Dependency(typeof(HealthKitDroid))]
namespace MyHeart.MobileApp.Droid.PlatformServices
{
    public class HealthKitDroid : IHealthKitService
    {
        public async Task<bool?> ConnectToAccount()
        {
            var currentActivity = Xamarin.Essentials.Platform.CurrentActivity;

            var mainActivity = currentActivity as MainActivity;

            var accountConnected = await mainActivity.ConnectToGoogleAccount();


            return accountConnected == null ? false : true;
        }

        public bool Disconnect()
        {
            try
            {
                var currentActivity = Xamarin.Essentials.Platform.CurrentActivity;

                var googleSignInOptionsBuilder = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                                                     .RequestEmail();
                var signInOptions = googleSignInOptionsBuilder
                        .AddExtension(GoogleSignInHelper.FitnessOptions)
                        .Build();

                var googleSignInClient = GoogleSignIn.GetClient(currentActivity, signInOptions);
                googleSignInClient.RevokeAccess();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<double?> FetchPreviousDayAverageBloodPressureDiastolic()
        {
            var currentActivity = Xamarin.Essentials.Platform.CurrentActivity;

            var mainActivity = currentActivity as MainActivity;

            var data = await mainActivity.ReadFitnessData(HealthDataTypes.TypeBloodPressure);

            return data.AverageBloodPressureDiastolic;
        }

        public async Task<double?> FetchPreviousDayAverageBloodPressureSystolic()
        {
            var currentActivity = Xamarin.Essentials.Platform.CurrentActivity;

            var mainActivity = currentActivity as MainActivity;

            var data = await mainActivity.ReadFitnessData(HealthDataTypes.TypeBloodPressure);

            return data.AverageBloodPressureSystolic;
        }

        public async Task<double?> FetchPreviousDayAverageBodyMass()
        {
            var currentActivity = Xamarin.Essentials.Platform.CurrentActivity;

            var mainActivity = currentActivity as MainActivity;

            var data = await mainActivity.ReadFitnessData(DataType.TypeWeight);

            return data.AverageWeigth;
        }

        public async Task<double?> FetchPreviousDayAverageHeartRate()
        {
            var currentActivity = Xamarin.Essentials.Platform.CurrentActivity;

            var mainActivity = currentActivity as MainActivity;

            var data = await mainActivity.ReadFitnessData(DataType.TypeHeartRateBpm);

            return data.AverageHeartRate;
        }
        public bool IsEnabled()
        {
            var currentActivity = Xamarin.Essentials.Platform.CurrentActivity;

            var account = GoogleSignIn.GetLastSignedInAccount(currentActivity);
            if(account == null)
            {
                return false;
            }

            var hasPermissions = GoogleSignIn.HasPermissions(account, GoogleSignInHelper.FitnessOptions);

            return hasPermissions;
        }
    }
}