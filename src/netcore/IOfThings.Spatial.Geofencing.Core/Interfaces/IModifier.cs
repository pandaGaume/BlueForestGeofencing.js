using IOfThings.Spatial.Geography;

namespace IOfThings.Spatial.Geofencing
{
    public enum ModifierType
    {
        Calendar,
        Predicate,
    }

    public interface IModifier : IGeofencingItem, IWithPriority, IWithFilter<IGeofencingSample>, IWithFilter<ISegment<IGeofencingSample>>
    {
        string Category { get; set; }
    }
}
