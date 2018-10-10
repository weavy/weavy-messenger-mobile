using Acr.Settings;
using System;
using Messenger.Helpers;
using Messenger.Interfaces;
using Xamarin.Forms;

namespace Messenger.Controls {
    /// <summary>
    /// Custom Weavy content page
    /// </summary>
    public class WeavyPage : ContentPage {
        public EventHandler NotificationReceived;
                
        /// <summary>
        /// When view appears
        /// </summary>
        protected override void OnAppearing() {
            base.OnAppearing();

            // set theme color
            var color = CrossSettings.Current.Get<string>("themecolor") ?? Constants.DefaultColor;
            SetThemeColor(color);
        }

        /// <summary>
        /// Handle notification (iOS)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnNotificationReceived(object sender, NotificationEventArgs e) {
            var handler = this.NotificationReceived;
            if (handler != null) {
                handler(this, e);
            }
        }

        /// <summary>
        /// Set theme color
        /// </summary>
        /// <param name="color">The hex color to use</param>
        protected void SetThemeColor(string color) {
            INavigationBarColor nav = DependencyService.Get<INavigationBarColor>();
            
            // update theme color            
            CrossSettings.Current.Set<string>("themecolor", color);

            Device.BeginInvokeOnMainThread(() => {
                nav.SetStatusBarColor(color);
            });            
        }
    }

    /// <summary>
    /// Notification event arguments
    /// </summary>
    public class NotificationEventArgs : EventArgs {
        public string NotificationUrl { get; set; }
    }
}
