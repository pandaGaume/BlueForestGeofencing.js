using System;

namespace IOfThings.Spatial.Geofencing.Text.Json
{
    public interface IGeofencingExtensionTypeFactory
    {
        public Type ForName(string typeName);
    }
}
