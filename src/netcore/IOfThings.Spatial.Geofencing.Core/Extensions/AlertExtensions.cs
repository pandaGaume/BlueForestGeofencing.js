using System;
using System.Collections.Generic;
using System.Linq;

namespace IOfThings.Spatial.Geofencing
{
    public static class AlertExtensions
    {
        public static int BuildTriggerMask(this IAlert a)
        {
            int m = 0x00000000;
            foreach(var str in a.RelativeTo)
            {
                if(Enum.TryParse<TriggerType>(str, out TriggerType i))
                {
                    m |= (0x01 << ((int)i));
                }
            }
            return m;
        }

        public static bool HasTrigger(this IAlert a, TriggerType t) => (a.TriggerMask & (0x01 << ((int)t))) != 0;
    }
}
