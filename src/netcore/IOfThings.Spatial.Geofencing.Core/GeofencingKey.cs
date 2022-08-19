using Microsoft.Extensions.ObjectPool;
using System;
using System.Diagnostics.CodeAnalysis;

namespace IOfThings.Spatial.Geofencing
{
    public class GeofencingKey : IGeofencingKey, IEquatable<IGeofencingKey>, ICloneable
    {
        internal class PoolPolicy : IPooledObjectPolicy<GeofencingKey>
        {
            public GeofencingKey Create() => new GeofencingKey();
            public bool Return(GeofencingKey obj) => true;
        }

        public static ObjectPool<GeofencingKey> SharedPool = ObjectPool.Create<GeofencingKey>(new PoolPolicy());

        string _major;
        string _minor;
        int _version;
        int _hc;

        public GeofencingKey()
        {
        }
        public GeofencingKey(string major, string minor, int version = 0)
        {
            _major = major ?? throw new ArgumentNullException(nameof(major));
            _minor = minor ?? throw new ArgumentNullException(nameof(minor));
            _version = version;
            _hc = HashCode.Combine(_major.GetHashCode(), _minor.GetHashCode(), _version);
        }
        public string Id { get => _major; 
            set{ 
                _major = value?? throw new ArgumentNullException(nameof(value)); 
                _hc = HashCode.Combine(_major.GetHashCode(), _minor?.GetHashCode()??0, _version); 
            } 
        }
        public string Who { get => _minor; 
            set{ 
                _minor = value ?? throw new ArgumentNullException(nameof(value));  
                _hc = HashCode.Combine(_major?.GetHashCode()??0, _minor.GetHashCode(), _version); 
            } 
        }

        public int Version
        {
            get => _version;
            set
            {
                _version = value ;
                _hc = HashCode.Combine(_major?.GetHashCode() ?? 0, _minor?.GetHashCode()?? 0, _version);
            }
        }
        public override int GetHashCode() => _hc;

        public override bool Equals(object obj)
        {
            if (obj != null && obj is IGeofencingKey k)
            {
                return _version == k.Version && _major.Equals(k.Id) && _minor.Equals(k.Who);
            }
            return false;
        }

        public bool Equals([AllowNull] IGeofencingKey other)
        {
            return other != null && _hc != other.GetHashCode() && _version == other.Version && _major.Equals(other.Id) && _minor.Equals(other.Who);
        }

        public object Clone()
        {
            return new GeofencingKey(_major, _minor, _version);
        }
    }
}
