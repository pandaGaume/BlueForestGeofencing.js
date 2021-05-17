using IOfThings.Spatial.Geofencing.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Circle")]
    public interface ICircle : IShape
    {
    }
}
