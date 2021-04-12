using System;

namespace IOfThings.Telemetry
{
    public static class Severity
    {
        public static IRange<ushort> HIGH = new Range<ushort>(801, 1000);
        public static IRange<ushort> MEDIUM_HIGH = new Range<ushort>(601, 800);
        public static IRange<ushort> MEDIUM = new Range<ushort>(401, 600);
        public static IRange<ushort> MEDIUM_LOW = new Range<ushort>(201, 400);
        public static IRange<ushort> LOW = new Range<ushort>(1, 200);
    }

    public enum EventType
    {
        simple, condition, tracking
    }

    public interface ISimpleEvent : IIndexed, ITimed
    {
        string Source { get; set; }
        EventType Type { get; }
        string Category { get; set; }
        ushort? Severity { get; set; }
        string Message { get; set; }
        DateTime? Expiration { get; set; }
    }

    public interface IConditionEvent : ISimpleEvent
    {
        string Condition { get; set; }
        string SubCondition { get; set; }
        dynamic State { get; set; }
        bool? AckRequired { get; set; }
        object Cookie { get; set; }
    }

    public interface ITrackingEvent : ISimpleEvent
    {
        object ActorID { get; set; }
    }
}