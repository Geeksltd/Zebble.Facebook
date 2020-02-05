using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Xamarin.Facebook.Ads.NativeAdBase;
using ads = Xamarin.Facebook.Ads;

namespace Zebble.FacebookAds
{
    [Preserve]
    internal class AndroidNativeAdViewContainer : FrameLayout
    {
        public AndroidNativeAdViewContainer() : base(Renderer.Context) { }

        [Preserve]
        public AndroidNativeAdViewContainer(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
    }

    [Preserve]
    public class AndroidNativeAdView : FrameLayout
    {
        NativeAdView View;
        AndroidNativeAdViewContainer Container;
        NativeAdInfo CurrentAd;
        AdAgent Agent;

        public AndroidNativeAdView(NativeAdView view) : base(Renderer.Context)
        {
            View = view;

            Agent = (view.Agent ?? throw new Exception(".NativeAdView.Agent is null"));

            var attributes = new ads.NativeAdViewAttributes(Renderer.Context)
                .SetBackgroundColor(Android.Graphics.Color.Transparent);

            var nativeAdView = Agent.Render(attributes);
            nativeAdView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            Container = new AndroidNativeAdViewContainer();
            Container.AddView(nativeAdView);

            AddView(Container);

            view.RotateRequested.Handle(LoadNext);
            LoadNext().RunInParallel();
        }

        async Task LoadNext()
        {
            var ad = await Agent.GetNativeAd();
            CreateAdView(ad);
        }

        void CreateAdView(NativeAdInfo ad)
        {
            CurrentAd = ad;
            View.Ad.Value = ad;

            if (ad is FailedNativeAdInfo)
            {
                View.HeadLineView.Text = ad.Headline;
                View.BodyView.Text = ad.Body;
                View.CallToActionView.Text = ad.CallToAction;
            }
            else
            {
                var nativeAd = ad.Native;
                nativeAd.UnregisterView();

                var iconView = View.IconView?.Native();
                var titleView = View.HeadLineView?.Native();
                var bodyView = View.BodyView?.Native();
                var mediaView = View.MediaView?.Native();
                var callToactionView = View.CallToActionView?.Native();
                var socialContextView = View.SocialContextView?.Native();

                var clickables = new List<Android.Views.View> { mediaView, callToactionView };

                nativeAd.RegisterViewForInteraction(
                    view: Container,
                    mediaView: mediaView as ads.MediaView,
                    clickableViews: clickables);

                if (iconView != null) NativeComponentTag.TagView(iconView, NativeComponentTag.AdIcon);
                if (titleView != null) NativeComponentTag.TagView(titleView, NativeComponentTag.AdTitle);
                if (bodyView != null) NativeComponentTag.TagView(bodyView, NativeComponentTag.AdBody);
                if (mediaView != null) NativeComponentTag.TagView(mediaView, NativeComponentTag.AdMedia);
                if (callToactionView != null) NativeComponentTag.TagView(callToactionView, NativeComponentTag.AdCallToAction);
                if (socialContextView != null) NativeComponentTag.TagView(socialContextView, NativeComponentTag.AdSocialContext);
            }
        }

        [Preserve]
        public AndroidNativeAdView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        protected override void Dispose(bool disposing)
        {
            if (disposing) View = null;
            base.Dispose(disposing);
        }
    }
}