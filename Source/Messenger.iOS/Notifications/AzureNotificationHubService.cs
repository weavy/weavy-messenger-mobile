using System;
using System.Globalization;

using Foundation;
using Messenger.iOS.Notifications;
using Messenger.Notifications;
using WindowsAzure.Messaging;

[assembly: Xamarin.Forms.Dependency(typeof(AzureNotificationHubService))]
namespace Messenger.iOS.Notifications {

    /// <summary>
    /// iOS implementation of the Notification Service
    /// </summary>
    public class AzureNotificationHubService : INotificationService {

        /// <summary>
        /// Register a device to the Azure notification hub
        /// </summary>
        /// <param name="userGuid">The user guid to associate the registration with</param>
        public void Register(string userGuid) {

            //Get token from APN registration process            
            var token = NSUserDefaults.StandardUserDefaults["token"];


            var ConnectionString = Constants.ConnectionString;
            var SharedKey = Constants.SharedKey;
            var NotificationHubPath = Constants.NotificationHubPath;

            if (token != null) {

                var cs = SBConnectionString.CreateListenAccess(new NSUrl(ConnectionString), SharedKey);
                var hub = new SBNotificationHub(cs, NotificationHubPath);

                hub.UnregisterAllAsync(token as NSData, (error) => {
                    if (error != null) {
                        return;
                    }

                    var tags = new NSSet(userGuid);
                    var expire = DateTime.Now.AddYears(1).ToString(CultureInfo.CreateSpecificCulture("en-US"));

                    NSError returnError;
                    hub.RegisterTemplate(token as NSData, 
                        "Template", 
                        "{\"aps\":{\"alert\":\"$(message)\", \"content-available\":\"#(silent)\", \"badge\":\"#(badge)\", \"sound\":\"$(sound)\"}, \"url\":\"$(url)\"}", 
                        expire, 
                        tags, 
                        out returnError);

                });
            }


        }
    }
}