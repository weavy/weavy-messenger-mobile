using Android.App;
using Firebase.Iid;
using Acr.Settings;

namespace Messenger.Droid.Notifications {
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseRegistrationService : FirebaseInstanceIdService {
        
        public override void OnTokenRefresh() {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            
            // store token for later use
            CrossSettings.Current.Set("token", refreshedToken);

            // flag to update azure notification hub
            CrossSettings.Current.Set("shouldRegisterToNotificationHub", true);

        }
    }
}