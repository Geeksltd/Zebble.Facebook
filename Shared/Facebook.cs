namespace Zebble
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static partial class Facebook
    {
        /// <summary>
        /// It allows you to customize its visual style.
        /// </summary>
        public static FacebookDialog CurrentDialog { get; set; }

        static string ClientId => Config.Get("Facebook.App.Id");

        static string GetLoginUrl(string requestedPermissions)
        {
            var returnUrl = ("https://" + "www.facebook.com/connect/login_success.html").UrlEncode();

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

            var url = GetLoginUrl(requestedPermissions);

            if (CurrentDialog == null) CurrentDialog = new FacebookDialog { IsDefualtDialog = true };

            CurrentDialog.RequestedUrl = url;
            CurrentDialog.Canceled.Handle(() =>
            {
                CurrentDialog.RemoveSelf();
                CurrentDialog = null;
                task.SetResult(null);
            });
            CurrentDialog.Succeeded.Handle(token => task.SetResult(token));

            await CurrentDialog.ShowDialog();

            return await task.Task;
        }

        public static async Task<User> GetUserInfo(string accessToken, Field[] fields)
        {
            var url = "https://" + "graph.facebook.com/me?" +
                "fields=" + fields.Select(x => x.ToString().ToLower()).ToString(",") +
                "&access_token=" + accessToken;

            return await Thread.Pool.Run(async () =>
            {
                using (var client = new HttpClient())
                {
                    var json = await client.GetStringAsync(url);
                    var data = JsonConvert.DeserializeObject<JObject>(json);
                    return new User(data) { AccessToken = accessToken };
                }
            });
        }

    }

    public class FacebookDialog : Stack
    {
        internal string RequestedUrl;
        internal bool IsDefualtDialog;

        internal readonly AsyncEvent<string> Succeeded = new AsyncEvent<string>();
        internal readonly AsyncEvent Canceled = new AsyncEvent();

        public readonly Stack Container = new Stack { Id = "FacebookContainer" };

        public override async Task OnInitializing()
        {
            await base.OnInitializing();

            if (IsDefualtDialog)
            {
                await InitializeDefaultDialog();
                await InitializeFacebook();
            }
            else await InitializeFacebook();

            Container.ZIndex(1000);
            await Add(Container);
        }

        public Task ShowDialog() => View.Root.Add(this);

        public Task CloseDialog()
        {
            this.Visible(value: false);
            return Canceled.Raise();
        }

        async Task InitializeDefaultDialog()
        {
            Container.Width.BindTo(View.Root.Width, x => x * 0.8f);
            Container.Height.BindTo(View.Root.Height, x => x * 0.8f);

            Container.CenterAlign(delayUntilRender: true)
                     .MiddleAlign(delayUntilRender: true)
                     .Background(color: Colors.White)
                     .Border(1, color: "#4267b2", radius: 5);

            await Container.Add(new Button().Text("Cancel").Background(color: Colors.Black).TextColor(Colors.White).On(x => x.Tapped, () => CloseDialog()));
        }

        async Task InitializeFacebook()
        {
            var browser = new WebView(RequestedUrl).Size(100.Percent());

            browser.BrowserNavigated.Handle(() =>
            {
                var uri = browser.Url.AsUri();

                if (uri.AbsolutePath.EndsWith("/login_success.html"))
                {
                    var token = uri.Fragment.OrEmpty().TrimStart("#").Split('&')
                    .FirstOrDefault(x => x.StartsWith("access_token="))?.TrimStart("access_token=");

                    Succeeded.Raise(token);
                    this.Visible(value: false);
                    return Task.CompletedTask;
                }
                else
                    return Task.CompletedTask;
            });

            await Container.Add(browser);
        }
    }
}