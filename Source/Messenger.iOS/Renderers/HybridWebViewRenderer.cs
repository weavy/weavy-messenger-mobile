
using Foundation;
using System;
using System.ComponentModel;
using System.Text;
using Messenger.iOS.Renderers;
using Messenger.Views;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace Messenger.iOS.Renderers {

    /// <summary>
    /// iOS implementation of the hybrid web view
    /// </summary>
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>, IWKScriptMessageHandler {
        private const string NativeFuncCall = "window.webkit.messageHandlers.native.postMessage";
        private const string NativeFunction = "function Native(action, data){window.webkit.messageHandlers.native.postMessage(JSON.stringify({ a: action, d: data }));}";

        WKUserContentController userController;

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e) {
            base.OnElementChanged(e);

            if (Control == null) {
                userController = new WKUserContentController();
                var script = new WKUserScript(new NSString(NativeFunction + GetFuncScript()), WKUserScriptInjectionTime.AtDocumentEnd, false);
                userController.AddUserScript(script);
                userController.AddScriptMessageHandler(this, "native");

                var config = new WKWebViewConfiguration { UserContentController = userController };
                var webView = new WKWebView(Frame, config) {
                    AllowsBackForwardNavigationGestures = true,
                    WeakNavigationDelegate = this,                    
                    AllowsLinkPreview = false,                    
                };
                webView.ScrollView.Bounces = false;

                SetNativeControl(webView);
            }
            if (e.OldElement != null) {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler("invokeAction");
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.Cleanup();
            }
            if (e.NewElement != null) {                
                Control.LoadRequest(new NSUrlRequest(new NSUrl(new Uri(e.NewElement.Uri).AbsoluteUri)));

                // set js handler
                e.NewElement.JavaScriptLoadRequested += (sender, js) => {
                    Inject(js);
                };
            }
        }

        /// <summary>
        /// Received a script message from the web page
        /// </summary>
        /// <param name="userContentController"></param>
        /// <param name="message"></param>
        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message) {
            if (Element == null) return;

            Element.MessageReceived(message.Body.ToString());
        }


        /// <summary>
        /// A property has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Uri") {                
                Control.LoadRequest(new NSUrlRequest(new NSUrl(new Uri(Element.Uri).AbsoluteUri)));
            }
        }

        /// <summary>
        /// Called when navigation is complete in the webview
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="navigation"></param>
        [Export("webView:didFinishNavigation:")]
        public void DidFinishNavigation(WKWebView webView, WKNavigation navigation) {
            var element = Element as HybridWebView;

            if (element == null) return;

            // inject scripts
            Inject(NativeFunction);
            Inject(GetFuncScript());

            // call load finished on the web view
            element.OnLoadFinished(this, EventArgs.Empty);

            // set if we can navigate back/forward
            element.CanGoBack = Control.CanGoBack;
            element.CanGoForward = Control.CanGoForward;

        }


        /// <summary>
        /// inject a javascript string into to the web view
        /// </summary>
        /// <param name="script"></param>
        public void Inject(string script) {
            if (Control == null) return;

            InvokeOnMainThread(() => Control.EvaluateJavaScript((string)new NSString(script), (r, e) => {
                if (e !=
                    null) {
                    //Debug.WriteLine(e);
                }
            }));
        }

        /// <summary>
        /// Generates the script to inject into the web view
        /// </summary>
        /// <returns></returns>
        private string GetFuncScript() {
            var builder = new StringBuilder();
            builder.Append("NativeFuncs = [];");
            builder.Append("function NativeFunc(action, data, callback){");

            builder.Append("  var callbackIdx = NativeFuncs.push(callback) - 1;");
            builder.Append(NativeFuncCall);
            builder.Append("(JSON.stringify({ a: action, d: data, c: callbackIdx }));}");
            builder.Append(" if (typeof(window.NativeFuncsReady) !== 'undefined') { ");
            builder.Append("   window.NativeFuncsReady(); ");
            builder.Append(" } ");

            return builder.ToString();
        }
    }
}