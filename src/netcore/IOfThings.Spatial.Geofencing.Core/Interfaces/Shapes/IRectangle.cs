using IOfThings.Spatial.Geofencing.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Rectangle")]
    public interface IRectangle : IShape
    {
    }
}
