using System.Threading.Tasks;

namespace Messenger.Notifications {
    /// <summary>
    /// Interface for handling hub notification registrations. 
    /// This is implemented in each platform project
    /// </summary>
    public interface INotificationService {

        /// <summary>
        /// Register with the Azure Notification Hub
        /// </summary>
        /// <param name="userGuid">The guid of the user to register. The guid is passed from the web page to the app through a callback. See MainPage.cs for more info</param>
        void Register(string userGuid);

    }
}
