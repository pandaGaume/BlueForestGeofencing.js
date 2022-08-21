using IOfThings.Spatial.Geography;
using System;

namespace IOfThings.Spatial.Geofencing
{
    public static class Severity
    {
        public static IValueRange<ushort> HIGH = new ValueRange<ushort>(801, 1000);
        public static IValueRange<ushort> MEDIUM_HIGH = new ValueRange<ushort>(601, 800);
        public static IValueRange<ushort> MEDIUM = new ValueRange<ushort>(401, 600);
        public static IValueRange<ushort> MEDIUM_LOW = new ValueRange<ushort>(201, 400);
        public static IValueRange<ushort> LOW = new ValueRange<ushort>(1, 200);
    }
    public interface IWithId
    {
        string Id { get; set; }
    }

    public interface IHasDeviceIdentifier
    {
        string DeviceId { get; set; }
    }

    public interface IIndexed
    {
        long? Index { get; set; }
    }

    public interface ITimed
    {
        DateTime When { get; set; }
    }

    public interface IGeofencingEvent : IHasDeviceIdentifier, IIndexed, ITimed, IWithId, ILocated
    {
        string Category { get; set; }
        ushort? Severity { get; set; }
        string Message { get; set; }
        DateTime? Expiration { get; set; }
        string Condition { get; set; }
        string SubCondition { get; set; }
        dynamic State { get; set; }
        bool? AckRequired { get; set; }
        object Cookie { get; set; }
        object ActorId { get; set; }
    }

    public interface IGeofencingEventMessageFactory
    {
        string BuildMessage(IGeofencingEvent e, IGeofencingItem node);
    }
}