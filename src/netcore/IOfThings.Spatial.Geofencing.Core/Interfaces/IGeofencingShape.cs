using IOfThings.Spatial.Geography;
using IOfThings.Telemetry;
using System.Collections.Generic;

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
        IEnumerable<IConditionEvent> CheckImpl(IPrimitive primitive, IGeofencingNode node, ISegment<IGeofencingSample> sample);
        IEnumerable<IConditionEvent> CheckImpl(IPrimitive primitive, IGeofencingNode node, IGeofencingSample sample);
    }
}
