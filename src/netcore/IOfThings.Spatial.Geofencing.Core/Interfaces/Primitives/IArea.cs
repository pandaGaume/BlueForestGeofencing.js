
using IOfThings.Spatial.Geofencing.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Area")]
    public interface IArea : IPrimitive
    {
    }
}
