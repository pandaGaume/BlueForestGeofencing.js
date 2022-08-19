
using IOfThings.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Area")]
    public interface IArea : IPrimitive
    {
    }
}
