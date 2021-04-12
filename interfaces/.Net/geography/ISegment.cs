
namespace IOfThings.Spatial.Geography
{
    public interface ISegment<T>
        where T : ILocated
    {
        T First { get; }
        T Second { get; }
    }
}
