using IOfThings.Spatial.Geography;
using IOfThings.Telemetry;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingSample : ILocated, ITimed
    {
        /// <summary>
        /// the asset who is binded with this sample
        /// </summary>
        string Who { get; }

        /// <summary>
        /// specific tags to be carry on by the pipeline
        /// </summary>
        string[] Tags { get; }

        /// <summary>
        /// If set, let geofencing item use it for they own state management.
        /// Pattern is to register a specific cache object for each node/asset key pair.
        /// </summary>
        ICache<IGeofencingKey, IGeofencingContext> Cache { get; }
    }
}
