using Acr.Settings;
using System.Text.RegularExpressions;
using Messenger.Helpers;
using Xamarin.Forms;

namespace Messenger {

    /// <summary>
    /// This is the page that allows the user to specify an url to the Weavy site
    /// </summary>
    public class SelectSitePage: ContentPage {

        private readonly Button _button;
        private readonly ActivityIndicator _activity;

        /// <summary>
        /// Constructor
        /// </summary>
        public SelectSitePage() {

            // the logo to show
            var logo = new Image() {
                Source = ImageSource.FromFile(GetLogo()),
                Aspect = Aspect.AspectFit,
                WidthRequest = 100,
                HeightRequest = 100
            };

            // the label. Change to whatever you want
            var label = new Label() {
                Text = $"Enter your {Constants.DisplayShortName} domain",
                FontSize = 20,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.FromHex("#ffffff"),
                FontFamily = GetFont()
            };

            // the input field
            var entry = new Entry() {
                TextColor = Color.FromHex("#555555"),
                Placeholder = "Weavy cloud domain or custom url",
                Text = "",
                BackgroundColor = Color.White,
                Keyboard = Keyboard.Url,
                FontFamily = GetFont()
            };

            // the button
            _button = new Button() {
                Text = "Continue",
                BackgroundColor = Color.FromHex("#1A85B7"),
                TextColor = Color.White,
                FontSize = 15,
                FontFamily = GetFont()
            };

            // this is just a helper label displaying what the user has entered
            var urlLabel = new Label {
                Text = ResolveUrl(entry.Text),
                FontSize = 14,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.FromHex("#ffffff"),
                FontFamily = GetFont(),
                Margin = GetLabelMargin()
            };

            // an activity indicator shown on submit
            _activity = new ActivityIndicator() {
                IsRunning = true,
                IsVisible = false
            };

            // the layout
            var layout = new StackLayout {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Spacing = 14,
                BackgroundColor = Color.FromHex("#1f9cd6"),
                Padding = GetLayoutPadding(),
                Children = { logo, label, entry, urlLabel, _button, _activity }
            };

            // handle url entry changes
            entry.TextChanged += (sender, args) => {
                urlLabel.Text = ResolveUrl(entry.Text);
            };

            // handle button click
            _button.Clicked += async (sender, args) => {
                var networkHelper = new NetworkHelper();
                var url = ResolveUrl(entry.Text);

                ToggleActivity(true);

                if (networkHelper.Connected()) {

                    url = EnsureHttps(url);
                    
                    // check reachability of entered url
                    var reachable = await networkHelper.IsReachable(url);

                    if (reachable) {
                        ToggleActivity(false);

                        //Store url in settings
                        CrossSettings.Current.Set<string>("url", $"{url}/messenger");

                        //Show page
                        Application.Current.MainPage = new MainPage();
                    } else {
                        ToggleActivity(false);
                        await DisplayAlert("Error", "Something went wrong. Please make sure you entered a correct url.", "OK");
                    }

                } else {
                    ToggleActivity(false);
                    await DisplayAlert("Connection error", "You need an internet connection to use this app.", "OK");
                }
            };

            // set the content
            Content = layout;
        }

        /// <summary>
        /// Ensure that the uri is https
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string EnsureHttps(string url) {
            
            if (Regex.Match(url, "https?://").Length == 0) {
                url = $"https://{url}";
            }

            return url;
        }


        #region private

        /// <summary>
        /// Gets the current url based on the entry text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ResolveUrl(string text) {
            if (string.IsNullOrEmpty(text)) {
                return "";
            } else if (Regex.Match(text.ToLower(), "https?:").Length != 0) {
                // custom url
                return text.ToLower();
            } else {
                // azure url
                return $"https://{text.ToLower()}.{Constants.AzureSuffix}";
            }
        }

        /// <summary>
        /// Get the logo
        /// </summary>
        /// <returns></returns>
        private string GetLogo() {
            string logo;

            switch (Device.RuntimePlatform) {
                case Device.iOS:
                    logo = "icon.png";
                    break;
                default:
                    logo = "ic_launcher_round.png";
                    break;
            }

            return logo;
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

        /// <summary>
        /// Get the layout padding
        /// </summary>
        /// <returns></returns>
        private Thickness GetLayoutPadding() {
            double topPadding;

            switch (Device.RuntimePlatform) {
                case Device.iOS:
                    topPadding = 77;
                    break;
                default:
                    topPadding = 47;
                    break;

            }
            return new Thickness(20, topPadding, 20, 20);
        }

        /// <summary>
        /// Get url label margins
        /// </summary>
        /// <returns></returns>
        private Thickness GetLabelMargin() {
            double margin;

            switch (Device.RuntimePlatform) {
                case Device.iOS:
                    margin = 0;
                    break;
                default:
                    margin = -10;
                    break;

            }
            return new Thickness(margin);
        }

        /// <summary>
        /// Toggle activity indicator and button
        /// </summary>
        /// <param name="busy"></param>
        private void ToggleActivity(bool busy) {
            _activity.IsVisible = busy;
            _button.IsEnabled = !busy;
        }

        #endregion
    }
}
