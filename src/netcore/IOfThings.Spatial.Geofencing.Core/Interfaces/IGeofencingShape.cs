using IOfThings.Spatial.Geography;
using System.Collections.Generic;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public enum ShapeType
    {
        Circle,
        Rectangle,
        Path,
        Line,
        Polyline,
        Polygon
    }

    public interface IGeofencingShape : IGeofencingItem, IGeoBounded
    {
        IRadius Radius { get; set; }
        IValueRange<double> Elevation { get; set; }
        int GeometryIndex { get; set; }
        IEnumerable<IGeofencingEvent> CheckImpl(IPrimitive primitive, IGeofencingNode node, ISegment<IGeofencingSample> sample, IGeofencingEventFactory eventFactory);
        IEnumerable<IGeofencingEvent> CheckImpl(IPrimitive primitive, IGeofencingNode node, IGeofencingSample sample, IGeofencingEventFactory eventFactory);
        Vector3? Anchor { get; set; }
    }
}
