using IOfThings.Spatial.Geography;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingSample : IGeofencingSessionComponent, ILocated, ITimed
    {
        /// <summary>
        /// specific tags to be carry on by the pipeline
        /// </summary>
        string[] Tags { get; set; }
        bool Fix { get; set; }
        float? Accuracy { get; set; }
        float? Speed { get; set; }
        float? Heading { get; set; }
    }
}
