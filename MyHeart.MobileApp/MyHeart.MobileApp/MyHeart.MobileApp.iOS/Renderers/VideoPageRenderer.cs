using Foundation;
using MyHeart.MobileApp.iOS.Renderers;
using MyHeart.MobileApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(VideoPage), typeof(VideoPageRenderer))]
namespace MyHeart.MobileApp.iOS.Renderers
{
    internal class VideoPageRenderer : PageRenderer
    {
        public override UIStatusBarStyle PreferredStatusBarStyle() => UIStatusBarStyle.LightContent; // For light content (white text)
    }
}