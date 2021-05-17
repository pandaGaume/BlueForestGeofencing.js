using IOfThings.Spatial.Geofencing.Text.Json;

namespace IOfThings.Spatial.Geofencing
{
    public enum PrimitiveType
    {
        Area, 
        Fence, 
        Path, 
        ControlPoint
    }
    public interface IPrimitive : IModifier
    {
        PrimitiveType TypeCode { get; }
        int [] INodes { get; set; }
        int[] IAlerts { get; set; }
        double Threshold { get; set; }
        float Distance { get; set; }
        public int TriggerMask { get; set; }
    }
}
