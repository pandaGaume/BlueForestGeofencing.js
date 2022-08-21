using IOfThings.Spatial.Geography;
using System;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingEventFactory
    {
        string BuildActorId(IGeofence geofence);
        IGeofencingEvent CreateEvent(string actor, string device, string subject, TriggerType trigger, ILocation where, DateTime when);
    }
}
