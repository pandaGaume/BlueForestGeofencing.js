namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingKey
    {
        /// <summary>
        /// used to register the global geofencing item id
        /// </summary>
        string Id { get; }

        /// <summary>
        /// used to register the asset id ie- Who
        /// </summary>
        string Who { get; }

        /// <summary>
        /// used to keep versioning of the cached item
        /// </summary>
        int Version { get; }
    }
}
