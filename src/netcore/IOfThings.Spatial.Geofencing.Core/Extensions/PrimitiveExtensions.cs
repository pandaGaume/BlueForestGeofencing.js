using IOfThings.Spatial.Geography;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IOfThings.Spatial.Geofencing
{
    public static class PrimitiveExtensions
    {
        internal static void BindGeofence(this IPrimitive i, IGeofence g)
        {
            i.Geofence = g;
            i.TriggerMask = i.BuildTriggerMask();
        }
        public static IEnumerable<IAlert> GetAlerts(this IPrimitive p)
        {
            if (p.IAlerts == null || p.Geofence == null || p.Geofence.Alerts == null) yield break;
            for (int i = 0; i != p.IAlerts.Length; i++)
            {
                if (i > 0 || i <= p.Geofence.Alerts.Count)
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
        public static IEnumerable<IGeofencingNode> GetNodes(this IPrimitive p, Func<IGeofencingNode, bool> predicate = null)
        {
            if (p?.Geofence != null && p?.INodes != null)
            {
                var nodes = p.Geofence.Nodes;
                if (nodes != null)
                {
                    var selectedFromIndices = p.INodes.Where(i => i >= 0 && i < nodes.Count).Select(i => nodes[i]);
                    return predicate != null ? selectedFromIndices.Where(predicate) : selectedFromIndices;
                }
            }
            return Enumerable.Empty<IGeofencingNode>();
        }
        internal static IGeofencingEvent[] CheckInternal(this IPrimitive primitive, ISegment<IGeofencingSample> segment, IGeofencingCheckOptions options )
        {
            if (primitive.GetPreModifiers<IModifier>().ApplyAll(segment, primitive))
            {
                var list = new List<IGeofencingEvent>(1);
                var ef = options?.EventFactory ?? throw new ArgumentNullException(nameof(options.EventFactory));
                foreach (var node in primitive.GetNodes())
                {
                    foreach (var alarm in node.CheckInternal(primitive, segment, ef))
                    {
                        if (options?.MessageFactory != null)
                        {
                            alarm.Message = options.MessageFactory.BuildMessage(alarm, node);
                        }
                        list.Add(alarm);
                    }
                }
                return list.ToArray();
            }
            return Array.Empty<IGeofencingEvent>();
        }
        internal static IGeofencingEvent[] CheckInternal(this IPrimitive primitive, IGeofencingSample sample, IGeofencingCheckOptions options)
        {
            if (primitive.GetPreModifiers<IModifier>().ApplyAll(sample, primitive))
            {
                var list = new List<IGeofencingEvent>(1);
                var ef = options?.EventFactory ?? throw new ArgumentNullException(nameof(options.EventFactory));
                foreach (var node in primitive.GetNodes())
                {
                    foreach (var alarm in node.CheckInternal(primitive, sample, ef))
                    {
                        if(options?.MessageFactory != null)
                        {
                            alarm.Message = options.MessageFactory.BuildMessage(alarm, node);
                        }
                        list.Add(alarm);
                    }
                }
                return list.ToArray();
            }
            return Array.Empty<IGeofencingEvent>();
        }
        internal static IGeofencingEvent[] CheckInternal(this IEnumerable<IPrimitive> primitives, ISegment<IGeofencingSample> segment, IGeofencingCheckOptions options )
        {
            return primitives.SelectMany(p => p.CheckInternal(segment,options)).ToArray();
        }
        internal static IGeofencingEvent[] CheckInternal(this IEnumerable<IPrimitive> primitives, IGeofencingSample sample, IGeofencingCheckOptions options)
        {
            return primitives.SelectMany(p => p.CheckInternal(sample,options)).ToArray();
        }
    }
}
