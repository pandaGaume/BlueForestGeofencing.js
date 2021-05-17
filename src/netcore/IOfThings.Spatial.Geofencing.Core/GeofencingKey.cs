using Microsoft.Extensions.ObjectPool;
using System;

namespace IOfThings.Spatial.Geofencing
{
    public class GeofencingKey : IGeofencingKey
    {
        internal class PoolPolicy : IPooledObjectPolicy<GeofencingKey>
        {
            public GeofencingKey Create() => new GeofencingKey();
            public bool Return(GeofencingKey obj) => true;
        }

        public static ObjectPool<GeofencingKey> SharedPool = ObjectPool.Create<GeofencingKey>(new PoolPolicy());

        string _major;
        string _minor;

        int _hc;

        public GeofencingKey()
        {
        }
        public GeofencingKey(string major, string minor)
        {
            _major = major;
            _minor = minor;
            _hc = HashCode.Combine(_major.GetHashCode(), _minor.GetHashCode());
        }
        public string Id { get => _major; set => _major = value; }
        public string Who { get => _minor; set => _minor = value; }

        public override int GetHashCode() => _hc;

        public override bool Equals(object obj)
        {
            if (obj != null && obj is IGeofencingKey k)
            {
                return _major.Equals(k.Id) && _minor.Equals(k.Who);
            }
            return false;
        }
    }
}
