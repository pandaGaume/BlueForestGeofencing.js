using IOfThings.Spatial.Geography;
using System;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingSampleFactory
    {
        IGeofencingSample CreateGeofencingSample(string who, ILocation where, DateTime when, long? index = null);
    }
}
