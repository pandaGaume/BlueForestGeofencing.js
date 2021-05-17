
using IOfThings.Spatial.Geofencing.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "ControlPoint")]
    public interface IControlPoint : IPrimitive
    {
    }
}
