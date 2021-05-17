using IOfThings.Spatial.Geofencing.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Polygon")]
    public interface IPolygon : IPolyline
    {
    }
}
