using IOfThings.Text.Json;
using System;
using System.Collections.Generic;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Node")]
    public interface ITreeItem 
    {
        IGeofencingNode Parent { get; }
        IEnumerable<IGeofencingNode> Children(Func<IGeofencingNode, bool> predicate = null);
        bool HasChildren { get; }
        int CountChildren { get; }
    }
}
