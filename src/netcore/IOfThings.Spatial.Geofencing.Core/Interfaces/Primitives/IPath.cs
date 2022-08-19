
using IOfThings.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Path")]
    public interface IPath : IPrimitive
    {
    }
}
