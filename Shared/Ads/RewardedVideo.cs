namespace Zebble.FacebookAds
{
    public partial class RewardedVideoAd
    {

        public readonly AsyncEvent OnAdLoaded = new AsyncEvent();
        public readonly AsyncEvent OnAdClicked = new AsyncEvent();
        public readonly AsyncEvent OnAdClosed = new AsyncEvent();
        public readonly AsyncEvent OnAdVideoCompleted = new AsyncEvent();
        public readonly AsyncEvent<string> OnAdFailed = new AsyncEvent<string>();

        public string PlacementId { get; set; }
    }
}