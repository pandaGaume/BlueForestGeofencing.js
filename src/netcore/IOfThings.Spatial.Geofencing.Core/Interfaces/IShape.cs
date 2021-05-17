using IOfThings.Spatial.Geography;

namespace IOfThings.Spatial.Geofencing
{
    public enum RelativePosition
    {
        Inside,
        Outside,
        OnEdge
    }

    public enum ShapeType
    {
        Circle,
        Rectangle,
        Path,
        Polyline,
        Polygon
    }

    public interface IShape : IGeofencingItem, IWithFilter<ILocated>, IGeoBounded
    {
        IRadius Radius { get; set; }
        IValueRange<double> Elevation { get; set; }
        int GeometryIndex { get; set; }
    }
 }
