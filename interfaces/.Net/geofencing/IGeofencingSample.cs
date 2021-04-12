using IOfThings.Spatial.Geography;
using System;
using System.Collections.Generic;

namespace IOfThings.Spatial.GeoFencing
{
    /// <summary>
    /// The incoming message.
    /// </summary>
    public interface IGeofencingSample : ILocated
    {
        /// <summary>
        /// UTC date time of message. For some reason, the message may has been delayed
        /// </summary>
        DateTime timestamp { get; }
        /// <summary>
        /// UTC date time of the location
        /// </summary>
        DateTime When { get; }
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
