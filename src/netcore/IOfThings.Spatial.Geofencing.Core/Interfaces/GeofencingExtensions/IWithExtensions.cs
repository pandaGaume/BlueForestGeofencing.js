using System.Collections.Generic;

namespace IOfThings.Spatial.Geofencing
{
    public interface IWithExtensions
    {
        IList<IGeofencingExtension> Extensions { get; set; }
    }
}
