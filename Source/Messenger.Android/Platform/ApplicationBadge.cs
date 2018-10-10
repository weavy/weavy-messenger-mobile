using Messenger.Droid.Platform;
using Messenger.Interfaces;
using Xamarin.Forms;
using XamarinShortcutBadger;

[assembly: Dependency(typeof(ApplicationBadge))]
namespace Messenger.Droid.Platform {
    /// <summary>
    /// Android implementation of setting the app badge
    /// </summary>
    public class ApplicationBadge : IApplicationBadge {
        /// <summary>
        /// Set the badge number
        /// </summary>
        /// <param name="number">the badge number</param>
        public void SetBadge(int number) {
            ShortcutBadger.ApplyCount(Forms.Context, number);
        }

    }


}