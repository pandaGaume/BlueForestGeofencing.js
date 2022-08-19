using IOfThings.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Line")]
    public interface IGLine : IGeofencingShape
    {
        int InitialIndex { get; set; }
        int FinalIndex { get; set; }
    }
}
