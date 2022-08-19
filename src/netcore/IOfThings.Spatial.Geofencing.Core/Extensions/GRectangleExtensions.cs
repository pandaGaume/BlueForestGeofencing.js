using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Text.GeoJson;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public static class GRectangleExtensions
    {
        public static IEnumerable<ILocation> GetGeometries(this IGRectangle shape, Matrix4x4 t)
        {
            if (shape?.Geofence?.Geometry != null)
            {
                var geometry = shape.Geofence.Geometry;

                // accept feature as indirection
                if (geometry is GeoJsonFeature f)
                {
                    geometry = f.Geometry;
                }
                if (geometry is GeoJsonPolygon p)
                {
                    var coordinates = p.Coordinates[0];
                    if (coordinates != null && coordinates.Length >= 5)
                    {
                        IEnumerable<ILocation> locations = coordinates.Select((c) => new Location(c[1], c[0], c.Length > 2 ? (float?)c[2] : null));
                        if(!t.IsIdentity)
                        {
                            locations = locations.Select((l) => l.TransformInPlace(t));
                        }
                        return locations;
                    }
                }
            }
            return Enumerable.Empty<ILocation>();
        }
        public static RelativePosition GetRelativePosition(this IGRectangle shape, ILocation l)
        {
            return RelativePosition.Unknown;
        }
    }
}
