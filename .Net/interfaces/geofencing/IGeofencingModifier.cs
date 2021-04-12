using IOfThings.Spatial.Geography;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOfThings.Spatial.GeoFencing
{
    public class ModifierException : Exception 
    {
        IGeofencingModifier _modifier;
        public ModifierException(IGeofencingModifier modifier)
        {
            _modifier = modifier;
        }

        public IGeofencingModifier Modifier => _modifier;
    }

    public class SampleRejectedException : ModifierException 
    {
        IGeofencingSample _sample;
        public SampleRejectedException(IGeofencingModifier modifier, IGeofencingSample sample) : base(modifier)
        {
            _sample = sample;
        }

        public IGeofencingSample Sample => _sample;
    }

    public enum ModifierScope
    {
        @static, asset
    }

    public interface IGeofencingModifier : IGeofencingNode
    {
        ModifierScope Scope { get; set; }
        string Category { get; set; }
        void Apply(IGeofencingPrimitive parent, IGeofencingSample value);
    }
}
