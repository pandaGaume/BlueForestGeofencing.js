using System.Collections.Generic;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingFactoryOptions
    {

    }

    public interface IGeofencingFactoryOptionsBuilder
    {
        IGeofencingFactoryOptions Build();
    }

    public interface IGeofencingFactory<TInput, TOptions>
        where TOptions : IGeofencingFactoryOptions
    {
        IEnumerable<IGeofence> Create(TInput source, TOptions options);
    }
}
