using IOfThings.Spatial.Geography;
using System.Collections.Generic;
using System.Linq;

namespace IOfThings.Spatial.Geofencing
{
    public static class GeofenceExtensions
    {
        public static ENUSystem DefineGeodeticSystem(this IEnumerable<IGeofence> geofences, Ellipsoid ellipsoid)
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
            var bound = geofences.SelectMany(g => g.Nodes.Select(n => n.BoundingEnvelope)).Aggregate();
            var anchor = bound.GetCenter();
            var enu = new ENUSystem(anchor, ellipsoid);
            foreach (var g in geofences)
            {
                g.GeodeticSystem = enu;
            }
            return enu;
        }

        public static IList<ITreeNode> ValidateRoots(this IGeofence geofence, IList<ITreeNode> actual)
        {
            if( actual == null && geofence.Nodes.Count != 0)
            {
                if (geofence.RootIndices == null)
                {
                    geofence.RootIndices = new int[] { 0 };
                }
                IList<ITreeNode> roots = new List<ITreeNode>(geofence.RootIndices.Length);
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
    }
}
