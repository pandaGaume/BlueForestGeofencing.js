using IOfThings.Spatial.Geography;
using System;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingSampleFactory<T>
        where T : IGeofencingSample
    {
        T CreateGeofencingSample(string who, ILocation where, DateTime when, long? index = null);
    }
}
