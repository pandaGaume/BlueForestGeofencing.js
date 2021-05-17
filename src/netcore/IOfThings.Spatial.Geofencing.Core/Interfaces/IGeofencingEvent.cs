using IOfThings.Telemetry;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingEvent : IConditionEvent
    {
        public TriggerType Trigger { get; set; }
    }
}
