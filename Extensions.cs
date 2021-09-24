using System;

namespace Company.Function
{
    public static class Extensions
    {
        public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp )
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dateTime;
        }

        public static DateTime? UnixTimeStampToDateTimeNullable(this double unixTimeStamp )
        {
            return (DateTime?)unixTimeStamp.UnixTimeStampToDateTime();
        }
    }
}