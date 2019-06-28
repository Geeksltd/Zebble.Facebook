namespace Zebble
{
    using Android.Content;
    using Android.OS;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Org.Json;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using SDK = Xamarin.Facebook;

    public partial class Facebook
    {
        static bool IsLoginCall = false;
        static string[] CurrentParameters;
        static SDK.ICallbackManager CallbackManager;
        static readonly AsyncEvent<JObject> UserInfoFetched = new AsyncEvent<JObject>();
        static SDK.Login.LoginManager LoginManager => SDK.Login.LoginManager.Instance;

        public static User CurrentUser;
        public readonly static AsyncEvent OnCancel = new AsyncEvent();
        public readonly static AsyncEvent<string> OnError = new AsyncEvent<string>();
        public readonly static AsyncEvent<User> OnSuccess = new AsyncEvent<User>();

        public static int ProfileImageWidth = 200, ProfileImageHeight = 200;

        public static async Task Login(params Field[] requestedFiels)
        {
            IsLoginCall = true;
            CurrentParameters = await SetRequirments(requestedFiels);
            LoginManager.LogInWithReadPermissions(UIRuntime.CurrentActivity, CurrentParameters);
        }

        public static Task LogOut()
        {
            LoginManager.LogOut();
            CurrentUser = null;
            IsLoginCall = false;

            return Task.CompletedTask;
        }

        public static async Task GetInfo(Field[] fields, Action<JObject> onCompleted)
        {
            IsLoginCall = false;
            GetGraphData(SDK.AccessToken.CurrentAccessToken);
            UserInfoFetched.ClearHandlers();
            UserInfoFetched.Handle(user => onCompleted(user));
        }

        public static void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            CallbackManager.OnActivityResult(requestCode, resultCode, data);
        }

        static Task<string[]> SetRequirments(Field[] fiels)
        {
            CallbackManager = SDK.CallbackManagerFactory.Create();
            LoginManager.RegisterCallback(CallbackManager, new FacebookCallback());
            return Task.FromResult(GetRequredPermissions(fiels));
        }

        static void GetGraphData(SDK.AccessToken accessToken)
        {
            if (accessToken.IsExpired) SDK.AccessToken.RefreshCurrentAccessTokenAsync(new AccessTokeCallback());
            else ExecuteGraphRequest(accessToken);
        }

        static void ExecuteGraphRequest(SDK.AccessToken accessToken)
        {
            var request = SDK.GraphRequest.NewMeRequest(accessToken, new GraphCallback());
            var bundle = new Bundle();
            bundle.PutString("fields", CurrentParameters.ToString(","));
            request.Parameters = bundle;
            request.ExecuteAsync();
        }

        class FacebookCallback : Java.Lang.Object, SDK.IFacebookCallback
        {
            public void OnCancel() => Facebook.OnCancel.Raise();

            public void OnError(SDK.FacebookException error)
            {
                Device.Log.Error("An error occured in Facebook :[" + error + "]");
                Facebook.OnError.Raise(error.ToString());
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                var loginResult = result as SDK.Login.LoginResult;
                if (loginResult != null) GetGraphData(loginResult.AccessToken);
                else Facebook.OnSuccess.Raise(null);
            }
        }

        class AccessTokeCallback : Java.Lang.Object, SDK.AccessToken.IAccessTokenRefreshCallback
        {
            public void OnTokenRefreshed(SDK.AccessToken accessToken)
            {
                SDK.GraphRequest.NewMeRequest(accessToken, new GraphCallback());
            }

            public void OnTokenRefreshFailed(SDK.FacebookException exception)
            {
                Device.Log.Error($"An error occured in Facebook access token : [{exception}]");
            }
        }

        class GraphCallback : Java.Lang.Object, SDK.GraphRequest.IGraphJSONObjectCallback
        {
            public void OnCompleted(JSONObject @object, SDK.GraphResponse response)
            {
                var data = JsonConvert.DeserializeObject<JObject>(@object.ToString());
                if (IsLoginCall)
                {
                    var token = SDK.AccessToken.CurrentAccessToken;
                    CurrentUser = new User(data)
                    {
                        AccessToken = new AccessToken
                        {
                            TokenString = token.Token,
                            AppId = token.ApplicationId,
                            UserId = token.UserId,
                            Permissions = token.Permissions.ToArray(),
                            DeclinedPermissions = token.DeclinedPermissions.ToArray(),
                            DataAccessExpirationDate = AccessToken.FromDate(token.DataAccessExpirationTime),
                            ExpirationDate = AccessToken.FromDate(token.Expires),
                            RefreshDate = AccessToken.FromDate(token.LastRefresh)
                        }
                    };
                    OnSuccess.Raise(CurrentUser);
                }
                else
                    UserInfoFetched.Raise(data);
            }
        }
    }
}