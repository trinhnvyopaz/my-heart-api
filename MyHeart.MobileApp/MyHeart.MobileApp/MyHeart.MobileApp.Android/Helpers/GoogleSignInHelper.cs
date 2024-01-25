using Android.App;
using Android.Content;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyHeart.MobileApp.Droid.Helpers
{
    public static class GoogleSignInHelper
    {
        public static FitnessOptions FitnessOptions => FitnessOptions.InvokeBuilder()
                   .AddDataType(DataType.TypeHeartRateBpm, FitnessOptions.AccessRead)
                   .AddDataType(DataType.TypeWeight, FitnessOptions.AccessRead)
                   .AddDataType(Android.Gms.Fitness.Data.HealthDataTypes.TypeBloodPressure, FitnessOptions.AccessRead)
                   .Build();
    }
}