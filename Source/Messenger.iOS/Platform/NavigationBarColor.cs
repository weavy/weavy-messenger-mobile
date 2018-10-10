using System;
using Foundation;
using UIKit;
using Messenger.Interfaces;
using Messenger.iOS.Extensions;
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
        /// Set the color for the status bar according to the Weavy theming color
        /// </summary>
        /// <param name="hexColor"></param>
        public void SetColor(string hexColor) {
            UIView statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBar")) as UIView;
            if (statusBar.RespondsToSelector(new ObjCRuntime.Selector("setBackgroundColor:"))) {

                try {

                    // use 95% alpha channel
                    statusBar.BackgroundColor = hexColor.ToUIColor();

                    var color = Color.FromHex(hexColor);

                    // check luminosity to determine if to use dark or light status bar style (text color)
                    if (color.Luminosity >= 0.5) {
                        UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.Default;
                    } else {
                        UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
                    }
                } catch (Exception) {
                }

            }
        }
    }
}