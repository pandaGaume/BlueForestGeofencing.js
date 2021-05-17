using IOfThings.Spatial.Geofencing.Text.Json;
using System.Collections.Generic;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Calendar")]
    public interface ICalendar : IModifier, IExpirable
    {
        public List<Period> Validities { get; set; }
    }
}
