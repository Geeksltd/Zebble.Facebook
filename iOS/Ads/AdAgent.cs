using Foundation;
using System;
using UIKit;
using ads = Facebook.AudienceNetwork;

namespace Zebble.FacebookAds
{
    partial class AdAgent
    {
        ads.NativeAd NativeAd;

        public AdAgent(string placementId)
        {
            PlacementId = placementId;
            Initialize();
        }

        public void Initialize()
        {
            if (NativeAd != null) throw new InvalidOperationException("AdAgent.Initialize() should only be called once.");

            NativeAd = new ads.NativeAd(PlacementId);
            NativeAd.Delegate = new NativeAdListener(this);
        }

        public UIView Render(ads.NativeAdViewAttributes attributes) => ads.NativeAdView.Create(NativeAd, attributes);

        void RequestNativeAd() => NativeAd.LoadAd();

        class NativeAdListener : NSObject, ads.INativeAdDelegate
        {
            readonly AdAgent Agent;

            public NativeAdListener(AdAgent agent) => Agent = agent;

            public void NativeAdDidLoad(ads.NativeAd ad) => Agent.OnNativeAdReady(new NativeAdInfo(ad));

            public void NativeAdDidFail(ads.NativeAd ad, NSError error) => Agent.OnAdFailedToLoad("Ad Loading Failed");
        }
    }
}