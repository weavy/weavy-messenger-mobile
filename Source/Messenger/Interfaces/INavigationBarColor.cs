namespace Messenger.Interfaces {

    /// <summary>
    /// An interface for setting the status bar background. The color is passed from the web page to the app through a callback. See MainPage.cs for more info.
    /// </summary>
    public interface INavigationBarColor {
        void SetStatusBarColor(string hexColor);
    }
}
