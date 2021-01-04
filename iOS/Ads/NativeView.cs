using ads = Facebook.AudienceNetwork;
using System;
using System.Threading.Tasks;
using UIKit;

namespace Zebble.FacebookAds
{
    public class IOSNativeAdView : UIView
    {
        NativeAdView View;
        NativeAdInfo CurrentAd;
        readonly AdAgent Agent;

        public IOSNativeAdView(NativeAdView view)
        {
            View = view;

            Agent = view.Agent ?? throw new Exception(".NativeAdView.Agent is null");

            var attributes = new ads.NativeAdViewAttributes { BackgroundColor = UIColor.Clear };

            var nativeAdView = Agent.Render(attributes);
            nativeAdView.Frame = View.GetFrame();
            
            Add(nativeAdView);

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
                var mediaView = View.MediaView?.Native();
                var callToActionView = View.CallToActionView?.Native();

                var clickables = new UIView[2] { mediaView, callToActionView };

                nativeAd.RegisterView(
                    view: this,
                    mediaView: mediaView as ads.MediaView,
                    iconView: iconView as ads.MediaView,
                    UIRuntime.NativeRootScreen as UIViewController,
                    clickableViews: clickables);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) View = null;
            base.Dispose(disposing);
        }
    }
}