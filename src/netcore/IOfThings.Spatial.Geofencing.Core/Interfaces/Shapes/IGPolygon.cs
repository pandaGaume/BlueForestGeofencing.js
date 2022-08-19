using IOfThings.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Polygon")]
    public interface IGPolygon : IGPolyline
    {
    }
}
