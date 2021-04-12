using IOfThings.Spatial.Geography;
using System;
using System.Collections.Generic;

namespace IOfThings.Telemetry
{
    public enum SerieItemType
    {
        datachunk,
        record,
        serie,
        sample
    }

    public interface IIndexFactory
    {
        long? Current { get; }
        long? Next { get; }
    }

    public interface IIndexed
    {
        long? Index { get; set; }
    }
    public interface ITimed
    {
        DateTime When { get; set; }
        long? NanoTime { get; set; }
    }

    public enum Quality : byte
    {
        good = 0,
        uncertain = 1,
        bad = 2
    }

    public interface ITimeWindows
    {
        IRange<DateTime> Period { get; }
    }

    public interface ISerieItem : IIndexed, ICloneable
    {
        IDictionary<string, object> Tags { get; set; } // allow vendors to add specific tags.
        bool HasTags { get; }
        SerieItemType ItemType { get; }
    }

    public interface ICompoundSerieItem : ISerieItem, ITimeWindows, IGeoBounded
    {
        bool HasChildren { get; }
        int Count { get; }
        IEnumerable<ISerieItem> Childrens { get; }
        void Add(ISerieItem item);
        void AddRange(IEnumerable<ISerieItem> item);
        void Remove(ISerieItem item);
        void Clear();
    }

    public interface ICompoundSerieItem<T> : ISerieItem, ITimeWindows
        where T : ISerieItem
    {
        bool HasChildren { get; }
        int Count { get; }
        IEnumerable<T> Childrens { get; }
        void Add(T item);
        void AddRange(IEnumerable<T> item);
        void Remove(T item);
        void Clear();
        T this [int i] { get; }
    }

    public interface ISample<T> : ISerieItem, ILocated, ITimeWindows, IRange<DateTime>, ITimed
    {
        T Value { get; set; }
        Quality Quality { get; set; }
    }

    public interface IMeasurement: ICloneable
    {
        string Name { get; set; }
        string Schema { get; set; }
    }

    public interface ITimeSerie : ICompoundSerieItem
    {
        IMeasurement Measurement { get;}
    }

    public interface ITimeSerie<T> : ICompoundSerieItem<ISample<T>>
    {
        IMeasurement Measurement { get; }
    }

    public interface IRecord : ICompoundSerieItem<ITimeSerie>
    {
        string DeviceId { get; set; }
        string Unit { get; set; }
    }

    public interface IDataChunk : ICompoundSerieItem<IRecord>
    {
    }
}
