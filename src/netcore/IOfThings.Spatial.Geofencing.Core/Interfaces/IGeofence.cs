using IOfThings.Spatial.Geofencing.Text.Json;
using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Text.GeoJson;
using System.Collections.Generic;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Geofence")]
    public interface IGeofence : IGeofencingItem, IWithIdentity, IGeoBounded
    {
        string Comment { get; set; }
        IGeoJsonGeometry Geometry {get;set;}
        EllipticSystem GeodeticSystem { get; set; }
        int[] RootIndices { get; set; }
        IList<ITreeNode> Nodes { get; }
        IList<IShape> Shapes { get; }
        IList<IModifier> Modifiers { get; }
        IList<IAlert> Alerts { get; }
        IList<IPrimitive> Primitives { get; }
        IList<ITreeNode> Roots { get; set; }
    }
}
