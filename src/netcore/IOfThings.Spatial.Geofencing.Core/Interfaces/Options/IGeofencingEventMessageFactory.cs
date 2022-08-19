using IOfThings.Telemetry;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingEventMessageFactory
    {
        string BuildMessage(IConditionEvent e, IGeofencingItem node);
    }
}
