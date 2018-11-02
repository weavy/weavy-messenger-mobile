using System;
using Acr.Settings;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Webkit;
using Messenger.Droid.Renderers;
using Messenger.Helpers;

namespace Messenger.Droid.WebView {

    /// <summary>
    /// Custom implementation of a WebViewClient
    /// </summary>
    public class HybridWebViewClient : WebViewClient {
        private const string INTERNET_DISCONNECTED = "net::ERR_INTERNET_DISCONNECTED";
        private const string CONNECTION_ABORTED = "net::ERR_CONNECTION_ABORTED";
        protected readonly WeakReference<HybridWebViewRenderer> WebHybrid;

        public HybridWebViewClient(HybridWebViewRenderer webHybrid) {
            this.WebHybrid = new WeakReference<HybridWebViewRenderer>(webHybrid);

        }

        /// <summary>
        /// When web page has finished loading
        /// </summary>
        /// <param name="view"></param>
        /// <param name="url"></param>
        public override void OnPageFinished(Android.Webkit.WebView view, string url) {
            base.OnPageFinished(view, url);

            HybridWebViewRenderer hybrid;
            if (this.WebHybrid != null && this.WebHybrid.TryGetTarget(out hybrid)) {
                hybrid.OnPageFinished();
            }
        }

        /// <summary>
        /// When web page starts loading
        /// </summary>
        /// <param name="view"></param>
        /// <param name="url"></param>
        /// <param name="favicon"></param>
        public override void OnPageStarted(Android.Webkit.WebView view, string url, Bitmap favicon) {
            base.OnPageStarted(view, url, favicon);

            HybridWebViewRenderer hybrid;
            if (this.WebHybrid != null && this.WebHybrid.TryGetTarget(out hybrid)) {
                hybrid.OnLoading();
            }
        }

        /// <summary>
        /// Handle errors in web view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="request"></param>
        /// <param name="error"></param>
        public override void OnReceivedError(Android.Webkit.WebView view, IWebResourceRequest request, WebResourceError error) {
            base.OnReceivedError(view, request, error);

            HybridWebViewRenderer hybrid;
            if (this.WebHybrid != null && this.WebHybrid.TryGetTarget(out hybrid)) {

                if (!error.Description.Equals(INTERNET_DISCONNECTED) && !error.Description.Equals(CONNECTION_ABORTED)) {
                    hybrid.OnError(error.ErrorCode, error.Description);
                }
            }
        }

    }
}