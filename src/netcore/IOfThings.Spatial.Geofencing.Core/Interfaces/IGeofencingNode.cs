using IOfThings.Spatial.Geography;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public interface IGeofencingNode : IGeofencingItem, IGeoBounded, IWithIdentity, ITransformable, ITreeItem
    {
        int IShape { get; set; }
        int[] IChildren { get; set; }
    }
}
