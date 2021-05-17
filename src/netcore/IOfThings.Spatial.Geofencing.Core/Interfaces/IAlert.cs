using IOfThings.Spatial.Geofencing.Text.Json;
using IOfThings.Spatial.Geography;
using IOfThings.Telemetry;

namespace IOfThings.Spatial.Geofencing
{
    public static class CommonAlertSeverities
    {
        public static IValueRange<ushort> HIGH = new ValueRange<ushort>(801, 1000);
        public static IValueRange<ushort> MEDIUM_HIGH = new ValueRange<ushort>(601, 800);
        public static IValueRange<ushort> MEDIUM = new ValueRange<ushort>(401, 600);
        public static IValueRange<ushort> MEDIUM_LOW = new ValueRange<ushort>(201, 400);
        public static IValueRange<ushort> LOW = new ValueRange<ushort>(1, 200);
    }

    public static class KnownAlerts
    {
        public const string Entering   = "entering";
        public const string Inside     = "inside";
        public const string Exiting    = "exiting";
        public const string Outside    = "outside";
        public const string Crossing   = "crossing";
        public const string Aproaching = "aproaching";
        public const string Leaving    = "leaving";
        public const string CheckIn = "checkin";
        public const string Above = "above";
        public const string Under = "under";

        public static readonly string[] TypeNames = { Entering, Inside, Exiting, Outside, Crossing, Aproaching, Leaving, CheckIn, Above, Under };
    }

    public enum TriggerType
    {
        Entering,
        Inside,
        Exiting,
        Outside,
        Crossing,
        Approaching,
        Leaving,
        CheckIn,
        Above,
        Under
    }


    [JsonPolymorphicType(Name = "Alert")]
    public interface IAlert : IGeofencingItem
    {
        string[] RelativeTo { get; set; }
        string Category { get; set; }
        ushort Severity { get; set; }
        string Message { get; set; }
        public int TriggerMask { get; set; }
    }
}
