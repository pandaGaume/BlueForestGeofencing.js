using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingCacheProvider
    {
        /// <summary>
        /// If set, let geofencing item use it for they own state management.
        /// Pattern is to register a specific cache object for each node/asset key pair.
        /// </summary>
        IMemoryCache PrimaryCache { get; }
        /// <summary>
        /// If set, let geofencing item use it for they own state management.
        /// Pattern is to register a specific cache object for each node/asset key pair.
        /// </summary>
        IDistributedCache SecondaryCache { get; }
    }
}
