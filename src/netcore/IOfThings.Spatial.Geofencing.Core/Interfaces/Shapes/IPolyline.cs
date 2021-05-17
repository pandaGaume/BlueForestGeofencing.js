using IOfThings.Spatial.Geofencing.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Polyline")]
    public interface IPolyline : IShape
    {
    }
}
