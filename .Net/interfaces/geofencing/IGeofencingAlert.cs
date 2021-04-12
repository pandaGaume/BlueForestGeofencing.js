namespace IOfThings.Spatial.GeoFencing
{
    public enum AlertType
    {

    }

    public interface IGeofencingAlert : IGeofencingNode
    {
        AlertType RelativeTo { get; set; }

        string Category { get; set; }

        ushort Severity { get; set; }

        string Message { get; set; }
    }
}
