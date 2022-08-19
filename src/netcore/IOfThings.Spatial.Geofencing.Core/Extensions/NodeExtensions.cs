using IOfThings.Spatial.Geography;
using IOfThings.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public static class NodeExtensions
    {
        public static IEnumerable<IGeofencingTreeNode> Descendants(this IGeofencingTreeNode n, Func<IGeofencingTreeNode, Boolean> predicate = null)
        {
            foreach (var c in n.Children(predicate))
            {
                yield return c;
                foreach (var cc in c.Descendants(predicate))
                {
                    yield return cc;
                }
            }
        }
        public static IEnumerable<IGeofencingTreeNode> Descendants(this IEnumerable<IGeofencingTreeNode> nodes, Func<IGeofencingTreeNode, Boolean> predicate = null)
        {
            return nodes.SelectMany(n => n.Descendants(predicate));
        }
        public static int Index(this IGeofencingTreeNode node)
        {
            var g = node.Geofence;
            return g != default(IGeofence)? g.Nodes.IndexOf(node) : -1;
        }
        public static IEnumerable<IPrimitive> Primitives(this IGeofencingTreeNode node)
        {
            var g = node.Geofence;
            var i = Index(node);
            if( i != -1)
            {
                return g.Primitives.Where(p => p.INodes.Contains(i));
            }
            return Enumerable.Empty<IPrimitive>();
        }
        public static IEnumerable<IPrimitive> Primitives(this IEnumerable<IGeofencingTreeNode> nodes) => nodes.SelectMany(n => Primitives(n)).Distinct();
        public static IEnumerable<IConditionEvent> Check(this IGeofencingNode node, IPrimitive primitive, ISegment<IGeofencingSample> sample)
        {
            // sort case where unconsistent segment
            if (sample.First == default(IGeofencingSample))
            {
                if (sample.Second == default(IGeofencingSample))
                {
                    return Enumerable.Empty<IConditionEvent>();
                }
                return CheckInternal(node, primitive, sample.Second);
            }

            if (sample.Second == default(IGeofencingSample) || sample.First == sample.Second)
            {
                return CheckInternal(node, primitive, sample.First);
            }

            return CheckInternal(node, primitive, sample);
        }
        public static IEnumerable<IConditionEvent> Check(this IEnumerable<IGeofencingNode> nodes, IPrimitive primitive, ISegment<IGeofencingSample> sample)
        {
            // sort case where unconsistent segment
            if (sample.First == default(IGeofencingSample))
            {
                if (sample.Second == default(IGeofencingSample))
                {
                    return Enumerable.Empty<IConditionEvent>();
                }
                return CheckInternal(nodes, primitive, sample.Second);
            }

            if (sample.Second == default(IGeofencingSample) || sample.First == sample.Second)
            {
                return CheckInternal(nodes, primitive, sample.First);
            }

            return CheckInternal(nodes, primitive, sample);
        }
        internal static IEnumerable<IConditionEvent> CheckInternal(this IGeofencingNode node, IPrimitive primitive, ISegment<IGeofencingSample> sample)
        {
            try
            {
                // note : modifiers are already sorted into GetPreModifiers
                if (primitive.GetPreModifiers<IModifier>().ApplyAll(sample, primitive, node))
                {
                    var shape = node.GetShape();
                    try
                    {
                        if (shape.GetPreModifiers<IModifier>().ApplyAll(sample, shape, node))
                        {
                            return shape.CheckImpl(primitive, node, sample);
                        }
                    }
                    finally
                    {
                        shape.GetPostModifiers<IModifier>().ApplyAll(sample, shape, node);
                    }
                }
            }
            finally
            {
                primitive.GetPostModifiers<IModifier>().ApplyAll(sample, primitive, node);
            }
            return Enumerable.Empty<IConditionEvent>();
        }

        internal static IEnumerable<IConditionEvent> CheckInternal(this IEnumerable<IGeofencingNode> nodes, IPrimitive primitive, ISegment<IGeofencingSample> sample) => nodes.SelectMany(n => CheckInternal(n, primitive, sample));

        internal static IEnumerable<IConditionEvent> CheckInternal(this IGeofencingNode node, IPrimitive primitive, IGeofencingSample sample)
        {
            try
            {
                // note : modifiers are already sorted into GetPreModifiers
                if (primitive.GetPreModifiers<IModifier>().ApplyAll(sample, primitive, node))
                {
                    var shape = node.GetShape();
                    try
                    {
                        if (shape.GetPreModifiers<IModifier>().ApplyAll(sample, shape, node))
                        {
                            return shape.CheckImpl(primitive, node, sample);
                        }
                    }
                    finally
                    {
                        shape.GetPostModifiers<IModifier>().ApplyAll(sample, shape, node);
                    }
                }
            }
            finally
            {
                primitive.GetPostModifiers<IModifier>().ApplyAll(sample, primitive, node);
            }
            return Enumerable.Empty<IConditionEvent>();
        }
        internal static IEnumerable<IConditionEvent> CheckInternal(this IEnumerable<IGeofencingNode> nodes, IPrimitive primitive, IGeofencingSample sample) => nodes.SelectMany(n=>CheckInternal(n,primitive,sample));

        public static IGeofencingShape GetShape(this IGeofencingNode n)
        {
            var shapes = n.Geofence?.Shapes;
            if(n.IShape < 0 || shapes == null || n.IShape >= shapes.Count)
            {
                return default(IGeofencingShape);
            }
            return shapes[n.IShape];
        }
        public static Matrix4x4 BakeLocalTransform(this IGeofencingNode n)
        {
            var s = Matrix4x4.CreateScale(n.Scale);
            var r = Matrix4x4.CreateFromQuaternion(n.Rotation);
            var t = Matrix4x4.CreateTranslation(n.Translation);
            if ( s.IsIdentity && r.IsIdentity)
            {
                return t;
            }
            var env = n.GetShape()?.BuildEnvelope();
            if (env != null)
            {
                var pivot = env.GetCenter(); ;
                if (pivot != null)
                {
                    var pv = new Vector3((float)pivot.Longitude, (float)pivot.Latitude, (float)(pivot.Altitude.HasValue ? pivot.Altitude.Value : 0));
                    var t0 = Matrix4x4.CreateTranslation(-pv);
                    var t1 = Matrix4x4.CreateTranslation(pv);
                    return t0 * t * r * s * t1;
                }
            }
            return s*r*t;
        }
        public static Matrix4x4 BakeWorldTransform(this IGeofencingTreeNode n)
        {
            return n.Parent != null ? n.Parent.WorldTransform * n.LocalTransform : n.LocalTransform ;
        }
        public static bool TryGetParent(this IGeofencingTreeNode n, out IGeofencingTreeNode parent)
        {
            var i = n.Geofence.Nodes.IndexOf(n);
            parent = default;
            if (i >= 0)
            {
                foreach (var p in n.Geofence.Nodes)
                {
                    if (p.ChildrenIndices?.Contains(i)??false)
                    {
                        parent = p;
                        return true;
                    }
                }
            }
            return false;
        }

        public static void Enable(this IGeofencingTreeNode node) => SetEnable(node, true);
        public static void Disable(this IGeofencingTreeNode node) => SetEnable(node, false);
        public static void SetEnable(this IGeofencingTreeNode node, bool e)
        {
            if (node.Enabled != e)
            {
                node.Enabled = e;
                foreach (var c in node.Children())
                {
                    c.SetEnable(e);
                }
            }
        }
        public static void Consume(this IGeofencingTreeNode node)
        {
            if (!node.Consumed)
            {
                node.Consumed = true;
                foreach (var c in node.Children())
                {
                    c.Consume();
                }
            }
        }
    }
}
