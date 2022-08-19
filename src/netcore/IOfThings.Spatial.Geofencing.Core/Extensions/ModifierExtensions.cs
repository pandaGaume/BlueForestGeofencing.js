using IOfThings.Spatial.Geography;
using System.Collections.Generic;
using System.Linq;

namespace IOfThings.Spatial.Geofencing
{
    public static class ModifierExtensions
    {
        public static bool ApplyAll(this IEnumerable<IModifier> modifiers, ISegment<IGeofencingSample> data, params IGeofencingItem[] target)
        {
            if (modifiers != null)
            {
                return modifiers.All(m => m.Apply(data, target));
            }
            return true;
        }
        public static bool ApplyAll(this IEnumerable<IModifier> modifiers, IGeofencingSample data, params IGeofencingItem[] target)
        {
            if (modifiers != null)
            {
                return modifiers.All(m => m.Apply(data, target));
            }
            return true;
        }
    }
}
