using System;

namespace Hanoi.Util
{
    public static class TimeUtil
    {
        public static string Format(this TimeSpan timeSpan)
        {
            var milliseconds = timeSpan.TotalMilliseconds;
            int seconds = (int)milliseconds / 1000;
            int remainingMillis = (int)milliseconds % 1000;

            int minutes = seconds / 60;
            if (minutes == 0)
                return $"{seconds}.{remainingMillis} s";
            var remainingSeconds = minutes % 60;
            return $"{minutes}:{remainingSeconds.ToString().PadLeft(2, '0')}.{remainingMillis} min";
        }
    }
}
