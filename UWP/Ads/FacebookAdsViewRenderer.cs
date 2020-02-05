using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Zebble.FacebookAds
{
    public class FacebookAdsViewRenderer : INativeRenderer
    {
        FrameworkElement Result;

        public Task<FrameworkElement> Render(Renderer renderer)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            Result = null;
        }
    }
}