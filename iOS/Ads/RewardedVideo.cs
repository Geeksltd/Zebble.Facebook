using Foundation;
using System.Threading.Tasks;
using UIKit;
using ads = Facebook.AudienceNetwork;

namespace Zebble.FacebookAds
{
    public partial class RewardedVideoAd
    {
        ads.RewardedVideoAd RewardedVideo;

        public Task LoadVideo() => Thread.UI.Run(() =>
        {
            if (string.IsNullOrEmpty(PlacementId))
            {
                OnAdFailed.Raise("The PlacementId of the RewardedVideoAd has not specified!");
                return;
            }

            RewardedVideo = new ads.RewardedVideoAd(PlacementId);
            RewardedVideo.Delegate = new RewardedVideoListener(this);
            RewardedVideo.LoadAd();
        });

        public Task ShowVideo() => Thread.UI.Run(() =>
        {
            var controller = UIRuntime.NativeRootScreen as UIViewController;
            RewardedVideo.ShowAd(controller);
        });

        class RewardedVideoListener : NSObject, ads.IRewardedVideoAdDelegate
        {
            readonly RewardedVideoAd Ad;

            public RewardedVideoListener(RewardedVideoAd ad) => Ad = ad;

            public void RewardedVideoAdDidClick(ads.RewardedVideoAd ad) => Ad.OnAdClicked.Raise();

            public void RewardedVideoAdDidLoad(ads.RewardedVideoAd ad) => Ad.OnAdLoaded.Raise();

            public void OnError(ads.RewardedVideoAd ad, NSError error) => Ad.OnAdFailed.Raise("An error occured in the RewardedVideoAd");

            public void RewardedVideoAdDidClose(ads.RewardedVideoAd ad) => Ad.OnAdClosed.Raise();

            public void RewardedVideoAdVideoComplete(ads.RewardedVideoAd ad) => Ad.OnAdVideoCompleted.Raise();
        }
    }
}