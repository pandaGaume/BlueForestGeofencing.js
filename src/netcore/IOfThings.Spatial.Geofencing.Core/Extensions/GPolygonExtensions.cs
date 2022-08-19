using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Text.GeoJson;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace IOfThings.Spatial.Geofencing
{
    public static class GPolygonExtensions
    {
        public static IEnumerable<IEnumerable<ILocation>> GetGeometries(this IGPolygon shape)
        {
            if (shape?.Geofence?.Geometry != null)
            {
                var geometry = shape.Geofence.Geometry;

                // accept feature as indirection
                if (geometry is GeoJsonFeature f)
                {
                    geometry = f.Geometry;
                }

                if (geometry is IGeoJsonGeometryCollection c)
                {
                     geometry = c.Geometries.ElementAt(shape.GeometryIndex);
                }

                float[][][] coordinates = null;
                if (geometry is GeoJsonMultiPolygon mp)
                {
                    var i = shape.Index.HasValue ? shape.Index.Value : shape.GeometryIndex;
                    coordinates = mp.Coordinates[i];
                }
                else if (geometry is GeoJsonPolygon p)
                {
                    coordinates = p.Coordinates;
                }
                if (coordinates != null)
                {
                    foreach (var subCoordinates in coordinates)
                    {
                        IEnumerable<ILocation> locations = subCoordinates.Select((c) => new Location(c[1], c[0], c.Length > 2 ? (float?)c[2] : null));
                        yield return locations.ToArray();
                    }
                }
                yield break;
            }
        }

        public static IPolygon ToPolygon(this IGPolygon shape, Matrix4x4 t, ENUSystem system)
        {
            // For type "Polygon", the "coordinates" member must be an array of LinearRing coordinate arrays. For Polygons with multiple rings, the first must be the exterior ring and any others must be interior rings or holes.
            var geometries = shape.GetGeometries().ToArray();

            // create the exterior
            var ext = geometries[0];
            if( t.IsIdentity == false)
            {
                ext = ext.TransformInPlace(t);
            }
            var coordinates = ext.ToENU(system);
            var polygon = new Polygon(coordinates).ForceDirection(PolygonOrientation.clockwise);
            if( geometries.Length > 1)
            {
                for(int i=1; i != geometries.Length; i++)
                {
                    var hole = geometries[0];
                    if (t.IsIdentity == false)
                    {
                        hole = hole.TransformInPlace(t);
                    }
                    var holeCoordinates = hole.ToENU(system);
                    polygon.AddInner(new Polygon(holeCoordinates).ForceDirection(PolygonOrientation.counterclockwise));
                }
            }
            return polygon;
        }
    }
}
