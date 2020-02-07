using System.Threading.Tasks;
using ads = Xamarin.Facebook.Ads;

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

            RewardedVideo = new ads.RewardedVideoAd(Renderer.Context, PlacementId);
            RewardedVideo.SetAdListener(new RewardedVideoListener(this));
            RewardedVideo.LoadAd();
        });

        public Task ShowVideo() => Thread.UI.Run(() =>
        {
            if (!RewardedVideo.IsAdLoaded)
            {
                OnAdFailed.Raise("The RewardedVideoAd has not loaded yet!");
                return;
            }

            RewardedVideo.Show();
        });

        class RewardedVideoListener : Java.Lang.Object, ads.IRewardedVideoAdListener
        {
            RewardedVideoAd Ad;

            public RewardedVideoListener(RewardedVideoAd ad) => Ad = ad;

            public void OnAdClicked(ads.IAd p0) => Ad.OnAdClicked.Raise();

            public void OnAdLoaded(ads.IAd p0) => Ad.OnAdLoaded.Raise();

            public void OnError(ads.IAd p0, ads.AdError p1) => Ad.OnAdFailed.Raise(p1.ErrorMessage);

            public void OnLoggingImpression(ads.IAd p0) { }

            public void OnRewardedVideoClosed() => Ad.OnAdClosed.Raise();

            public void OnRewardedVideoCompleted() => Ad.OnAdVideoCompleted.Raise();
        }
    }
}