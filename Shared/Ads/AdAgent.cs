using System;
using System.Threading.Tasks;

namespace Zebble.FacebookAds
{
    public partial class AdAgent
    {
        public string PlacementId { get; set; }

        TaskCompletionSource<NativeAdInfo> NextNativeAd;

        public void OnAdFailedToLoad(string reason) => NextNativeAd?.TrySetResult(FailedNativeAdInfo.Create(reason));

        public void OnNativeAdReady(NativeAdInfo ad) => NextNativeAd?.TrySetResult(ad);

        public Task<NativeAdInfo> GetNativeAd()
        {
            NextNativeAd = new TaskCompletionSource<NativeAdInfo>();
            RequestNativeAd();
            return NextNativeAd.Task;
        }
    }

    public class FailedNativeAdInfo : NativeAdInfo
    {
        static Func<string, FailedNativeAdInfo> CustomProvider;
        public virtual string ImageUrl { get; set; }
        public virtual string TargetUrl { get; set; }

        public static void OnRequested(Func<string, FailedNativeAdInfo> provider) => CustomProvider = provider;

        internal static FailedNativeAdInfo Create(string reason)
        {
            return CustomProvider?.Invoke(reason) ?? new

            FailedNativeAdInfo
            {
                Headline = "Ad loading failed",
                Body = reason,
                CallToAction = "Try again later",
            };
        }
    }
}