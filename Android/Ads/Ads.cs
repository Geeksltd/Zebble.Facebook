using Google.Ads.Identifier;

namespace Zebble.FacebookAds
{
    public partial class Ads
    {
        public static string GetTestDeviceId()
        {
            var info = AdvertisingIdClient.GetAdvertisingIdInfo(UIRuntime.CurrentActivity);
            return info.Id;
        }
    }
}
