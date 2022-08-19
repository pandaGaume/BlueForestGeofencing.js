namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingSessionComponent : IGeofencingCacheProvider
    {
        /// <summary>
        /// the asset who is binded with this session
        /// </summary>
        string Who { get; }
    }
}
