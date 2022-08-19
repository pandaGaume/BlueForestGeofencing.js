using IOfThings.Text.Json;
using System;
using System.Collections.Generic;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Node")]
    public interface IGeofencingTreeNode : IGeofencingNode
    {
        IGeofencingTreeNode Parent { get; }
        IEnumerable<IGeofencingTreeNode> Children(Func<IGeofencingTreeNode, bool> predicate = null);
        bool HasChildren { get; }
        int CountChildren { get; }
    }
}
