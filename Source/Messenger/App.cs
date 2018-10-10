using Acr.Settings;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Messenger {
    /// <summary>
    /// Main entry point for Xamarin Forms apps
    /// </summary>
    public class App : Xamarin.Forms.Application {

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="url">The url included in a push notification. Empty if app wasn't opened from a notification</param>
        public App(string url = "") {

            // get url
            var defaultUrl = CrossSettings.Current.Get<string>("url");

            // make sure the window resizes on Android when keyboard is displayed
            Current.On<Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            if (string.IsNullOrEmpty(defaultUrl)) {
                MainPage = new SelectSitePage();
            } else {
                MainPage = new MainPage(url);
            }
            
        }

        /// <summary>
        /// App resumes
        /// </summary>
        protected override void OnResume() {            
            MessagingCenter.Send<App>(this, "APP_RESUME");
        }
    }
}
