using IOfThings.Spatial.Geofencing.Text.Json;
using System;
using System.Collections.Generic;

namespace IOfThings.Spatial.Geofencing
{
    [JsonPolymorphicType(Name = "Node")]
    public interface ITreeNode : INode
    {
        ITreeNode Parent { get; }
        IEnumerable<ITreeNode> Children(Func<ITreeNode, bool> predicate = null);
        bool HasChildren { get; }
        int CountChildren { get; }
    }
}
