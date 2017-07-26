namespace Zebble
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static partial class Facebook
    {
        static View CurrentDialog;

        /// <summary>
        ///  Invoked when a login dialog is created. It allows you to customise its visual style.
        /// </summary>
        public static Action<View, Plugin.WebView> CustomizeDialog;

        static string ClientId => Config.Get("Facebook.App.Id");

        static string GetLoginUrl(string requestedPermissions)
        {
            var returnUrl = ("http://" + "www.facebook.com/connect/login_success.html").UrlEncode();

            return $"https://graph.facebook.com/oauth/authorize" +
                $"?client_id={ClientId}" +
                $"&redirect_uri={returnUrl}" +
                $"&scope={requestedPermissions.UrlEncode()}" +
                $"&type=user_agent&display=popup";
        }

        /// <summary>
        /// Opens a facebook login dialog to get the user to sign in. It will then retrieve the user's data.
        /// If login was not successful then it will return null.
        /// Full list of fields: https://developers.facebook.com/docs/graph-api/reference/user
        /// </summary>
        public static async Task<User> Register(params Field[] fields)
        {
            var token = await Login(GetRequredPermissions(fields).ToString(","));
            if (token.LacksValue()) return null;

            return await GetUserInfo(token, fields);
        }

        public static Task<string> Login(params Field[] requestedFields)
        {
            return Login(GetRequredPermissions(requestedFields).ToString(","));
        }

        /// <summary>
        /// Opens a login dialog and returns the access token if it was successful, otherwise null.
        /// </summary> 
        /// <param name="permissions">The permissions to get from the user, so the access token can be used to retrieve such data later on.
        /// See the full list of permissions here: https://developers.facebook.com/docs/facebook-login/permissions/.
        /// Use GetRequredPermissions(...) to get the list.</param>
        public static async Task<string> Login(string requestedPermissions)
        {
            var task = new TaskCompletionSource<string>();

            var browser = await OpenDialog(GetLoginUrl(requestedPermissions), task);

            browser.BrowserNavigated.Handle(() =>
            {
                var uri = browser.Url.AsUri();

                if (uri.AbsolutePath.EndsWith("/login_success.html"))
                {
                    var token = uri.Fragment.OrEmpty().TrimStart("#").Split('&')
                    .FirstOrDefault(x => x.StartsWith("access_token="))?.TrimStart("access_token=");

                    task.SetResult(token);
                    CloseDialog();
                    return Task.CompletedTask;
                }
                else
                    return Task.CompletedTask;
            });

            return await task.Task;
        }

        public static async Task<User> GetUserInfo(string accessToken, Field[] fields)
        {
            var url = "https://" + "graph.facebook.com/me?" +
                "fields=" + fields.Select(x => x.ToString().ToLower()).ToString(",") +
                "&access_token=" + accessToken;

            return await Device.ThreadPool.Run(async () =>
            {
                using (var client = new HttpClient())
                {
                    var json = await client.GetStringAsync(url);
                    var data = JsonConvert.DeserializeObject<JObject>(json);
                    return new User(data) { AccessToken = accessToken };
                }
            });
        }

        static void CloseDialog()
        {
            if (CurrentDialog == null) return;
            CurrentDialog.Visible = false;
            CurrentDialog = null;
        }

        static async Task<Plugin.WebView> OpenDialog(string url, TaskCompletionSource<string> task)
        {
            // TODO: Add a cancel button too.
            CurrentDialog = new Stack();
            CurrentDialog.Width.BindTo(View.Root.Width, x => x * 0.8f);
            CurrentDialog.Height.BindTo(View.Root.Height, x => x * 0.8f);
            CurrentDialog.ZIndex(1000);

            CurrentDialog.CenterAlign(delayUntilRender: true).MiddleAlign(delayUntilRender: true)
                .Background(color: Colors.White).Border(1, color: "#4267b2", radius: 5);

            await View.Root.Add(CurrentDialog);

            await CurrentDialog.Add(new Button().Text("Cancel").Background(color: Colors.Black)
                 .On(x => x.Tapped, () => { task.SetResult(null); CloseDialog(); }));

            var browser = new Plugin.WebView(url).Size(100.Percent());
            await CurrentDialog.Add(browser);
            CustomizeDialog?.Invoke(CurrentDialog, browser);

            return browser;
        }
    }
}