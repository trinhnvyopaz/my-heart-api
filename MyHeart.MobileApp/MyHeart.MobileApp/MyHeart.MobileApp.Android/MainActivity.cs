using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android;
using Plugin.Fingerprint;
using MyHeart.MobileApp.Services;
using Xamarin.Forms;
using ZXing.Mobile;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Request;
using Android.Gms.Fitness.Result;
using Java.Util;
using Java.Util.Concurrent;
using System;
using System.Threading.Tasks;
using System.Threading;
using MyHeart.MobileApp.Droid.Helpers;
using System.Collections.Generic;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.FirebasePushNotification;
using AndroidX.Core.App;
using Android.Content;
using Android.Nfc;
using Android.Util;
using Plugin.LocalNotification;
using Android.Support.V7.View.Menu;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.EventArgs;
using Android.Content.Res;
using AndroidX.Lifecycle;

namespace MyHeart.MobileApp.Droid
{
    [Activity(Label = "MojeSrdce", Icon = "@drawable/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, SupportsPictureInPicture = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, Android.Gms.Tasks.IOnSuccessListener, Android.Gms.Tasks.IOnFailureListener
    {
        int SIGN_IN_REQUEST_CODE = 1001;

        CancellationTokenSource fetchFitDataTokenSource = new CancellationTokenSource();

        private SessionService sessionService;
        private Android.Gms.Fitness.Data.DataType currentDataType;
        private double? averageHeartRate;
        private double? averageWeight;
        private double? averageBloodPressureSystolic;
        private double? averageBloodPressureDiastolic;

        Result? googlePermissionResultCode;

        public event EventHandler<EventArgs> OnMinimizing;


        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            RequestPermissions();

            Plugin.InputKit.Platforms.Droid.Config.Init(this, savedInstanceState);

            AppCenter.Start("9ced6645-e325-4122-8b56-ffba0b60caff",
                   typeof(Analytics), typeof(Crashes));
            LocalNotificationCenter.CreateNotificationChannel();
            LocalNotificationCenter.NotifyNotificationTapped(Intent);

            Xamarin.Forms.Forms.SetFlags("Expander_Experimental", "RadioButton_Experimental");
            Rg.Plugins.Popup.Popup.Init(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            MobileBarcodeScanner.Initialize(Application);
            CrossFingerprint.SetCurrentActivityResolver(() => Xamarin.Essentials.Platform.CurrentActivity);
            NativeMedia.Platform.Init(this, savedInstanceState);
            LoadApplication(new App());
            sessionService = DependencyService.Resolve<SessionService>();
            Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);

            CrossFirebasePushNotification.Current.OnNotificationReceived += async (s, p) =>
            {
                try
                {
                    if (IsAppInForeground(this))
                    {
                        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
                        {
                            await LocalNotificationCenter.Current.RequestNotificationPermission();
                        }

                        var notification = new NotificationRequest
                        {
                            NotificationId = Guid.NewGuid().GetHashCode(),
                            Title = p.Data["title"].ToString(),
                            Description = p.Data["body"].ToString(),
                            ReturningData = Newtonsoft.Json.JsonConvert.SerializeObject(p.Data),
                        };

                        await LocalNotificationCenter.Current.Show(notification);
                    }
                }
                catch (Exception)
                {
                }
            };

            FirebasePushNotificationManager.ProcessIntent(this, Intent);
        }
        protected override void OnUserLeaveHint()
        {
            base.OnUserLeaveHint();

            OnMinimizing?.Invoke(this, EventArgs.Empty);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            MessagingCenter.Send(Xamarin.Forms.Application.Current, "OnConfigurationChanged", IsInPictureInPictureMode);
        }


        protected override void OnStop()
        {
            base.OnStop();
            System.Diagnostics.Debug.WriteLine("OnStop");
        }

        protected override void OnNewIntent(Intent intent)
        {
            LocalNotificationCenter.NotifyNotificationTapped(intent);
            base.OnNewIntent(intent);
        }

        public bool IsAppInForeground(Context context)
        {
            var activityManager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            var appProcesses = activityManager.RunningAppProcesses;
            if (appProcesses != null)
            {
                var packageName = context.PackageName;
                foreach (var appProcess in appProcesses)
                {
                    if (appProcess.Importance == Importance.Foreground && appProcess.ProcessName == packageName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void RequestPermissions()
        {
            List<string> permissionsToRequest = new List<string>();

            if (CheckSelfPermission(Manifest.Permission.Camera) != (int)Permission.Granted)
            {
                permissionsToRequest.Add(Manifest.Permission.Camera);
            }

            if (CheckSelfPermission(Manifest.Permission.RecordAudio) != (int)Permission.Granted)
            {
                permissionsToRequest.Add(Manifest.Permission.RecordAudio);
            }

            if ((int)Build.VERSION.SdkInt >= 33 && CheckSelfPermission(Manifest.Permission.PostNotifications) != (int)Permission.Granted)
            {
                permissionsToRequest.Add(Manifest.Permission.PostNotifications);
            }

            if (permissionsToRequest.Count > 0)
            {
                RequestPermissions(permissionsToRequest.ToArray(), 0);
            }
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public override void OnUserInteraction()
        {
            sessionService?.ResetSession();
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }

        private GoogleSignInAccount GoogleAuth()
        {
            return GoogleSignIn.GetAccountForExtension(this, GoogleSignInHelper.FitnessOptions);
        }

        private bool OAuthPermissionsApproved(GoogleSignInAccount account)
        {
            return GoogleSignIn.HasPermissions(account, GoogleSignInHelper.FitnessOptions);
        }

        public async Task<GoogleSignInAccount> ConnectToGoogleAccount()
        {
            var signedAccount = GoogleSignIn.GetLastSignedInAccount(this);

            if (signedAccount == null)
            {
                var signInOptions = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestIdToken("991221169920-qethab47tqt95cto3iq0heh6vn531oib.apps.googleusercontent.com")
                    .RequestEmail()
                    .AddExtension(GoogleSignInHelper.FitnessOptions)
                    .Build();

                var signInClient = GoogleSignIn.GetClient(this, signInOptions);
                var signInIntent = signInClient.SignInIntent;
                StartActivityForResult(signInIntent, SIGN_IN_REQUEST_CODE);

                return null;
            }

            var permissionsApproved = OAuthPermissionsApproved(signedAccount);
            if (!permissionsApproved)
            {
                googlePermissionResultCode = null;
                GoogleSignIn.RequestPermissions(this, SIGN_IN_REQUEST_CODE, signedAccount, GoogleSignInHelper.FitnessOptions);

                SIGN_IN_REQUEST_CODE++;

                while (googlePermissionResultCode == null)
                {
                    await Task.Delay(100);
                }

                if (googlePermissionResultCode != Result.Ok)
                {
                    return null;
                }
            }

            return signedAccount;
        }

        public async Task<FitnessData> ReadFitnessData(Android.Gms.Fitness.Data.DataType dataType)
        {
            try
            {
                currentDataType = dataType;

                averageHeartRate = null;
                averageWeight = null;
                averageBloodPressureSystolic = null;
                averageBloodPressureDiastolic = null;

                var signedAccount = await ConnectToGoogleAccount();

                if (signedAccount != null)
                {
                    // Specify the time range for the heart rate data query
                    var calendar = Calendar.Instance;
                    calendar.Add(CalendarField.Date, -1); // Retrieve data from yesterday
                    var endTime = calendar.TimeInMillis;
                    calendar.Add(CalendarField.HourOfDay, -24);
                    var startTime = calendar.TimeInMillis;


                    //Field systolicField = new Field("FIELD_BLOOD_PRESSURE_SYSTOLIC", Field.FormatFloat, Java.Lang.Boolean.True); // The last argument is a guess; adjust as needed.
                    //Field diastolicField = new Field("FIELD_BLOOD_PRESSURE_DIASTOLIC", Field.FormatFloat, Java.Lang.Boolean.True); // Adjust the last argument if necessary.

                    //// Add other fields if needed...

                    //string bloodPressureName = "com.google.blood_pressure";
                    //int format = Field.FormatFloat; // Assuming float for simplicity. Adjust as needed.
                    //string[] fieldsNames = { systolicField.Name, diastolicField.Name };
                    //string[] fieldsFormats = { systolicField.Format.ToString(), diastolicField.Format.ToString() };

                    //Android.Gms.Fitness.Data.DataType bloodPressureType = new Android.Gms.Fitness.Data.DataType(bloodPressureName, format, fieldsNames[0], fieldsFormats[0], systolicField, diastolicField);

                    var dataReadRequest = new DataReadRequest.Builder()
                        .Read(dataType)
                        .SetTimeRange(startTime, endTime, TimeUnit.Milliseconds)
                        .Build();

                    var historyClient = FitnessClass.GetHistoryClient(this, signedAccount)
                        .ReadData(dataReadRequest)
                        .AddOnSuccessListener(this)
                        .AddOnFailureListener(this);

                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(30), fetchFitDataTokenSource.Token);
                    }
                    catch (Exception ex)
                    {

                    }

                    fetchFitDataTokenSource = new CancellationTokenSource();

                    return new FitnessData
                    {
                        AverageHeartRate = averageHeartRate,
                        AverageWeigth = averageWeight,
                        AverageBloodPressureSystolic = averageBloodPressureSystolic,
                        AverageBloodPressureDiastolic = averageBloodPressureDiastolic
                    };
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Android.Content.Intent data)
        {
            if (NativeMedia.Platform.CheckCanProcessResult(requestCode, resultCode, data))
                NativeMedia.Platform.OnActivityResult(requestCode, resultCode, data);

            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == SIGN_IN_REQUEST_CODE)
            {
                googlePermissionResultCode = resultCode;

                var result = GoogleSignIn.GetSignedInAccountFromIntent(data);
                result.AddOnSuccessListener(this, new SignInSuccessListener(OnSignInSuccess));
                result.AddOnFailureListener(this, new SignInFailureListener());
            }


        }

        public void OnSuccess(Java.Lang.Object result)
        {
            if (result is DataReadResponse response)
            {
                var heartRateSum = 0.0;
                var heartRateCount = 0;

                var weightRateSum = 0.0;
                var weightRateCount = 0;

                var bloodPressureSystolicSum = 0.0;
                var bloodPressureSystolicCount = 0;

                var bloodPressureDiastolicSum = 0.0;
                var bloodPressureDiastolicCount = 0.0;

                foreach (var dataSet in response.DataSets)
                {
                    foreach (var dataPoint in dataSet.DataPoints)
                    {
                        if (dataPoint.DataType.Name == Android.Gms.Fitness.Data.DataType.TypeWeight.Name)
                        {
                            foreach (var field in dataPoint.DataType.Fields)
                            {
                                var weight = dataPoint.GetValue(field).AsFloat();
                                weightRateSum += weight;
                                weightRateCount++;
                            }
                        }
                        if (dataPoint.DataType.Name == Android.Gms.Fitness.Data.DataType.TypeHeartRateBpm.Name)
                        {
                            foreach (var field in dataPoint.DataType.Fields)
                            {
                                var heartRate = dataPoint.GetValue(field).AsFloat();
                                heartRateSum += heartRate;
                                heartRateCount++;
                            }
                        }
                        if (dataPoint.DataType.Name == Android.Gms.Fitness.Data.HealthDataTypes.TypeBloodPressure.Name)
                        {
                            foreach (var field in dataPoint.DataType.Fields)
                            {
                                if (field.Name == "blood_pressure_systolic")
                                {
                                    var bloodPressure = dataPoint.GetValue(field).AsFloat();
                                    bloodPressureSystolicSum += bloodPressure;
                                    bloodPressureSystolicCount++;
                                }
                                if (field.Name == "blood_pressure_diastolic")
                                {
                                    var bloodPressure = dataPoint.GetValue(field).AsFloat();
                                    bloodPressureDiastolicSum += bloodPressure;
                                    bloodPressureDiastolicCount++;
                                }
                            }
                        }
                    }
                }

                if (heartRateSum != 0)
                {
                    averageHeartRate = heartRateSum / heartRateCount;
                }

                if (weightRateCount != 0)
                {
                    averageWeight = weightRateSum / weightRateCount;
                }

                if (bloodPressureSystolicSum != 0)
                {
                    averageBloodPressureSystolic = bloodPressureSystolicSum / bloodPressureSystolicCount;
                }
                if (bloodPressureDiastolicSum != 0)
                {
                    averageBloodPressureDiastolic = bloodPressureDiastolicSum / bloodPressureDiastolicCount;
                }

                fetchFitDataTokenSource.Cancel();
            }
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            averageHeartRate = null;

            fetchFitDataTokenSource.Cancel();
        }

        private void OnSignInSuccess()
        {
            ReadFitnessData(currentDataType);
        }

        public class SignInSuccessListener : Java.Lang.Object, Android.Gms.Tasks.IOnSuccessListener
        {
            private Action _callback;

            public SignInSuccessListener(Action callback)
            {
                _callback = callback;
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                var account = result as GoogleSignInAccount;
                if (account != null)
                {
                    _callback.Invoke();
                }
            }
        }

        public class SignInFailureListener : Java.Lang.Object, Android.Gms.Tasks.IOnFailureListener
        {
            public void OnFailure(Java.Lang.Exception e)
            {
                // Handle the error here
            }
        }
    }
    public class FitnessData
    {
        public double? AverageHeartRate { get; set; }
        public double? AverageWeigth { get; set; }
        public double? AverageBloodPressureSystolic { get; set; }
        public double? AverageBloodPressureDiastolic { get; set; }
    }

}