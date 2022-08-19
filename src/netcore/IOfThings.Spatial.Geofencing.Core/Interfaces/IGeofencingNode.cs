using IOfThings.Spatial.Geography;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingNode : IGeofencingItem, IGeoBounded, IWithIdentity
    {
        int IShape { get; set; }
        int[] ChildrenIndices { get; set; }
        Vector3 Translation { get; set; }
        Vector3 Scale { get; set; }
        Quaternion Rotation { get; set; }
        public Matrix4x4 LocalTransform { get; }
        public Matrix4x4 WorldTransform { get; }
    }
}
