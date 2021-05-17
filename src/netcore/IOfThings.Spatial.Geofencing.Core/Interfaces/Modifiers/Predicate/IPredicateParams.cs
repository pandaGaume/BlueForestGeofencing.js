namespace IOfThings.Spatial.Geofencing
{
    public interface IPredicateParams
    {
        public IGeofencingSample Input { get; set; }
        public IGeofencingItem Target { get; set; }
    }
}
