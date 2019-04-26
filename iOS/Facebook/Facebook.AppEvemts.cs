﻿namespace Zebble
{
    using Foundation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SDK = global::Facebook.CoreKit;

    public partial class Facebook
    {
        public static void FacebookAutoLogAppEventsEnabled(bool enable) => SDK.Settings.AutoLogAppEventsEnabled = enable;

        public static void EnableUpdatesOnAccessTokenChange(bool enable) => SDK.Profile.EnableUpdatesOnAccessTokenChange(enable: enable);

        public static void SetAdvertiserIDCollectionEnabled(bool enable) => SDK.Settings.AdvertiserIdCollectionEnabled = enable;

        public static void ActivateAppEvents() => SDK.AppEvents.ActivateApp();

        public static void CallEvent(EventNames eventName, Dictionary<ParameterNames, object> @params, double? valueToSum = null)
        {
            var keys = @params.Select(x => x.Key.ToParameterName()).ToArray();
            var values = @params.Select(x => NSObject.FromObject(x.Value)).ToArray();

            var param = NSDictionary.FromObjectsAndKeys(values, keys);
            if (valueToSum.HasValue)
                SDK.AppEvents.LogEvent(eventName.ToEventName(), valueToSum.Value, param);
            else
                SDK.AppEvents.LogEvent(eventName.ToEventName(), param);
        }

        public static void CallEvent(string eventName, Dictionary<string, object> @params, double? valueToSum = null)
        {
            var keys = @params.Select(x => x.Key.ToNs()).ToArray();
            var values = @params.Select(x => x.Value).ToArray();

            var param = NSDictionary.FromObjectsAndKeys(values, keys);
            if (valueToSum.HasValue)
                SDK.AppEvents.LogEvent(eventName, valueToSum.Value, param);
            else
                SDK.AppEvents.LogEvent(eventName, param);
        }

        public static void CallLogPurchase(double purchaseAmount, string currency, Dictionary<string, object> @params = null, AccessToken accessToken = null)
        {
            if (@params != null)
            {
                var keys = @params.Select(x => x.Key.ToNs()).ToArray();
                var values = @params.Select(x => x.Value).ToArray();
                var param = NSDictionary.FromObjectsAndKeys(values, keys);

                if (accessToken == null) SDK.AppEvents.LogPurchase(purchaseAmount, currency, param);
                else SDK.AppEvents.LogPurchase(purchaseAmount, currency, param, accessToken.ToSDKAccessToken());
            }
            else
                SDK.AppEvents.LogPurchase(purchaseAmount, currency);
        }

        static NSString ToEventName(this EventNames eventname)
        {
            switch (eventname)
            {
                case EventNames.AchieveLevel:
                    return SDK.AppEventName.AchievedLevel;
                case EventNames.InAppAdClick:
                    return SDK.AppEventName.AdClick;
                case EventNames.AddPaymentInfo:
                    return SDK.AppEventName.AddedPaymentInfo;
                case EventNames.AddtoCart:
                    return SDK.AppEventName.AddedToCart;
                case EventNames.AddtoWishlist:
                    return SDK.AppEventName.AddedToWishlist;
                case EventNames.CompleteRegistration:
                    return SDK.AppEventName.CompletedRegistration;
                case EventNames.CompleteTutorial:
                    return SDK.AppEventName.CompletedTutorial;
                case EventNames.Contact:
                    return SDK.AppEventName.Contact;
                case EventNames.CustomizeProduct:
                    return SDK.AppEventName.CustomizeProduct;
                case EventNames.Donate:
                    return SDK.AppEventName.Donate;
                case EventNames.FindLocation:
                    return SDK.AppEventName.FindLocation;
                case EventNames.InitiateCheckout:
                    return SDK.AppEventName.InitiatedCheckout;
                case EventNames.Rate:
                    return SDK.AppEventName.Rated;
                case EventNames.Schedule:
                    return SDK.AppEventName.Schedule;
                case EventNames.SpentCredits:
                    return SDK.AppEventName.SpentCredits;
                case EventNames.StartTrial:
                    return SDK.AppEventName.StartTrial;
                case EventNames.SubmitApplication:
                    return SDK.AppEventName.SubmitApplication;
                case EventNames.Subscription:
                    return SDK.AppEventName.Subscribe;
                case EventNames.UnlockAchievement:
                    return SDK.AppEventName.UnlockedAchievement;
                case EventNames.ViewContent:
                    return SDK.AppEventName.ViewedContent;
                default:
                    return null;
            }
        }

        static NSString ToParameterName(this ParameterNames paramName)
        {
            switch (paramName)
            {
                case ParameterNames.Level:
                    return SDK.AppEventParameterName.Level;
                case ParameterNames.AdType:
                    return SDK.AppEventParameterName.AdType;
                case ParameterNames.Success:
                    return SDK.AppEventParameterName.Success;
                case ParameterNames.ContentType:
                    return SDK.AppEventParameterName.ContentType;
                case ParameterNames.Currency:
                    return SDK.AppEventParameterName.Currency;
                case ParameterNames.ContentID:
                    return SDK.AppEventParameterName.ContentID;
                case ParameterNames.Content:
                    return SDK.AppEventParameterName.Content;
                case ParameterNames.RegistrationMethod:
                    return SDK.AppEventParameterName.RegistrationMethod;
                case ParameterNames.NumItems:
                    return SDK.AppEventParameterName.NumItems;
                case ParameterNames.PaymentInfoAvailable:
                    return SDK.AppEventParameterName.PaymentInfoAvailable;
                case ParameterNames.MaxRatingValue:
                    return SDK.AppEventParameterName.MaxRatingValue;
                case ParameterNames.SearchString:
                    return SDK.AppEventParameterName.SearchString;
                case ParameterNames.OrderID:
                    return SDK.AppEventParameterName.OrderId;
                case ParameterNames.Description:
                    return SDK.AppEventParameterName.Description;
                default:
                    return null;
            }
        }
    }
}