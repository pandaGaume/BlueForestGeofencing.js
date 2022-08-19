using IOfThings.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Circle")]
    public interface IGCircle : IGeofencingShape
    {
        int CenterIndex { get; set; }
    }
}
