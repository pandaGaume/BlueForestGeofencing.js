using System.Collections.Generic;

namespace IOfThings.Spatial.Geography.Text
{
    public enum GeospatialType
    {
        Point,
        LineString,
        Polygon,
        MultiPoint,
        MultiLineString,
        MultiPolygon
    }

    public interface IGeoJsonFeature<T> : IGeoBounded
    {
        IGeoJsonGeometry Geometry { get; set; }
        Dictionary<string,T> Properties { get; }
    }

    public interface IGeoJsonFeatureCollection<T> : IEnumerable<IGeoJsonFeature<T>>, IGeoBounded
    {
    }

    public interface IGeoJsonGeometry : IGeoBounded
    {
        GeospatialType Type { get; }
    }

    public interface ISimpleGeometry : IGeoJsonGeometry
    {
        double[] Coordinates { get; set; }
    }

    public interface ICompoundGeometry : IGeoJsonGeometry
    {
        double[][] Coordinates { get; set; }
    }


    public interface IGeoJsonBoundingBox
    {
        double West { get; set; }
        double South { get; set; }
        double East { get; set; }
        double North { get; set; }
        double? AltMin { get; set; }
        double? AltMax { get; set; }
    }
}
