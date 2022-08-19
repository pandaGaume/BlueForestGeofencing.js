using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingLogProvider
    {
        ILogger Logger { get; }
    }
}
