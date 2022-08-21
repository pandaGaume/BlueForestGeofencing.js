using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public interface ITransformable 
    {
        Vector3 Scale { get; set; }
        float Rotation { get; set; }
        Vector3? Pivot { get; set; }
        public Matrix4x4 LocalTransform { get; }
        public Matrix4x4 WorldTransform { get; }
    }
}
