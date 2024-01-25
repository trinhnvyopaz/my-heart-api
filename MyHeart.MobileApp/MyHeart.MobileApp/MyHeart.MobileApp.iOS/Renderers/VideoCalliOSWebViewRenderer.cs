using Foundation;
using MyHeart.MobileApp.iOS.Renderers;
using MyHeart.MobileApp.Views.Controls;
using Plugin.InputKit.Platforms.iOS;
using System;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(VideoWebView), typeof(VideoCalliOSWebViewRenderer))]
namespace MyHeart.MobileApp.iOS.Renderers
{
    public class VideoCalliOSWebViewRenderer : AutoHeightWebViewiOS, IWKScriptMessageHandler, IWKUIDelegate
    {
        const string messageName = "callDisconnected";
        readonly string JavaScriptFunction = $"function callDisconnected(data){{window.webkit.messageHandlers.{messageName}.postMessage(data);}}";
        WKUserContentController userController;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler(messageName);
            }
            if (e.NewElement != null)
            {
                var view = (WKWebView)NativeView;

                userController = view.Configuration.UserContentController;
                var script = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
                userController.AddUserScript(script);
                userController.AddScriptMessageHandler(this, messageName);

                view.UIDelegate = new PermissionHandlerUIDelegate();

#if DEBUG
                view.Inspectable = true;
#endif
            }
        }
        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            if (message.Name == messageName)
            {
                var sharedWebView = (VideoWebView)Element;
                sharedWebView.OnDisconnected();
            }
        }
        private class PermissionHandlerUIDelegate : WKUIDelegate
        {
            [Export("webView:requestMediaCapturePermissionForOrigin:initiatedByFrame:type:decisionHandler:")]
            public override void RequestMediaCapturePermission(WKWebView webView, WKSecurityOrigin origin, WKFrameInfo frame, WKMediaCaptureType type, Action<WKPermissionDecision> decisionHandler)
            {
                // Here you decide whether to grant or deny permission
                decisionHandler(WKPermissionDecision.Grant);
            }
        }

    }
}