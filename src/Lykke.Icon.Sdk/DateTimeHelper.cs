using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Icon.Sdk
{
    public static class DateTimeHelper
    {
        public static long GetCurrentUnixTime()
        {
            DateTime foo = DateTime.UtcNow;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            return unixTime;
        }

        public static long GetCurrentUnixTimeWithTimeSpan(TimeSpan timeSpan)
        {
            DateTime foo = DateTime.UtcNow + timeSpan;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            return unixTime;
        }
    }
}
