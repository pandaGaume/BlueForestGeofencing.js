using IOfThings.Spatial.Text.GeoJson;
using IOfThings.Spatial.Text.GeoJson.Converters;
using IOfThings.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IOfThings.Spatial.Geofencing.Text.Json
{
    public static class GeofencingJsonSerializer
    {
        public static class GeoJsonExtendedPropertyNames
        {
            public const string geofence = "geofence";
        }
        internal static IEnumerable<IGeofence> MountGeofence(this IGeoJsonFeature<IGeoJsonObject, object> feature)
        {
            if (feature.Properties != null && feature.Properties.TryGetValue(GeoJsonExtendedPropertyNames.geofence, out var obj))
            {
                if (obj is IGeofence geofence)
                {
                    geofence.Geometry = feature.Geometry;
                    return new IGeofence[] { geofence.Mount() };
                }
            }
            return Enumerable.Empty<IGeofence>();
        }
        internal static IEnumerable<IGeofence> MountGeofences(this IEnumerable<IGeoJsonFeature<IGeoJsonObject, object>> features) => features.SelectMany(f => f.MountGeofence()).Where(g => g != default);
        public static List<IGeofence> Deserialize(string json, JsonSerializerOptions options = null)
        {
            var o = options ?? new JsonSerializerOptions();

            // build the inner properties converter 
            var geoJsonConverter = new GeoJsonPropertiesJsonConverter<object>();
            geoJsonConverter.BindType(GeoJsonExtendedPropertyNames.geofence, typeof(IGeofence));

            // register the converters into options
            o.Converters.Add(geoJsonConverter);
            o.Converters.Add(new JsonPolymorphicConverterFactory<IGeofencingItem>());

            // deserialize the geojson
            var geojson = JsonSerializer.Deserialize<IGeoJsonObject>(json, o);

            var list = new List<IGeofence>(1);
            if (geojson is IGeoJsonFeature<IGeoJsonObject, object> feature)
            {
                list.AddRange(feature.MountGeofence());
            }
            else if (geojson is IGeoJsonFeatureCollection<object> featureCollection)
            {
                list.AddRange(featureCollection.Features.MountGeofences());
            }
            return list;
        }
        public static async ValueTask<List<IGeofence>> DeserializeAsync(Stream jsonStream, JsonSerializerOptions options = null, CancellationToken cancel = default)
        {
            var o = options ?? new JsonSerializerOptions();

            // build the inner properties converter 
            var geoJsonConverter = new GeoJsonPropertiesJsonConverter<object>();
            geoJsonConverter.BindType(GeoJsonExtendedPropertyNames.geofence, typeof(IGeofence));

            // register the converters into options
            o.Converters.Add(geoJsonConverter);
            o.Converters.Add(new JsonPolymorphicConverterFactory<IGeofencingItem>());

            // deserialize the geojson
            var geojson = await JsonSerializer.DeserializeAsync<IGeoJsonObject>(jsonStream, o, cancel);

            var list = new List<IGeofence>(1);
            if (geojson is IGeoJsonFeature feature)
            {
                list.AddRange(feature.MountGeofence());
            }
            else if (geojson is IGeoJsonFeatureCollection featureCollection)
            {
                list.AddRange(featureCollection.Features.MountGeofences());
            }
            return list;
        }
    }
}
