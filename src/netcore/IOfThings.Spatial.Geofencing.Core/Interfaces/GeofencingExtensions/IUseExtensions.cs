using System.Collections.Generic;

namespace IOfThings.Spatial.Geofencing
{
    public interface IUseExtensions
    {
        IList<string> ExtensionsUsed { get; set; }
        IList<string> ExtensionsRequired { get; set; }
    }
}
