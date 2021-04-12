using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Geography.Text;
using System;

namespace IOfThings.Spatial.GeoFencing
{
    public interface IGeofence : IGeoJsonFeature<IGeofencingPrimitive>, IObserver<IGeofencingSample>, IObserver<ISegment<IGeofencingSample>>, IObservable<IGeofencingEvent>
    {

    }
}
