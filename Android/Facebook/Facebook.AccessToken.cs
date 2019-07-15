namespace Zebble
{
    using Java.Util;
    using System;

    internal static class FacebookExtensions
    {
        static DateTime UnixStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime ToDateTime(this Date date)
        {
            try
            {
                return UnixStart.AddMilliseconds(date.Time);
            }
            catch (Exception ex)
            {
                Device.Log.Error(ex);
                return DateTime.Now.AddMinutes(5);
            }
        }
    }
}