using IOfThings.Spatial.Geography;
using System.Collections.Generic;

namespace IOfThings.Spatial.Geofencing
{
    public static class ModifierExtensions
    {
        public static void Apply<T> (this IEnumerable<IModifier> modifiers, T data, IGeofencingItem target)
            where T : ILocated
        {
            foreach (var m in modifiers) m.Apply(data, target);
        }
    }
}
