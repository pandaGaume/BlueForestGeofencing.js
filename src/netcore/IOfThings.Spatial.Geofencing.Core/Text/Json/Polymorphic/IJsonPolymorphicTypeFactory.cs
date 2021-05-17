using System;

namespace IOfThings.Spatial.Geofencing.Text.Json
{
    public interface IJsonPolymorphicTypeFactory
    {
        public Type ForName(string typeName);
    }
}
