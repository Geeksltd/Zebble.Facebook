using Facebook.AudienceNetwork;
using Olive;

namespace Zebble.FacebookAds
{
    partial class NativeAdInfo
    {
        public NativeAd Native { get; private set; }

        public NativeAdInfo(NativeAd ad)
        {
            Native = ad;
            Headline = ad.Headline.OrEmpty();
            Advertiser = ad.AdvertiserName.OrEmpty();
            Body = ad.BodyText.OrEmpty();
            //Icon = ConvertUIImageToByteArray(ad.AdChoicesIcon);
            SocialContext = ad.SocialContext.OrEmpty();
            CallToAction = ad.CallToAction.OrEmpty();
        }

        byte[] ConvertUIImageToByteArray(AdImage img)
        {
            if (img == null) return new byte[0];

            var image = Zebble.Device.Network.Download(img.Url).Result;
            if (image != null) return image;

            return new byte[0];
        }
    }
}