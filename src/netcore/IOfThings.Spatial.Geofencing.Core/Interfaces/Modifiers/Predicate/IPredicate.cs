
using IOfThings.Spatial.Geofencing.Text.Json;
using IOfThings.Spatial.Geography;
using System;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Predicate")]
    public interface IPredicate : IModifier
     {
        Delegate Logic { get; set; }

        bool Invoke(IPredicateParams data);
    }
}
