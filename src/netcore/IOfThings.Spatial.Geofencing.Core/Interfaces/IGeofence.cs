using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Text.GeoJson;
using IOfThings.Text.Json;
using System.Collections.Generic;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Geofence")]
    public interface IGeofence : IGeofencingItem, IWithIdentity, IGeoBounded, IUseExtensions, IGeofencingCacheProvider, IGeofencingLogProvider
    {
        string Namespace { get; set; }
        string Comment { get; set; }
        IGeoJsonObject Geometry {get;set;}
        EllipticSystem GeodeticSystem { get; set; }
        int[] RootIndices { get; set; }
        IList<IGeofencingNode> Nodes { get; }
        IList<IGeofencingShape> Shapes { get; }
        IList<IModifier> Modifiers { get; }
        IList<IAlert> Alerts { get; }
        IList<IPrimitive> Primitives { get; }
    }
}
