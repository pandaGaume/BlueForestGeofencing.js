using IOfThings.Spatial.Geography;
using IOfThings.Telemetry;
using System;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingEventFactory
    {
        string BuildActorId(IGeofence geofence);
        IConditionEvent CreateEvent(string actor, string device, string subject, TriggerType trigger, ILocation where, DateTime when);
    }
}
