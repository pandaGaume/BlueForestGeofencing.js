using System;
using System.Collections.Generic;
using System.Text;

namespace IOfThings.Spatial.GeoFencing
{
    public class GeofencingKnownType
    {
        public const string GeoCircle  = "geoCircle";
        public const string GeoRect    = "geoRect";
        public const string GeoArea    = "geoArea";
        public const string GeoSegment = "geoSegment";
        public const string GeoPath    = "geoPath";
    }

    public interface IGeofencingNode
    {
        string Id { get; set; }
        bool Enabled { get; set; }
        bool Consumed { get; set; }
        string DisplayName { get; set; }
        string Description { get; set; }
        List<string> Tags { get; }
        List<IGeofencingModifier> Modifiers { get;  }
    }
}
