using System;

namespace IOfThings.Telemetry
{
    public interface IRange<T> : ICloneable
    {
        T From { get; set; }
        T To { get; set; }
    }
}
