using IOfThings.Spatial.Geofencing.Text.Json;
using IOfThings.Spatial.Geography;
using System;

namespace IOfThings.Spatial.Geofencing
{
    public class ModifierException : Exception
    {
        readonly IModifier _modifier;
        public ModifierException(IModifier modifier)
        {
            _modifier = modifier;
        }

        public IModifier Modifier => _modifier;
    }

    public class RejectedException : ModifierException 
    {
        readonly ILocated _input;
        public RejectedException(IModifier modifier, ILocated input) : base(modifier)
        {
            _input = input;
        }
        public ILocated Input => _input;
    }

    public enum ModifierType
    {
        Calendar,
        Predicate,
    }

    public interface IModifier : IGeofencingItem, IWithPriority, IWithFilter<ILocated>
    {
        string Category { get; set; }
        void Apply(ILocated data, IGeofencingItem target);
    }
}
