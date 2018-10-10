using Android.Content;
using Android.Webkit;

namespace Messenger.Droid.WebView {

    /// <summary>
    /// A custom web chrome client to hanlde input type=file in hybrid web view.
    /// </summary>
    public partial class FileChooserWebChromeClient : WebChromeClient {
        MainActivity activity;        
        private static int FILECHOOSER_RESULTCODE = 1;

        public FileChooserWebChromeClient(MainActivity activity) {
            this.activity = activity;
        }

        /// <summary>
        /// When file chooser is clicked
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="filePathCallback"></param>
        /// <param name="fileChooserParams"></param>
        /// <returns></returns>
        [Android.Runtime.Register("onShowFileChooser", "(Landroid/webkit/WebView;Landroid/webkit/ValueCallback;Landroid/webkit/WebChromeClient$FileChooserParams;)Z", "GetOnShowFileChooser_Landroid_webkit_WebView_Landroid_webkit_ValueCallback_Landroid_webkit_WebChromeClient_FileChooserParams_Handler")]
        public override bool OnShowFileChooser(Android.Webkit.WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams) {
            activity.mUploadMessage = filePathCallback;
            Intent i = new Intent(Intent.ActionGetContent);
            i.AddCategory(Intent.CategoryOpenable);
            i.SetType("*/*");
            activity.StartActivityForResult(Intent.CreateChooser(i, "File Chooser"), FILECHOOSER_RESULTCODE);

            return true;
        }
    }
}