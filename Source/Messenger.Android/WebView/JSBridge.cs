using System;
using Android.Webkit;
using Java.Interop;
using Messenger.Droid.Renderers;

namespace Messenger.Droid.WebView {

    /// <summary>
    /// Javascript bridge for web view
    /// </summary>
    public class JSBridge : Java.Lang.Object {
        readonly WeakReference<HybridWebViewRenderer> hybridWebViewRenderer;

        public JSBridge(HybridWebViewRenderer hybridRenderer) {
            hybridWebViewRenderer = new WeakReference<HybridWebViewRenderer>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("call")]
        public void Call(string data) {
            HybridWebViewRenderer hybridRenderer;

            if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out hybridRenderer)) {
                if (hybridRenderer != null && hybridRenderer.Element != null) {
                    hybridRenderer.Element.MessageReceived(data);
                }

            }
        }
    }
}