using IOfThings.Spatial.Geography;
using IOfThings.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IOfThings.Spatial.Geofencing
{
    public static class GeofenceExtensions
    {
        public static IGeofence Mount(this IGeofence geofence)
        {
            geofence.Shapes.BindGeofence(geofence);
            geofence.Modifiers.BindGeofence(geofence);
            geofence.Alerts.BindGeofence(geofence);
            geofence.Primitives.BindGeofence(geofence);
            geofence.Nodes.BindGeofence(geofence);
            return geofence;
        }

        public static IEnumerable<IGeofencingItem> Items(this IGeofence geofence)
        {
            List<IGeofencingItem> _items = new List<IGeofencingItem>();
            if(geofence.Nodes!= null) _items.AddRange(geofence.Nodes);
            if (geofence.Shapes != null) _items.AddRange(geofence.Shapes);
            if (geofence.Primitives != null) _items.AddRange(geofence.Primitives);
            if (geofence.Modifiers != null) _items.AddRange(geofence.Modifiers);
            if (geofence.Alerts != null) _items.AddRange(geofence.Alerts);
            return _items;
        }

        public static IEnumerable<IGeofencingNode> ActivNodes(this IGeofence geofence)
        {
            foreach( var n in RootNodes(geofence).Where(n => !n.Consumed && n.Enabled))
            {
                yield return n;
                foreach( var d in n.Descendants(n => !n.Consumed && n.Enabled))
                {
                    yield return d;
                }
            }
            yield break;
        }

        public static IGeofencingNode SearchNodes(this IEnumerable<IGeofence> geofences, string id) => geofences.Select(g=>g.SearchNodes(id)).FirstOrDefault();
        public static IGeofencingNode SearchNodes(this IGeofence geofence, string id)
        {
            foreach(var n in RootNodes(geofence))
            {
                if(n.Id.CompareTo(id) == 0)
                {
                    return n;
                }
                var d = n.Descendants(n => n.Id.CompareTo(id) == 0).FirstOrDefault();
                if( d != default(IGeofencingNode))
                {
                    return d;
                }
            }
            return default(IGeofencingNode);
        }

        public static IEnumerable<IGeofencingNode> RootNodes(this IGeofence geofence)
        {
            foreach(var i in geofence.RootIndices)
            {
                if (i >= 0 && i < geofence.Nodes.Count)
                {
                    yield return geofence.Nodes[i];
                }
            }
        }

        public static ENUSystem UpdateGeodeticSystem(this IEnumerable<IGeofence> geofences, Ellipsoid ellipsoid)
        {
            var system = new EllipticSystem(ellipsoid);
            foreach (var g in geofences)
            {
                if (g.GeodeticSystem != system)
                {
                    g.Nodes.Invalidate();
                    g.GeodeticSystem = system;
                }
            }
            var bounds = geofences.SelectMany(g => g.Nodes.Select(n => n.Envelope)).Aggregate();
            if (bounds != null)
            {
                var anchor = bounds.GetCenter();
                var enu = new ENUSystem(anchor, ellipsoid);
                foreach (var g in geofences)
                {
                    g.GeodeticSystem = enu;
                }
                return enu;
            }
            return null;
        }

        public static IList<IGeofencingNode> ValidateRoots(this IGeofence geofence, IList<IGeofencingNode> actual)
        {
            if( actual == null && geofence.Nodes.Count != 0)
            {
                if (geofence.RootIndices == null)
                {
                    geofence.RootIndices = new int[] { 0 };
                }
                IList<IGeofencingNode> roots = new List<IGeofencingNode>(geofence.RootIndices.Length);
                var nodes = geofence.Nodes;
                for (int i = 0; i != geofence.RootIndices.Length; i++)
                {

                    if (i > 0 || i <= nodes.Count)
                    {
                        roots.Add(nodes[i]);
                    }
                }
                actual = roots;
            }
            return actual;
         }

        public static IEnumerable<IConditionEvent> Check(this IGeofence geofence, ISegment<IGeofencingSample> sample, IGeofencingCheckOptions options = null)
        {
            // sort case where unconsistent segment
            if (sample.First == default(IGeofencingSample))
            {
                if (sample.Second == default(IGeofencingSample))
                {
                    return Enumerable.Empty<IConditionEvent>();
                }
                return Check(geofence, sample.Second, options);
            }

            if (sample.Second == default(IGeofencingSample) || sample.First == sample.Second)
            {
                return Check(geofence, sample.First, options);
            }

            return CheckInternal(geofence, sample, options);
        }

        public static IConditionEvent[] Check(this IEnumerable<IGeofence> geofences, ISegment<IGeofencingSample> sample, IGeofencingCheckOptions options = null)
        {
            // sort case where unconsistent segment
            if (sample.First == default(IGeofencingSample))
            {
                if (sample.Second == default(IGeofencingSample))
                {
                    return Array.Empty<IConditionEvent>();
                }
                return Check(geofences, sample.Second,options);
            }

            if (sample.Second == default(IGeofencingSample) || sample.First == sample.Second)
            {
                return Check(geofences, sample.First,options);
            }

            return CheckInternal(geofences, sample,options);
        }

        internal static IConditionEvent[] CheckInternal(this IGeofence geofence, ISegment<IGeofencingSample> sample, IGeofencingCheckOptions options = null)
        {
            try
            {
                // note : modifiers are already sorted into GetPreModifiers
                if (geofence.GetPreModifiers<IModifier>().ApplyAll(sample, geofence))
                {
                    return geofence.Primitives.CheckInternal(sample,options);
                }
            }
            finally
            {
                geofence.GetPostModifiers<IModifier>().ApplyAll(sample, geofence);
            }
            return Array.Empty<IConditionEvent>();

        }

        internal static IConditionEvent[] CheckInternal(this IEnumerable<IGeofence> geofences, ISegment<IGeofencingSample> sample, IGeofencingCheckOptions options = null) => geofences.SelectMany(p => p.CheckInternal(sample,options)).ToArray();

        public static IConditionEvent[] Check(this IGeofence geofence, IGeofencingSample sample, IGeofencingCheckOptions options = null)
        {
            try
            {
                // note : modifiers are already sorted into GetPreModifiers
                if (geofence.GetPreModifiers<IModifier>().ApplyAll(sample, geofence))
                {
                    return geofence.Primitives.CheckInternal(sample,options);
                }
            }
            finally
            {
                geofence.GetPostModifiers<IModifier>().ApplyAll(sample, geofence);
            }
            return Array.Empty<IConditionEvent>();

        }

        public static IConditionEvent[] Check(this IEnumerable<IGeofence> geofences, IGeofencingSample sample, IGeofencingCheckOptions options = null) => geofences.SelectMany(p => p.Check(sample,options)).ToArray();
    }
}
