using IOfThings.Spatial.Geography;
using IOfThings.Telemetry;

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

    public interface IModifier : IGeofencingItem, IWithPriority, IBehavior<IGeofencingSample>, IBehavior<ISegment<IGeofencingSample>>, IBehavior<IConditionEvent>
    {
        string Category { get; set; }
    }
}
