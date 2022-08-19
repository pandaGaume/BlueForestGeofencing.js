
using IOfThings.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Fence")]
    public interface IFence : IPrimitive
    {
    }
}
