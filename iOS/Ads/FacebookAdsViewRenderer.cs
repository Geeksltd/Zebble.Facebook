using System;
using System.Threading.Tasks;
using UIKit;

namespace Zebble.FacebookAds
{
    public class FacebookAdsViewRenderer : INativeRenderer
    {
        UIView Result;

        public Task<UIView> Render(Renderer renderer)
        {
            if (renderer.View is NativeAdView native) Result = new IOSNativeAdView(native);
            else if (renderer.View is NativeAdMediaView media) Result = new IOSNativeMediaView(media);
            else throw new NotSupportedException();

            return Task.FromResult(Result);
        }

        public void Dispose()
        {
            Result.Dispose();
            Result = null;
        }
    }
}