using IOfThings.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    /// <summary>
    /// The Rectangle geometry is not supported by the GeoJSON spec. We use a GeoJSON Polygon Feature object to represent a rectangle.
    /// </summary>
    [JsonPolymorphicType(Name = "Rectangle")]
    public interface IGRectangle : IGeofencingShape
    {
    }
}
