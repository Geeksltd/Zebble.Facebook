﻿namespace Zebble
{
    using SDK = global::Facebook;
    using Foundation;
    using System.Threading.Tasks;
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using UIKit;

    public partial class Facebook
    {
        static bool IsLoginCall = false;
        static readonly AsyncEvent<JObject> UserInfoFetched = new AsyncEvent<JObject>();
        static SDK.LoginKit.LoginManager LoginManager;
        static string[] CurrentParameters;

        public static User CurrentUser;

        public readonly static AsyncEvent OnCancel = new AsyncEvent();
        public readonly static AsyncEvent<string> OnError = new AsyncEvent<string>();
        public readonly static AsyncEvent<User> OnSuccess = new AsyncEvent<User>();

        public static int ProfileImageWidth = 200, ProfileImageHeight = 200;

        public static Task Login(params Field[] requestedFiels)
        {
            IsLoginCall = true;
            CurrentParameters = GetRequredPermissions(requestedFiels);
            LoginManager = new SDK.LoginKit.LoginManager();
            LoginManager.LogInWithReadPermissions(CurrentParameters, UIViewController,
                new SDK.LoginKit.LoginManagerRequestTokenHandler(RequestTokenHandler));

            return Task.CompletedTask;
        }

        public static async Task GetInfo(Field[] fields, Action<JObject> onCompleted)
        {
            IsLoginCall = false;

            var accessToken = await RefreshAccessToken();

            var param = NSDictionary.FromObjectAndKey(CurrentParameters.ToString(",").ToNs(), "fields".ToNs());
            var request = new SDK.CoreKit.GraphRequest("me", param, accessToken.TokenString, null, "GET");
            request.Start(new SDK.CoreKit.GraphRequestHandler(GraphCallback));

            UserInfoFetched.ClearHandlers();
            UserInfoFetched.Handle(user => onCompleted(user));
        }

        public static Task LogOut()
        {
            LoginManager.LogOut();
            IsLoginCall = false;
            CurrentUser = null;
            return Task.CompletedTask;
        }

        public static void FinishedLaunching(UIApplication application, NSDictionary options)
        {
            SDK.CoreKit.ApplicationDelegate.SharedInstance.FinishedLaunching(application, options);
        }

        public static void OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
        {
            SDK.CoreKit.ApplicationDelegate.SharedInstance.OpenUrl(application, url, options);
        }

        static async void RequestTokenHandler(SDK.LoginKit.LoginManagerLoginResult result, NSError error)
        {
            if (error != null)
            {
                Device.Log.Error("An error occured in Facebook :[" + error + "]");
                await OnError.Raise(error.ToString());
            }
            else if (result.IsCancelled)
            {
                await OnCancel.Raise();
            }
            else
            {
                var accessToken = result.Token;
                if (accessToken.IsExpired) accessToken = await RefreshAccessToken();
                
                var param = NSDictionary.FromObjectAndKey(CurrentParameters.ToString(",").ToNs(), "fields".ToNs());
                var request = new SDK.CoreKit.GraphRequest("me", param, accessToken.TokenString, null, "GET");
                request.Start(new SDK.CoreKit.GraphRequestHandler(GraphCallback));
            }
        }

        static async void GraphCallback(SDK.CoreKit.GraphRequestConnection connection, NSObject result, NSError error)
        {
            if (error == null)
            {
                var data = JsonConvert.DeserializeObject<JObject>(result.ToString());
                if (IsLoginCall)
                {
                    CurrentUser = new User(data);
                    await OnSuccess.Raise(CurrentUser);
                }
                else
                    await UserInfoFetched.Raise(data);
            }
            else
                await OnSuccess.Raise(null);
        }

        static async Task<SDK.CoreKit.AccessToken> RefreshAccessToken()
        {
            var accessToken = SDK.CoreKit.AccessToken.CurrentAccessToken;
            if (accessToken.IsExpired)
            {
                var token = await SDK.CoreKit.AccessToken.RefreshCurrentAccessTokenAsync();
                accessToken = token.Result as SDK.CoreKit.AccessToken;
            }

            return accessToken;
        }
    }
}