using Android.Graphics;
using Android.Graphics.Drawables;
using System.IO;
using Olive;

namespace Zebble.FacebookAds
{
    partial class NativeAdInfo
    {
        public Xamarin.Facebook.Ads.NativeAd Native { get; private set; }

        public NativeAdInfo(Xamarin.Facebook.Ads.NativeAd ad)
        {
            Native = ad;
            Headline = ad.AdHeadline.OrEmpty();
            Advertiser = ad.AdvertiserName.OrEmpty();
            Body = ad.AdBodyText.OrEmpty();
            //Icon = ToByteArray(ad.AdIcon?.);
            SocialContext = ad.AdSocialContext.OrEmpty();
            CallToAction = ad.AdCallToAction.OrEmpty();
        }

        static byte[] ToByteArray(Drawable drawable)
        {
            if (drawable == null) return new byte[0];

            var bitmap = ((BitmapDrawable)drawable).Bitmap;
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                return stream.ReadAllBytes();
            }
        }
    }
}