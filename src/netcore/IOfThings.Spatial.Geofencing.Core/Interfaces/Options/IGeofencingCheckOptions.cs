using System;
using System.Collections.Generic;
using System.Text;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingCheckOptions
    {
        IGeofencingEventMessageFactory MessageFactory { get; set; }
    }
}
