using IOfThings.Spatial.Geography;

namespace IOfThings.Spatial.Geofencing
{
    public enum ModifierType
    {
        Calendar,
        Predicate,
    }
    public interface IBehavior<T>
    {
        bool Apply(T mess, params IGeofencingItem[] target);
    }

    public interface IModifier : IGeofencingItem, IWithPriority, IBehavior<IGeofencingSample>, IBehavior<ISegment<IGeofencingSample>>, IBehavior<IGeofencingEvent>
    {
        string Category { get; set; }
    }
}
