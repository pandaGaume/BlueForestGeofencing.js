using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Text.GeoJson;

namespace IOfThings.Spatial.Geofencing
{
    public static class PositionExtensions
    {
        public static ILocation ToLocation(this Position p)=> new Location(p.Latitude, p.Longitude, p.Altitude);
        public static ILocation ToLocation(this float[] p)
        {
            double lon  = p[0];
            double lat  = p[1];
            return p.Length > 2 ? new Location(lat, lon, p[2]) : new Location(lat, lon);
        }
    }
}
