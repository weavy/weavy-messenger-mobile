using Messenger.iOS.Platform;
using UIKit;
using Messenger.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(ApplicationBadge))]
namespace Messenger.iOS.Platform {

    /// <summary>
    /// iOS implementation of setting the app badge
    /// </summary>
    public class ApplicationBadge : IApplicationBadge {

        /// <summary>
        /// Set the badge
        /// </summary>
        /// <param name="number"></param>
        public void SetBadge(int number) {
            Device.BeginInvokeOnMainThread(() => {
                UIApplication.SharedApplication.ApplicationIconBadgeNumber = number;
            });
            
        }

    }


}