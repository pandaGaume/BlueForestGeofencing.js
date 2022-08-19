using IOfThings.Text.Json;
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
