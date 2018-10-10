using System;
using System.Text;
using Android.Content;
using Android.Webkit;
using Messenger.Droid.Renderers;
using Messenger.Droid.WebView;
using Messenger.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace Messenger.Droid.Renderers {

    /// <summary>
    /// Platform specific implementation of the hybrid web view
    /// </summary>
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Android.Webkit.WebView> {

        public static Func<HybridWebViewRenderer, HybridWebViewClient> GetWebViewClientDelegate;
        private const string NativeFuncCall = "jsBridge.call";
        private const string NativeFunction = "function Native(action, data){jsBridge.call(JSON.stringify({ a: action, d: data }));}";
        Context _context;

        public HybridWebViewRenderer(Context context) : base(context) {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e) {
            base.OnElementChanged(e);

            Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);

            if (Control == null) {
                var webView = new Android.Webkit.WebView(_context);
                webView.Settings.JavaScriptEnabled = true;
                webView.Settings.DomStorageEnabled = true;
                webView.LayoutParameters = new Android.Widget.LinearLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                webView.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
                webView.SetWebViewClient(GetWebViewClient());
                webView.SetWebChromeClient(new FileChooserWebChromeClient(Context as MainActivity));
                SetNativeControl(webView);
            }
            if (e.OldElement != null) {
                Control.RemoveJavascriptInterface("jsBridge");
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.Cleanup();
            }
            if (e.NewElement != null) {
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                Control.LoadUrl(Element.Uri);

                // handle javascript injection requests
                e.NewElement.JavaScriptLoadRequested += (sender, script) => {
                    InjectJS(script);
                };

                // handle go back requests
                e.NewElement.GoBackRequested += (sender, args) => {
                    if (!Control.CanGoBack()) return;

                    Control.GoBack();
                };

                // handle go formward requests
                e.NewElement.GoForwardRequested += (sender, args) => {
                    if (!Control.CanGoForward()) return;

                    Control.GoForward();
                };
            }
        }

        /// <summary>
        /// Get the custom web view client implementation
        /// </summary>
        /// <returns></returns>
        protected virtual HybridWebViewClient GetWebViewClient() {
            var d = GetWebViewClientDelegate;

            return d != null ? d(this) : new HybridWebViewClient(this);
        }

        /// <summary>
        /// On loading
        /// </summary>
        public void OnLoading() {
            var hybridWebView = Element as HybridWebView;
            if (hybridWebView == null) return;

            hybridWebView.OnLoading(this, EventArgs.Empty);
        }

        /// <summary>
        /// On error
        /// </summary>
        public void OnError(ClientError code, string description) {
            var hybridWebView = Element as HybridWebView;
            if (hybridWebView == null) return;

            hybridWebView.OnLoadError(this, EventArgs.Empty);
        }

        /// <summary>
        /// On page finished
        /// </summary>
        public void OnPageFinished() {
            var hybridWebView = Element as HybridWebView;
            if (hybridWebView == null) return;

            InjectJS(NativeFunction);
            InjectJS(GetFuncScript());
            hybridWebView.OnLoadFinished(this, EventArgs.Empty);
            hybridWebView.CanGoBack = Control.CanGoBack();
            hybridWebView.CanGoForward = Control.CanGoForward();
        }

        /// <summary>
        /// Inject script to the page
        /// </summary>
        /// <param name="script">the script to execute</param>
        void InjectJS(string script) {
            if (Control != null) {
                Control.LoadUrl(string.Format("javascript: {0}", script));
            }
        }


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