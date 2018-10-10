using Android.App;
using Messenger.Droid.Platform;
using Messenger.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(NavigationBarColor))]
namespace Messenger.Droid.Platform {

    /// <summary>
    /// Android implementation of setting the status bar color
    /// </summary>
    public class NavigationBarColor : INavigationBarColor {

        /// <summary>
        /// Set the color for the status bar according to the Weavy theming color
        /// </summary>
        /// <param name="hexColor">The hex color to use</param>
        public void SetStatusBarColor(string hexColor) {

            // parse to droid color
            var color = Android.Graphics.Color.ParseColor(hexColor);
            
            // set status bar color
            ((Activity)Forms.Context).Window.SetStatusBarColor(color);
        }
    }

    
}