using System;

namespace IOfThings.Spatial.Geofencing
{
    public interface IExpirable
    {
        DateTime? AbsoluteExpiration { get; set; }
    }
}
