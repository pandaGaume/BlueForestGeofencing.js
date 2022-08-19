using IOfThings.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Polyline")]
    public interface IGPolyline : IGeofencingShape
    {
        int? Index { get; set; }
    }
}
