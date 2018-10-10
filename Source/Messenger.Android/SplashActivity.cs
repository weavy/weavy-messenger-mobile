
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Messenger.Droid;

namespace WeavyMobile.Droid {
    [Activity(Theme = "@style/MyTheme.Splash", Icon = "@drawable/ic_launcher", RoundIcon = "@drawable/ic_launcher_round", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity {
        
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState) {
            base.OnCreate(savedInstanceState, persistentState);            
        }

        // Launches the startup task
        protected override void OnResume() {
            base.OnResume();
            var intent = new Intent(Application.Context, typeof(MainActivity));
            intent.PutExtra("url", this.Intent.GetStringExtra("url"));
            StartActivity(intent);
        }

        // Prevent the back button from canceling the startup process
        public override void OnBackPressed() { }
                
    }
}