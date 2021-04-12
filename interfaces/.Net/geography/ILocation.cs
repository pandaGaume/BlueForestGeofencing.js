using IOfThings.Spatial.Geography.Text;
using System;

namespace IOfThings.Spatial.Geography
{
    public interface ILocation : ILocated, ICloneable, ISimpleGeometry
    {
        double Latitude { get; }
        double Longitude { get; }
        double? Altitude { get; }
    }
}
