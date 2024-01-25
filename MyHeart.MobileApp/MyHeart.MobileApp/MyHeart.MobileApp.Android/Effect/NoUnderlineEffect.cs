﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MyHeart.MobileApp.Droid.Effect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("effects")]
[assembly: ExportEffect(typeof(NoUnderlineEffect), nameof(NoUnderlineEffect))]
namespace MyHeart.MobileApp.Droid.Effect
{
    public class NoUnderlineEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Control.Background = null;
            Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }

        protected override void OnDetached()
        {

        }
    }
}