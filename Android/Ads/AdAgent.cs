using System;
using ads = Xamarin.Facebook.Ads;
using Olive;

namespace Zebble.FacebookAds
{
    partial class AdAgent
    {
        ads.NativeAd NativeAd;

        public AdAgent(string placementId)
        {
            PlacementId = placementId;
            Log.For(this).Debug($"Facebook ads test device Id:{Ads.GetTestDeviceId()}");
            Initialize();
        }

        public void Initialize()
        {
            if (NativeAd != null) throw new InvalidOperationException("AdAgent.Initialize() should only be called once.");

            NativeAd = new ads.NativeAd(Renderer.Context, PlacementId);
            NativeAd.SetAdListener(new NativeAdListener(this));
        }

        void RequestNativeAd() => NativeAd.LoadAd();
        
        public Android.Views.View Render(ads.NativeAdViewAttributes attrs) => ads.NativeAdView.Render(Renderer.Context, NativeAd, attrs);

        class NativeAdListener : Java.Lang.Object, ads.INativeAdListener
        {
            AdAgent Agent;

            public NativeAdListener(AdAgent agent) => Agent = agent;

            public void OnAdClicked(ads.IAd p0) { }

            public void OnAdLoaded(ads.IAd p0) => Agent.OnNativeAdReady(new NativeAdInfo(p0 as ads.NativeAd));

            public void OnError(ads.IAd p0, ads.AdError p1) => Agent.OnAdFailedToLoad("Ad Loading Failed");

            public void OnLoggingImpression(ads.IAd p0) { }

            public void OnMediaDownloaded(ads.IAd p0) { }
        }
    }
}
