namespace Zebble.FacebookAds
{
    public partial class NativeAdInfo
    {
        public virtual string Headline { get; set; } = "...";
        public virtual string Advertiser { get; set; }
        public virtual string Body { get; set; }
        public virtual string SocialContext { get; set; }
        public virtual string CallToAction { get; set; } = "Open";
        public virtual byte[] Icon { get; set; }

        public NativeAdInfo()
        {
        }
    }
}
