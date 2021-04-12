namespace IOfThings.Spatial.GeoFencing
{

    public interface IGeofencingAlert : IGeofencingNode
    {
        string RelativeTo { get; set; }

        string Category { get; set; }

        ushort Severity { get; set; }

        string Message { get; set; }
    }
}
