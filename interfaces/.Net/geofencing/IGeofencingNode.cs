using System.Collections.Generic;

namespace IOfThings.Spatial.GeoFencing
{
    public class GeofencingKnownType
    {
        public const string Namespace  = "BlueForest.Geofencing";
        public const string PreferredPrefix  = "bfgf";

#region Geometry
        public const string GeoCircle  = "geoCircle";
        public const string GeoRect    = "geoRect";
        public const string GeoArea    = "geoArea";
        public const string GeoSegment = "geoSegment";
        public const string GeoPath    = "geoPath";
        public const string GeoCylinder    = "geoCylinder";
        public const string GeoBox    = "geoBox";
        public const string GeoPlane    = "geoPlane";
        public const string GeoMesh    = "geoMesh";
#endregion

#region Modifier
       public const string GeoExpiration = "geoExpiration";
       public const string GeoValidityPeriods = "geoValidityPeriods";

#endregion

#region Alert
      public const string GeoAlert = "geoAlert";
#endregion
    }

    public interface IGeofencingNode
    {
        string Id { get; set; }
        string GeofenceType{ get;set;}
        bool Enabled { get; set; }
        bool Consumed { get; set; }
        string DisplayName { get; set; }
        string Description { get; set; }
        List<string> Tags { get; }
        List<IGeofencingModifier> Modifiers { get;  }
    }
    public interface IWithPriority
    {
        byte Priority { get; set; }
    }
}
