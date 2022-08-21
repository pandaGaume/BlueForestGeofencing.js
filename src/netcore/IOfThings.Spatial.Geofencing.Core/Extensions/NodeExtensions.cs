using IOfThings.Spatial.Geography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public static class NodeExtensions
    {
        public static IEnumerable<IGeofencingNode> Descendants(this IGeofencingNode n, Func<IGeofencingNode, Boolean> predicate = null)
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
        public static IEnumerable<IGeofencingNode> Descendants(this IEnumerable<IGeofencingNode> nodes, Func<IGeofencingNode, Boolean> predicate = null)=>nodes.SelectMany(n => n.Descendants(predicate));
        public static int Index(this IGeofencingNode node)
        {
            var g = node.Geofence;
            return g != default(IGeofence)? g.Nodes.IndexOf(node) : -1;
        }
        public static IEnumerable<IPrimitive> Primitives(this IGeofencingNode node)
        {
            var g = node.Geofence;
            var i = Index(node);
            if( i != -1)
            {
                return g.Primitives.Where(p => p.INodes.Contains(i));
            }
            return Enumerable.Empty<IPrimitive>();
        }
        public static IEnumerable<IPrimitive> Primitives(this IEnumerable<IGeofencingNode> nodes) => nodes.SelectMany(n => Primitives(n)).Distinct();
        public static IEnumerable<IGeofencingEvent> Check(this IGeofencingNode node, IPrimitive primitive, ISegment<IGeofencingSample> sample, IGeofencingEventFactory eventFactory)
        {
            // sort case where unconsistent segment
            if (sample.First == default(IGeofencingSample))
            {
                if (sample.Second == default(IGeofencingSample))
                {
                    return Enumerable.Empty<IGeofencingEvent>();
                }
                return CheckInternal(node, primitive, sample.Second, eventFactory);
            }

            if (sample.Second == default(IGeofencingSample) || sample.First == sample.Second)
            {
                return CheckInternal(node, primitive, sample.First, eventFactory);
            }

            return CheckInternal(node, primitive, sample, eventFactory);
        }
        public static IEnumerable<IGeofencingEvent> Check(this IEnumerable<IGeofencingNode> nodes, IPrimitive primitive, ISegment<IGeofencingSample> sample, IGeofencingEventFactory eventFactory)
        {
            // sort case where unconsistent segment
            if (sample.First == default(IGeofencingSample))
            {
                if (sample.Second == default(IGeofencingSample))
                {
                    return Enumerable.Empty<IGeofencingEvent>();
                }
                return CheckInternal(nodes, primitive, sample.Second, eventFactory);
            }

            if (sample.Second == default(IGeofencingSample) || sample.First == sample.Second)
            {
                return CheckInternal(nodes, primitive, sample.First, eventFactory);
            }

            return CheckInternal(nodes, primitive, sample, eventFactory);
        }
        internal static IEnumerable<IGeofencingEvent> CheckInternal(this IGeofencingNode node, IPrimitive primitive, ISegment<IGeofencingSample> sample, IGeofencingEventFactory eventFactory)
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
                            return shape.CheckImpl(primitive, node, sample, eventFactory);
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
            return Enumerable.Empty<IGeofencingEvent>();
        }
        internal static IEnumerable<IGeofencingEvent> CheckInternal(this IEnumerable<IGeofencingNode> nodes, IPrimitive primitive, ISegment<IGeofencingSample> sample, IGeofencingEventFactory eventFactory) => nodes.SelectMany(n => CheckInternal(n, primitive, sample, eventFactory));
        internal static IEnumerable<IGeofencingEvent> CheckInternal(this IGeofencingNode node, IPrimitive primitive, IGeofencingSample sample, IGeofencingEventFactory eventFactory)
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
                            return shape.CheckImpl(primitive, node, sample,eventFactory);
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
            return Enumerable.Empty<IGeofencingEvent>();
        }
        internal static IEnumerable<IGeofencingEvent> CheckInternal(this IEnumerable<IGeofencingNode> nodes, IPrimitive primitive, IGeofencingSample sample, IGeofencingEventFactory eventFactory) => nodes.SelectMany(n=>CheckInternal(n,primitive,sample, eventFactory));
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
            // fast track
            if (n.Scale == Vector3.One && n.Rotation ==  0)
            {
                return n.Position == Vector3.Zero ? Matrix4x4.Identity : Matrix4x4.CreateTranslation(n.Position);
            }
            // compose the matrix.
            var s = Matrix4x4.CreateScale(n.Scale);
            var r = Matrix4x4.CreateRotationZ(n.Rotation);
            var t = Matrix4x4.CreateTranslation(n.Position);
            var pivot = n.GetPivot();
            if (pivot != null &&  pivot.Value != Vector3.Zero)
            {
                var t0 = Matrix4x4.CreateTranslation(-pivot.Value);
                var t1 = Matrix4x4.CreateTranslation(pivot.Value);
                return t0 * s * r * t * t1;
            }
            return s  * r * t;
        }
        public static Vector3? GetPivot(this IGeofencingNode n)
        {
            // we have pivot defined
            if (n.Pivot.HasValue)
            {
                return n.Pivot.Value;
            }
            var p = n.GetShape()?.GetPivot();
            return p;
        }
        public static Matrix4x4 BakeWorldTransform(this IGeofencingNode n)
        {
            return n.Parent != null ? n.LocalTransform * n.Parent.WorldTransform  : n.LocalTransform ;
        }
        public static bool TryGetParent(this IGeofencingNode n, out IGeofencingNode parent)
        {
            var i = n.Geofence.Nodes.IndexOf(n);
            parent = default;
            if (i >= 0)
            {
                foreach (var p in n.Geofence.Nodes)
                {
                    if (p.IChildren?.Contains(i)??false)
                    {
                        parent = p;
                        return true;
                    }
                }
            }
            return false;
        }
        public static void Enable(this IGeofencingNode node) => SetEnable(node, true);
        public static void Disable(this IGeofencingNode node) => SetEnable(node, false);
        public static void SetEnable(this IGeofencingNode node, bool e)
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
        public static void Consume(this IGeofencingNode node)
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
        public static IEnvelope BuildEnvelope(this IGeofencingNode node)
        {
            var s = node.GetShape();
            IEnvelope env = null;
            if (s != default(IGeofencingShape))
            {
                env = s.Envelope;
                var T = node.WorldTransform;
                env = (T.IsIdentity) ? env : env.Transform(T);
            }

            env = env ?? new Envelope();
            if (node.HasChildren)
            {
                foreach (var c in node.Children())
                {
                    var e = c.Envelope;
                    env.AddInPlace(e);
                }
            }
            return env;
        }
    }
}
