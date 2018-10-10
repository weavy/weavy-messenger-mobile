using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Messenger.Models;
using Xamarin.Forms;
using XLabs.Serialization;

namespace Messenger.Views {

    /// <summary>
    /// A custom implementation of a web view used in each platform projects
    /// This allows us to implement platform specific behaviour such as script injection
    /// </summary>
    public class HybridWebView : View {
        private readonly object injectLock = new object();
        private readonly Dictionary<string, Action<string>> registeredActions;
        private readonly Dictionary<string, Func<string, object[]>> registeredFunctions;        
        private readonly IJsonSerializer jsonSerializer;

        public EventHandler LoadFinished;
        public EventHandler IsLoading;
        public EventHandler LoadError;
        public EventHandler<string> JavaScriptLoadRequested;

        public event EventHandler GoBackRequested;
        public event EventHandler GoForwardRequested;

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
          propertyName: "Uri",
          returnType: typeof(string),
          declaringType: typeof(HybridWebView),
          defaultValue: default(string));

        public static readonly BindableProperty CanGoBackProperty = BindableProperty.Create(
          propertyName: "CanGoBack",
          returnType: typeof(bool),
          declaringType: typeof(HybridWebView),
          defaultValue: default(bool));

        public static readonly BindableProperty CanGoForwardProperty = BindableProperty.Create(
          propertyName: "CanGoForward",
          returnType: typeof(bool),
          declaringType: typeof(HybridWebView),
          defaultValue: default(bool));


        public HybridWebView(IJsonSerializer serializer) {
            jsonSerializer = serializer;
            registeredActions = new Dictionary<string, Action<string>>();
            registeredFunctions = new Dictionary<string, Func<string, object[]>>();
        }

        #region props
        public string Uri {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public bool CanGoBack {
            get { return (bool)GetValue(CanGoBackProperty); }
            set { SetValue(CanGoBackProperty, value); }
        }

        public bool CanGoForward {
            get { return (bool)GetValue(CanGoForwardProperty); }
            set { SetValue(CanGoForwardProperty, value); }
        }
        #endregion

        #region events
        public void OnLoadFinished(object sender, EventArgs e) {
            var handler = this.LoadFinished;
            if (handler != null) {
                handler(this, e);
            }
        }

        public void OnLoading(object sender, EventArgs e) {
            var handler = this.IsLoading;
            if (handler != null) {
                handler(this, e);
            }
        }

        public void OnLoadError(object sender, EventArgs e) {
            var handler = this.LoadError;
            if (handler != null) {
                handler(this, e);
            }
        }

        public void GoBack() {
            var handler = this.GoBackRequested;
            if (handler != null) {
                handler(this, null);
            }
        }

        public void GoForward() {
            var handler = this.GoForwardRequested;
            if (handler != null) {
                handler(this, null);
            }
        }
        #endregion

        public void InjectJavaScript(string script) {
            lock (injectLock) {                
                JavaScriptLoadRequested?.Invoke(this, script);                                
            }
        }

        public void RegisterCallback(string name, Action<string> callback) {
            registeredActions.Add(name, callback);
        }

        public void RegisterNativeFunction(string name, Func<string, object[]> func) {
            this.registeredFunctions.Add(name, func);
        }

        internal bool TryGetAction(string name, out Action<string> action) {
            return this.registeredActions.TryGetValue(name, out action);
        }

        internal bool TryGetFunc(string name, out Func<string, object[]> func) {
            return this.registeredFunctions.TryGetValue(name, out func);
        }


        /// <summary>
        /// A message is received from the web page. Try call the C# method or function
        /// </summary>
        /// <param name="message"></param>
        public void MessageReceived(string message) {
            var m = jsonSerializer.Deserialize<Message>(message);

            if (m == null || m.Action == null) return;
            
            if (TryGetAction(m.Action, out Action<string> action)) {
                action.Invoke(m.Data.ToString());
                return;
            }

            if (TryGetFunc(m.Action, out Func<string, object[]> func)) {
                Task.Run(() => {
                    var result = func.Invoke(m.Data.ToString());
                    CallJsFunction(string.Format("NativeFuncs[{0}]", m.Callback), result);
                });
            }
        }

        /// <summary>
        /// Call a javascript function in the web page
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="parameters"></param>
        public void CallJsFunction(string funcName, params object[] parameters) {
            var builder = new StringBuilder();

            builder.Append(funcName);
            builder.Append("(");

            for (var n = 0; n < parameters.Length; n++) {
                builder.Append(this.jsonSerializer.Serialize(parameters[n]));
                if (n < parameters.Length - 1) {
                    builder.Append(", ");
                }
            }

            builder.Append(");");

            InjectJavaScript(builder.ToString());
        }

        public void Cleanup() {
            // clean up
        }
    }
}
