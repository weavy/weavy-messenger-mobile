using System.Threading.Tasks;
using Messenger.Notifications;
using Messenger.Helpers;
using Messenger.Views;
using Xamarin.Forms;
using XLabs.Serialization.JsonNET;
using Acr.Settings;
using Xamarin.Essentials;
using System;
using Messenger.Controls;
using Messenger.Interfaces;

namespace Messenger {
    public class MainPage : WeavyPage {

        private HybridWebView _webview;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">The url to use. When a notification is received on Android, this is set to the notification url</param>
        public MainPage(string url = "") {

            // create a new hybrid webview
            _webview = new HybridWebView(new JsonSerializer()) {
                Uri = ResolveUrl(url),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            // resume event
            SubscribeToResume();

            // push notification handler
            NotificationReceived += (sender, args) => {
                var notificationArgs = args as NotificationEventArgs;
                _webview.Uri = notificationArgs.NotificationUrl;
            };

            // register callbacks
            RegisterCallbacks();

            // handle load finished
            _webview.LoadFinished += (sender, args) => {
                _webview.InjectJavaScript(ScriptHelper.ScriptChecker);
            };

            // handle webview errors
            _webview.LoadError += async (sender, args) => {

                var retry = await DisplayAlert("Connection error", $"The {Constants.DisplayShortName} site could not be loaded", "Retry", $"Select another {Constants.DisplayShortName} site");
                if (retry) {
                    _webview.Uri = ResolveUrl(url);
                } else {
                    CrossSettings.Current.Remove("userguid");
                    CrossSettings.Current.Remove("url");
                    CrossSettings.Current.Remove("theme");

                    SelectSite();

                }
            };

            // the layout
            var layout = new StackLayout {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = { _webview }
            };
            Padding = new Thickness(0, 50);
            Content = layout;
        }


        /// <summary>
        /// Overrides the harware backbutton on Android
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed() {

            if (_webview.CanGoBack) {
                _webview.GoBack();
            }
            return true;
        }

        #region private


        /// <summary>
        /// Set the Select Site page as current
        /// </summary>
        private void SelectSite() {
            Device.BeginInvokeOnMainThread(() => {
                Application.Current.MainPage = new SelectSitePage();
            });
        }

        /// <summary>
        /// Subscribe to Resume Message
        /// </summary>
        private void SubscribeToResume() {
            MessagingCenter.Subscribe<App>(this, "APP_RESUME", (s) => {
                // reconnect to rtm, update conversations and notifications
                _webview.InjectJavaScript(ScriptHelper.ReconnectScript);
                _webview.InjectJavaScript(ScriptHelper.UpdateBadgeScript);
            });
        }

        /// <summary>
        /// Register all callbacks from web page javascript
        /// </summary>
        private void RegisterCallbacks() {

            // Callback for injecting base script
            _webview.RegisterCallback("injectScriptCallback", (userGuid) => {
                Device.BeginInvokeOnMainThread(() => {
                    _webview.InjectJavaScript(ScriptHelper.Scripts);
                });

            });

            // Callback for registration to Azure Notification Hub
            _webview.RegisterCallback("registerForNotificationsCallback", (userGuid) => {
                var guid = CrossSettings.Current.Get<string>("userguid");
                var shouldRegister = CrossSettings.Current.Get<bool>("shouldRegisterToNotificationHub");

                if (guid != userGuid || shouldRegister) {
                    //Register with Azure notification hub
                    CrossSettings.Current.Set<string>("userguid", userGuid);
                    CrossSettings.Current.Set<bool>("shouldRegisterToNotificationHub", false);

                    Task.Run(() => {
                        DependencyService.Get<INotificationService>().Register(userGuid);
                    });
                }
            });

            //Callback from sign out script
            _webview.RegisterCallback("signOutCallback", (args) => {
                //Sign out
                CrossSettings.Current.Remove("userguid");
                CrossSettings.Current.Remove("theme");
            });

            //Callback from change site script
            _webview.RegisterCallback("changeSiteCallback", (args) => {
                CrossSettings.Current.Remove("userguid");
                CrossSettings.Current.Remove("url");
                CrossSettings.Current.Remove("theme");

                SelectSite();
            });

            //Callback for badge change
            _webview.RegisterCallback("badgeCallback", (args) => {
                //Set badge
                var badge = int.Parse(args);

                Task.Run(() => {
                    DependencyService.Get<IApplicationBadge>().SetBadge(badge);
                });
            });

            // callback for theming
            _webview.RegisterCallback("themeCallback", (color) => {
                var themeColor = CrossSettings.Current.Get<string>("themecolor");

                // update theme color if different
                if (color != themeColor) {
                    SetThemeColor(color);
                }
            });

            //Callback from external links script
            _webview.RegisterCallback("handleLinksCallback", (args) => {
                Device.OpenUri(new Uri(args));
            });
        }

        /// <summary>
        /// Resolve the next url to display
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string ResolveUrl(string url) {

            if (string.IsNullOrEmpty(url)) {
                url = CrossSettings.Current.Get<string>("url");
            }

            return url;
        }

        /// <summary>
        /// Get the font name
        /// </summary>
        /// <returns></returns>
        private string GetFont() {
            string font;

            switch (Device.RuntimePlatform) {
                case Device.iOS:
                    font = "SegoeUI-Light";
                    break;
                default:
                    font = "segoeuil.ttf#segoeuil";
                    break;
            }

            return font;
        }

        #endregion

    }
}
