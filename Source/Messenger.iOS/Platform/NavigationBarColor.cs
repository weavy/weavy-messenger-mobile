using UIKit;
using Messenger.Interfaces;
using Messenger.iOS.Platform;
using Xamarin.Forms;

[assembly: Dependency(typeof(NavigationBarColor))]
namespace Messenger.iOS.Platform {
    /// <summary>
    /// iOS implementation of setting the status bar color to the theming color
    /// </summary>
    public class NavigationBarColor : INavigationBarColor {

        /// <summary>
        /// Set the color
        /// </summary>
        /// <param name="hexColor"></param>
        public void SetStatusBarColor(string hexColor) {
            SetColor(hexColor);
        }

        /// <summary>
        /// Set the status bar style to default or light depending on the Weavy theming color
        /// </summary>
        /// <param name="hexColor"></param>
        public void SetColor(string hexColor) {
            var color = Color.FromHex(hexColor);

            // check luminosity to determine if to use dark or light status bar style (text color)
            if (color.Luminosity >= 0.5) {
                UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.Default;
            } else {
                UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
            }

        }
    }
}