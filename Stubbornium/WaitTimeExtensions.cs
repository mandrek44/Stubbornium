using System;

namespace Stubbornium
{
    public static class WaitTimeExtensions
    {
        public static TimeSpan ToTimeSpan(this WaitTime waitTime)
        {
            // TODO: make wait time configurable
            if (waitTime == WaitTime.Short)
                return TimeSpan.FromSeconds(0.5);
            else if (waitTime == WaitTime.Long)
                return TimeSpan.FromSeconds(5);

            throw new NotSupportedException(waitTime.ToString());
        }
    }
}