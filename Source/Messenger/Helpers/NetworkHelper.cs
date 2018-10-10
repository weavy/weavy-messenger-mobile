using RestSharp.Portable;
using RestSharp.Portable.HttpClient;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Messenger.Helpers {

    /// <summary>
    /// Network helpers
    /// </summary>
    public class NetworkHelper {

        /// <summary>
        /// Determines if connected to internet
        /// </summary>
        /// <returns><c>true</c> if connected, otherwise <c>false</c></returns>
        public bool Connected() {
            return Connectivity.NetworkAccess == NetworkAccess.Internet;
        }


        /// <summary>
        /// Check if the Weavy site is reachable
        /// </summary>
        /// <param name="uri"></param>
        /// <returns><c>true</c> if the site is reachable</returns>
        public async Task<bool> IsReachable(string uri) {

            const string resource = "/api/status";

            var client = new RestClient() { BaseUrl = new Uri(uri) };

            try {
                var response = await client.Execute<string>(new RestRequest(resource));
                return response.IsSuccess;
            } catch (Exception) {
                return false;
            }
        }
    }
}
