using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Android.Webkit;
using Android.Runtime;
using System;
using Messenger.Droid.Notifications;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using System.Threading.Tasks;
using Firebase.Iid;

namespace Messenger.Droid {
    [Activity(Label = "Messenger", 
        Icon = "@drawable/ic_launcher", 
        RoundIcon = "@drawable/ic_launcher_round",
        Theme = "@style/MainTheme", 
        MainLauncher = false, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        WindowSoftInputMode = SoftInput.AdjustPan)]
    public class MainActivity : FormsAppCompatActivity
    {

        public IValueCallback mUploadMessage;
        private static int FILECHOOSER_RESULTCODE = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var url = this.Intent.GetStringExtra("url");
            LoadApplication(new App(url));

            // try to create a notification channel
            CreateNotificationChannel();
            
#if DEBUG
            // Force refresh of the token. If we redeploy the app, no new token will be sent but the old one will
            // be invalid.
            Task.Run(() => {
                // This may not be executed on the main thread.
                FirebaseInstanceId.Instance.DeleteInstanceId();                
            });
#endif
        }

        /// <summary>
        /// Handle result from file chooser (input type=file in webView)
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data) {
            if (requestCode == FILECHOOSER_RESULTCODE) {
                if (null == mUploadMessage) return;
                Android.Net.Uri[] result = data == null || resultCode != Result.Ok ? null : new Android.Net.Uri[] { data.Data };
                try {
                    mUploadMessage.OnReceiveValue(result);

                } catch (Exception e) {
                }

                mUploadMessage = null;
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        /// <summary>
        /// Create a notification if needed api level >= 26
        /// </summary>
        private void CreateNotificationChannel() {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var name = "General notifications";
            var description = $"General notifications from {Helpers.Constants.DisplayName}";
            var channel = new NotificationChannel(Constants.CHANNEL_ID, name, NotificationImportance.High) {
                Description = description
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

    }
}