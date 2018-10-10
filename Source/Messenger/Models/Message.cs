using System.Runtime.Serialization;

namespace Messenger.Models {
    /// <summary>
    /// Represents a message passed from a web page to the app via javascript
    /// </summary>
    [DataContract]
    public class Message {
        /// <summary>
        /// The action to call
        /// </summary>
        [DataMember(Name = "a")]
        public string Action { get; set; }

        /// <summary>
        /// Arguments
        /// </summary>
        [DataMember(Name = "d")]
        public object Data { get; set; }

        /// <summary>
        /// The callback 
        /// </summary>
        [DataMember(Name = "c")]
        public string Callback { get; set; }
    }
}
