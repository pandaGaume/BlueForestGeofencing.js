using IOfThings.Spatial.Geography;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public static class NodeExtensions
    {
        public static IEnumerable<IGeofencingEvent> Apply(this IEnumerable<INode> nodes, ISegment<IGeofencingSample> segment) => nodes.SelectMany(n => n.Apply(segment));
        public static IEnumerable<IGeofencingEvent> Apply(this IEnumerable<INode> nodes, IGeofencingSample location) => nodes.SelectMany(n => n.Apply(location));

        public static IEnumerable<IGeofencingEvent> Apply(this INode node, ISegment<IGeofencingSample> segment)
        {
            if(segment.Second == null || segment.First == segment.Second )
            {
                return Apply(node, segment.First);
            }
            
            if (segment.First == null)
            {
                return Apply(node, segment.Second);
            }

            return Enumerable.Empty<IGeofencingEvent>();
        }
        public static IEnumerable<IGeofencingEvent> Apply(this INode node, IGeofencingSample location)
        {
            return Enumerable.Empty<IGeofencingEvent>();
        }

        public static void Invalidate(this IEnumerable<INode> nodes)
        {
            foreach (var n in nodes) n.Invalidate();
        }

        public static IShape GetShape(this INode n) => n.Geofence?.Shapes?[n.IShape];
        public static Matrix4x4 BakeLocalTransform(this INode n)
        {
            var t = Matrix4x4.CreateTranslation(n.Translation);
            var r = Matrix4x4.CreateFromQuaternion(n.Rotation);
            var s = Matrix4x4.CreateScale(n.Scale);
            return t * r * s;
        }
        public static Matrix4x4 BakeGlobalTransform(this ITreeNode n)
        {
            return n.Parent != null ? n.Parent.GlobalTransform * n.Transform : n.Transform ;
        }

        public static bool TryGetParent(this ITreeNode n, out ITreeNode parent)
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

        public static T GetSampleContext<T>(this INode node, IGeofencingSample sample)
            where T : IGeofencingContext
        {
            if (sample.Cache != null)
            {
                var key = GeofencingKey.SharedPool.Get();
                key.Id = node.Id;
                key.Who = sample.Who;
                try
                {
                    if (sample.Cache.TryGetValue(key, out var context))
                    {
                        return (T)context;
                    }
                }
                finally
                {
                    GeofencingKey.SharedPool.Return(key);
                }
            }
            return default;
        }
        public static void CreateSampleContext<T>(this INode node, IGeofencingSample sample, T context)
            where T : IGeofencingContext
        {
            if (sample.Cache != null)
            {
                var key = new GeofencingKey(node.Id, sample.Who);
                var e = sample.Cache.CreateEntry(key);
                e.Value = context;
            }
        }

        public static bool IsEnabled(this ITreeNode node)
        {
            if (!node.Enabled) return false;
            var n = node.Parent;
            if (n != null)
            {
                do
                {
                    if (!n.Enabled) return false;
                    n = node.Parent;
                } while (n != null);
            }
            return true;
        }

        public static bool IsConsumed(this ITreeNode node)
        {
            if (!node.Consumed) return false;
            var n = node.Parent;
            if (n != null)
            {
                do
                {
                    if (!n.Consumed) return false;
                    n = node.Parent;
                } while (n != null);
            }
            return true;
        }

        public static void Enable(this ITreeNode node) => SetEnable(node, true);
        public static void Disable(this ITreeNode node) => SetEnable(node, false);
        public static void SetEnable(this ITreeNode node, bool e)
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
        public static void Consume(this ITreeNode node)
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
