using IOfThings.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Calendar")]
    public interface ICalendar : IModifier, IExpirable, IWithPeriods
    {
    }
}
