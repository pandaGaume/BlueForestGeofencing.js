using IOfThings.Spatial.Geography;
using IOfThings.Telemetry;
using System;
using System.Collections.Generic;

namespace IOfThings.Spatial.GeoFencing
{
    /// <summary>
    /// The incoming message.
    /// </summary>
    public interface IGeofencingSample : ILocated, ITimed
    {
        /// <summary>
        /// UTC date time of message. For some reason, the message may has been delayed
        /// </summary>
        DateTime timestamp { get; }
 
        /// <summary>
        /// the asset who is binded with this sample
        /// </summary>
        string Who { get; }
 
        /// <summary>
        /// specific tags to be carryng on by the pipeline
        /// </summary>
        IEnumerable<string> Tags { get; }
    }
}
