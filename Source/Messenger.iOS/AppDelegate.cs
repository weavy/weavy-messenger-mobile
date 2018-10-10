using System;
using Acr.Settings;
using CoreFoundation;
using Foundation;
using UIKit;
using UserNotifications;

namespace Messenger.iOS {
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {

            // Request notification permissions from the user
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (approved, err) => {
                DispatchQueue.MainQueue.DispatchAsync(UIApplication.SharedApplication.RegisterForRemoteNotifications);
            });

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        /// <summary>
        /// Registered for notifications
        /// </summary>
        /// <param name="application"></param>
        /// <param name="deviceToken">The device token</param>
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken) {

            var currentToken = NSUserDefaults.StandardUserDefaults["token"];
            NSUserDefaults.StandardUserDefaults.SetValueForKey(deviceToken, new NSString("token"));

            if (currentToken != null && !currentToken.Equals(deviceToken)) {
                // force update to Azure notification hub
                CrossSettings.Current.Set("shouldRegisterToNotificationHub", true); 
            }
        }

        /// <summary>
        /// A notification is clicked by the user
        /// </summary>
        /// <param name="application"></param>
        /// <param name="userInfo"></param>
        /// <param name="completionHandler"></param>
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler) {
            if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Background)
                ProcessNotification(userInfo, UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active);
        }
        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo) {
            if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Background)
                ProcessNotification(userInfo, UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active);
        }

        /// <summary>
        /// Handle the notification
        /// </summary>
        /// <param name="options"></param>
        /// <param name="fromFinishedLaunching"></param>
        private void ProcessNotification(NSDictionary options, bool fromFinishedLaunching) {
            // Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
            if (null != options && options.ContainsKey(new NSString("aps"))) {
                //Get the aps dictionary
                NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;
                var url = options.ObjectForKey(new NSString("url")) as NSString;

                if (!fromFinishedLaunching) {
                    if (url != null) {
                        // set the notification url for later use (See WeavyPageRenderer.cs for more info)
                        CrossSettings.Current.Set("notification_url", url.ToString());
                    }
                }
            }
        }
    }
}
