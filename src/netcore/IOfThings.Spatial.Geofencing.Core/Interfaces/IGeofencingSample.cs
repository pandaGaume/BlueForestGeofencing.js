using IOfThings.Spatial.Geography;
using IOfThings.Telemetry;
using IOfThings.Telemetry.Dataflow;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingSample : IGeofencingSessionComponent, ILocated, ISequenceable
    {
        /// <summary>
        /// specific tags to be carry on by the pipeline
        /// </summary>
        string[] Tags { get; set; }
        bool Fix { get; set; }
        float? Accuracy { get; set; }
        float? Speed { get; set; }
        float? Heading { get; set; }
        public Quality Quality { get; set; }
    }
}
