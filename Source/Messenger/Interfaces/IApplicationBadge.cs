namespace Messenger.Interfaces {

    /// <summary>
    /// An interface for handling app badges. The badge count is passed from the web page to the app through a callback. See MainPage.cs for more info.
    /// </summary>
    public interface IApplicationBadge {
        void SetBadge(int number);
    }
}
