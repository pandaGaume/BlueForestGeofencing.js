using System;
using System.Collections.Generic;

namespace IOfThings.Spatial.Geofencing
{
    public interface IWithPeriods
    {
        List<Period> Periods { get; set; }
    }

    public interface IWithPriority
    {
        byte Priority { get; set; }
    }
    public interface IWithIdentity
    {
        string Id { get; set; }
    }

    public interface IGeofencingItem : IWithExtensions
    {
        event EventHandler<bool> StatusChanged;

        string Type { get; }
        IGeofence Geofence { get; set; }
        bool Consumed { get; set; }
        bool Enabled { get; set; }
        string DisplayName { get; set; }
        string Description { get; set; }
        string[] Tags { get; set; }
        int[] PreModifierIndices { get; set; }
        int[] PostModifierIndices { get; set; }
        void Invalidate(bool forward = false);
        void Validate();
    }
}
