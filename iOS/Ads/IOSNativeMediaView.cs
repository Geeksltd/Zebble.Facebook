using Facebook.AudienceNetwork;
using System;

namespace Zebble.FacebookAds
{
    public class IOSNativeMediaView : MediaView
    {
        NativeAdMediaView View;

        public IOSNativeMediaView(NativeAdMediaView view)
        {
            View = view;
            Frame = View.GetFrame();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) View = null;
            base.Dispose(disposing);
        }
    }
}