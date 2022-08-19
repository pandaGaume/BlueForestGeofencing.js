using IOfThings.Spatial.Geography;
using IOfThings.Telemetry;
using System;

namespace IOfThings.Spatial.Geofencing
{
    public sealed class GeofencingEventFactory : IGeofencingEventFactory
    {
        public static IGeofencingEventFactory Shared = new GeofencingEventFactory();

        internal GeofencingEventFactory() { }
        public string BuildActorId(IGeofence geofence)
        {
            return geofence.Namespace != null ? $"{geofence.Namespace}:{geofence.Id}" : geofence.Id;
        }
        public IConditionEvent CreateEvent(string actor, string device, string subject, TriggerType trigger, ILocation where, DateTime when)
        {
            return new ConditionEvent()
            {
                Id = Guid.NewGuid().ToString(),
                DeviceId = device,
                When = when,
                Where = where,
                Condition = trigger.ToString(),
                SubCondition = subject,
                ActorId = actor
            };
        }
    }
}
