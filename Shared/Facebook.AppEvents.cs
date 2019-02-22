using System;

namespace Zebble
{
    public partial class Facebook
    {
        public enum EventNames
        {
            AchieveLevel,
            InAppAdClick,
            AddPaymentInfo,
            AddtoCart,
            AddtoWishlist,
            CompleteRegistration,
            CompleteTutorial,
            Contact,
            CustomizeProduct,
            Donate,
            FindLocation,
            InitiateCheckout,
            Rate,
            Schedule,
            SpentCredits,
            StartTrial,
            SubmitApplication,
            Subscription,
            UnlockAchievement,
            ViewContent,
        }

        public enum ParameterNames
        {
            Level,
            AdType,
            Success,
            ContentType,
            Currency,
            ContentID,
            Content,
            RegistrationMethod,
            NumItems,
            PaymentInfoAvailable,
            MaxRatingValue,
            SearchString,
            OrderID,
            Description
        }
    }
}
