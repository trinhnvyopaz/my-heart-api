using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using MyHeart.MobileApp.Droid.Renderers;
using MyHeart.MobileApp.ViewModels;
using MyHeart.MobileApp.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(VideoWebView), typeof(VideoCallWebVieRenderer))]
namespace MyHeart.MobileApp.Droid.Renderers
{
    public class VideoCallWebVieRenderer : WebViewRenderer
    {
        public VideoCallWebVieRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);
            if(Control != null)
            {
                Control.SetWebChromeClient(new CustomWebChromeClient(Xamarin.Essentials.Platform.CurrentActivity));
                //Control.Settings.UserAgentString = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246";
                Control.Settings.JavaScriptEnabled = true;
                Control.Settings.MediaPlaybackRequiresUserGesture = false;
            }
        }
    }
    public class CustomWebChromeClient : WebChromeClient
    {
        Activity mActivity = null;
        public CustomWebChromeClient(Activity activity)
        {            
            mActivity = activity;
        }
        public override void OnPermissionRequest(PermissionRequest request)
        {
            mActivity.RunOnUiThread(() => {
                request.Grant(request.GetResources());
            });
        }
    }
}