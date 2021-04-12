
namespace IOfThings.Spatial.Geography
{
    public interface IGeoBounded : ILocated
    {
        IEnvelope BoundingEnvelope { get; }
    }
}
