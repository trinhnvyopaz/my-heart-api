using Foundation;
using MyHeart.MobileApp.iOS.Renderers;
using MyHeart.MobileApp.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AutoHeigthWebView), typeof(AutoHeightWebViewiOS))]
namespace MyHeart.MobileApp.iOS.Renderers
{
    public class AutoHeightWebViewiOS : WkWebViewRenderer
    {
        WKWebView wkWebView;

        static WKWebViewConfiguration configuration = new WKWebViewConfiguration
        {
            AllowsInlineMediaPlayback = true,
            MediaTypesRequiringUserActionForPlayback = WKAudiovisualMediaTypes.None
        };

        public AutoHeightWebViewiOS() : base(configuration)
        {

        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (NativeView != null)
            {
                wkWebView = (WKWebView)NativeView;
                wkWebView.ScrollView.ScrollEnabled = false;

                NavigationDelegate = new CustomWKNavigationDelegate(this);
            }
        }
        public override WKNavigation LoadHtmlString(NSString htmlString, NSUrl baseUrl)
        {
            try
            {
                // Add additional HTML to ensure fonts scale correctly and don't appear extremely small and almost unreadable
                var iOSHtmlWithScaling = htmlString.ToString().Insert(0, "<meta name='viewport' content='width=device-width,initial-scale=1,maximum-scale=1' />");
                return base.LoadHtmlString((NSString)iOSHtmlWithScaling, baseUrl);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return base.LoadHtmlString(htmlString, baseUrl);
            }
        }
    }
    public class CustomWKNavigationDelegate : WKNavigationDelegate
    {

        AutoHeightWebViewiOS webViewRenderer;

        public CustomWKNavigationDelegate(AutoHeightWebViewiOS webViewRenderer)
        {
            this.webViewRenderer = webViewRenderer;
        }

        public override async void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            var wv = webViewRenderer.Element as AutoHeigthWebView;

            await System.Threading.Tasks.Task.Delay(200);

            wv.HeightRequest = webView.ScrollView.ContentSize.Height;
        }
    }
}
