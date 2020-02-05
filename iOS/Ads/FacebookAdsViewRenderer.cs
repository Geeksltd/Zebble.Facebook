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
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            Result.Dispose();
            Result = null;
        }
    }
}