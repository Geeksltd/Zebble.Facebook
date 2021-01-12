namespace Zebble.FacebookAds
{
    using Olive;

    public class NativeAdView : View, IRenderedBy<FacebookAdsViewRenderer>
    {
        public readonly Bindable<NativeAdInfo> Ad = new Bindable<NativeAdInfo>(new NativeAdInfo());
        internal readonly AsyncEvent RotateRequested = new AsyncEvent();

        public void Rotate() => RotateRequested.RaiseOn(Thread.UI);

        public TextView HeadLineView { get; set; }
        public TextView BodyView { get; set; }
        public Button CallToActionView { get; set; }
        public NativeAdMediaView IconView { get; set; }
        public TextView SocialContextView { get; set; }
        public TextView AdvertiserView { get; set; }
        public NativeAdMediaView MediaView { get; set; }

        public AdAgent Agent { get; set; }
    }
}
