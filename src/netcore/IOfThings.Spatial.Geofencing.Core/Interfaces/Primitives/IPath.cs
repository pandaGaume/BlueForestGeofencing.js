
using IOfThings.Spatial.Geofencing.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Path")]
    public interface IPath : IPrimitive
    {
    }
}
