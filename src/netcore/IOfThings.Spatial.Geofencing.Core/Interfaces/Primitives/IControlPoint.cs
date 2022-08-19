
using IOfThings.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "ControlPoint")]
    public interface IControlPoint : IPrimitive
    {
    }
}
