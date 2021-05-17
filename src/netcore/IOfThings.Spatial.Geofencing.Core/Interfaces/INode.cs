using IOfThings.Spatial.Geofencing.Text.Json;
using IOfThings.Spatial.Geography;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public interface INode : IGeofencingItem, IGeoBounded, IWithIdentity, IBounded
    {
        int IShape { get; set; }
        int[] ChildrenIndices { get; set; }
        Vector3 Translation { get; set; }
        Vector3 Scale { get; set; }
        Quaternion Rotation { get; set; }
        public Matrix4x4 Transform { get; }
        public Matrix4x4 GlobalTransform { get; }
        void Validate();
        void Invalidate();
    }
}
