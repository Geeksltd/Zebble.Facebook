using Android.Runtime;
using Android.Views;
using System;
using Xamarin.Facebook.Ads;

namespace Zebble.FacebookAds
{
    public class AndroidNativeMediaView : MediaView
    {
        NativeAdMediaView View;

        [Preserve]
        public AndroidNativeMediaView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public AndroidNativeMediaView(NativeAdMediaView view) : base(Renderer.Context)
        {
            View = view;
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) View = null;
            base.Dispose(disposing);
        }
    }
}