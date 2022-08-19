using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Text.GeoJson;
using System.Linq;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public static class GLineExtensions
    {
        public static ILocation InitialLocation(this IGLine line, Matrix4x4 t) => GetLocation(line, 0, line.InitialIndex, t);
        public static ILocation FinalLocation(this IGLine line, Matrix4x4 t) => GetLocation(line, 1, line.FinalIndex, t);
        public static ILocation InitialLocation(this IGLine line) => GetLocation(line, 0, line.InitialIndex, Matrix4x4.Identity);
        public static ILocation FinalLocation(this IGLine line) => GetLocation(line, 1, line.FinalIndex, Matrix4x4.Identity);

        private static ILocation GetLocation(this IGLine line, int index, int defaultIndex, Matrix4x4 t)
        {
            var geom = line.Geofence?.Geometry;
            if (geom != null)
            {
                if (geom is IGeoJsonGeometryCollection c)
                {
                    geom = c.Geometries.ElementAt(line.GeometryIndex);
                }
                if (geom is IGeoJsonLineString ls)
                {
                    var l = ls.Coordinates[defaultIndex].ToLocation();
                    return t.IsIdentity ? l : l.Transform(t);
                }
                if (geom is IGeoJsonMultiPoint mp)
                {
                    var l =  mp.Coordinates[index].ToLocation();
                    return t.IsIdentity ? l : l.Transform(t);
                }
            }
            return default(ILocation);
        }

        public static IEnvelope BoundingEnvelope(this IGLine line)
        {
            ILocation[] locations = { line.InitialLocation(), line.FinalLocation() };
            return locations.Aggregate();
        }

        public static IEnvelope BoundingEnvelope(this IGLine line, Matrix4x4 t)
        {
            ILocation[] locations = { line.InitialLocation(t), line.FinalLocation(t) };
            return locations.Aggregate();
        }
    }
}
