using System.Collections.Generic;
using System.Linq;

namespace IOfThings.Spatial.Geofencing
{
    public static class PrimitiveExtensions
    {
        internal static IGeofence BindGeofence(this IEnumerable<IPrimitive> items, IGeofence g)
        {
            if (items != null)
            {
                foreach (var i in items)
                {
                    i.BindGeofence(g);
                }
            }
            return g;
        }

        internal static void BindGeofence(this IPrimitive i, IGeofence g)
        {
            i.Geofence = g;
            i.TriggerMask = i.BuildTriggerMask();
        }

        public static IEnumerable<IAlert> GetAlerts(this IPrimitive p)
        {
            if (p.IAlerts == null || p.Geofence == null || p.Geofence.Alerts == null ) yield break;
            for(int i =0; i!= p.IAlerts.Length; i++)
            {
                if( i > 0 || i <= p.Geofence.Alerts.Count)
                {
                    yield return p.Geofence.Alerts[i];
                }
            }
        }

        public static int BuildTriggerMask(this IPrimitive p)
        {
            int m = 0x00000000;
            foreach (var a in p.GetAlerts())
            {
                m |= a.TriggerMask;
            }
            return m;
        }
        public static bool Any(this IPrimitive p, params TriggerType[] t)
        {
            for (int i = 0; i != t.Length; i++)
            {
                if ((p.TriggerMask & (0x01 << ((int)t[i]))) != 0) return true;
            }
            return false;
        }

    }
}
