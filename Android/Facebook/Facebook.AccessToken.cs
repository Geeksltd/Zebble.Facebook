namespace Zebble
{
    using Java.Util;
    using System;

    public partial class Facebook
    {
        public partial class AccessToken
        {
            public static DateTime FromDate(Date date) =>
                new DateTime(date.Year, date.Month, date.Day, date.Hours, date.Minutes, date.Seconds);

        }
    }
}