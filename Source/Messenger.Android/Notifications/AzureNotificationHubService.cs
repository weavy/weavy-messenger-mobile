using System;
using Acr.Settings;
using WindowsAzure.Messaging;
using Messenger.Droid.Notifications;
using Messenger.Notifications;
using Android.App;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(AzureNotificationHubService))]
namespace Messenger.Droid.Notifications {
    /// <summary>
    /// Platform specific implementation for registering to Azure Notification Hub
    /// </summary>
    public class AzureNotificationHubService: INotificationService {
        
        /// <summary>
        /// Register with Azure notification Hub
        /// </summary>
        /// <param name="userGuid">The guid of the user to register</param>
        public void Register(string userGuid) {

            // get stored token from Firebase Messaging registration process
            var token = CrossSettings.Current.Get<string>("token");

            // create azure notificaion hub
            var hub = new NotificationHub(Constants.NotificationHubPath, Constants.ConnectionString, Application.Context);
            try {
                hub.UnregisterAll(token);
            } catch (Exception ex) {
                // handle error
            }

            // notifcation payload template for Android Firebase Messaging
            const string template = "{\"data\" : { \"message\" : \"$(message)\", \"badge\" : \"#(badge)\", \"sound\" : \"default\", \"url\" : \"$(url)\" }}";
            try {
                var result = hub.RegisterTemplate(token, "Template", template, userGuid);

            } catch (Exception ex) {
                // handle error
            }
        }
    }
}