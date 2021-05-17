using IOfThings.Spatial.Geofencing.Text.Json;
using IOfThings.Spatial.Geography;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IOfThings.Spatial.Geofencing
{
    public interface IWithFilter<T>
    {
        bool Accept(T mess);
    }

    public interface IWithPriority
    {
        byte Priority { get; set; }
    }
    public interface IWithIdentity
    {
        string Id { get; set; }
    }

    public interface IGeofencingItem
    {
        string Type { get; }
        IGeofence Geofence { get; set; }
        bool Consumed { get; set; }
        bool Enabled { get; set; }
        string DisplayName { get; set; }
        string Description { get; set; }
        string[] Tags { get; set; }
        int[] PreModifierIndices { get; set; }
        int[] PostModifierIndices { get; set; }
    }

}
