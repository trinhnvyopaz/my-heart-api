using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MyHeart.MobileApp.Droid.Renderers;
using MyHeart.MobileApp.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AutoHeigthWebView), typeof(AutoHeigthWebViewDroid))]
namespace MyHeart.MobileApp.Droid.Renderers
{
    public class AutoHeigthWebViewDroid : WebViewRenderer
    {
        public AutoHeigthWebViewDroid(Context context) : base(context)
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            Control.SetWebViewClient(new ExtendedWebViewClient(Element as AutoHeigthWebView));
            Control.VerticalScrollBarEnabled = false;
            Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }

        class ExtendedWebViewClient : Android.Webkit.WebViewClient
        {
            AutoHeigthWebView xwebView;
            public ExtendedWebViewClient(AutoHeigthWebView webView)
            {
                xwebView = webView;
            }

            async public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                if (xwebView != null)
                {

                    await System.Threading.Tasks.Task.Delay(100);

                    xwebView.HeightRequest = view.ContentHeight;

                }

                base.OnPageFinished(view, url);
            }
        }
    }
}