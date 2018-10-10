using Acr.Settings;
using Foundation;
using UIKit;
using Messenger.Controls;
using Messenger.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WeavyPage), typeof(WeavyPageRenderer))]
namespace Messenger.iOS.Renderers {

    /// <summary>
    /// iOS implementation of the Weavy Page
    /// </summary>
    public class WeavyPageRenderer : PageRenderer {

        private NSObject _enterBackgroundNotificationObserver;
        private NSObject _becomeActiveNotificationObserver;

        /// <summary>
        /// View disappeard
        /// </summary>
        /// <param name="animated"></param>
        public override void ViewDidDisappear(bool animated) {
            base.ViewDidDisappear(animated);

            if (_enterBackgroundNotificationObserver != null) {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_enterBackgroundNotificationObserver);
            }
            if (_becomeActiveNotificationObserver != null) {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_becomeActiveNotificationObserver);
            }
        }

        /// <summary>
        /// View will appear
        /// </summary>
        /// <param name="animated"></param>
        public override void ViewWillAppear(bool animated) {

            _enterBackgroundNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidEnterBackgroundNotification, DidDisappearCallback);
            _becomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, DidAppearCallback);

            base.ViewWillAppear(animated);
        }

        /// <summary>
        /// The view appears
        /// </summary>
        /// <param name="obj"></param>
        private void DidAppearCallback(NSNotification obj) {
            // check if there is a notification url stored from AppDelegte.DidReceiveRemoteNotification
            var notificationUrl = CrossSettings.Current.Get<string>("notification_url");
            if (notificationUrl != null) {
                var element = Element as WeavyPage;
                if (element != null) {
                    CrossSettings.Current.Remove("notification_url");

                    // trigger event (Event is subscribed to in MainPage.cs)
                    element.OnNotificationReceived(null, new NotificationEventArgs() {
                        NotificationUrl = notificationUrl
                    });
                }
            }
        }

        /// <summary>
        /// The view disappears
        /// </summary>
        /// <param name="obj"></param>
        private void DidDisappearCallback(NSNotification obj) {

        }

    }
}