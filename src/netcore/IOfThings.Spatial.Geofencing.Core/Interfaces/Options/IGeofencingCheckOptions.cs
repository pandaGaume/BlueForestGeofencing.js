namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingCheckOptions
    {
        IGeofencingEventMessageFactory MessageFactory { get; set; }
        IGeofencingEventFactory EventFactory { get; set; }
    }
}
