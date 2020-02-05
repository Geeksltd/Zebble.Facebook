using Android.Runtime;
using System;
using System.Threading.Tasks;

namespace Zebble.FacebookAds
{
    [Preserve]
    public class FacebookAdsViewRenderer : INativeRenderer
    {
        Android.Views.View Result;

        public Task<Android.Views.View> Render(Renderer renderer)
        {
            if (renderer.View is NativeAdView native) Result = new AndroidNativeAdView(native);
            else if (renderer.View is NativeAdMediaView media) Result = new AndroidNativeMediaView(media);
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