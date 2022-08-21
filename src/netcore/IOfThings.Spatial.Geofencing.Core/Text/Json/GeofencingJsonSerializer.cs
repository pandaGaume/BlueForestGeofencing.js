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
        internal static IEnumerable<T> MountGeofence<T>(this IGeoJsonFeature<IGeoJsonObject, object> feature) where T : IGeofence
        {
            if (feature.Properties != null && feature.Properties.TryGetValue(GeoJsonExtendedPropertyNames.geofence, out var obj))
            {
                if (obj is T geofence)
                {
                    geofence.Geometry = feature.Geometry;
                    return new T[] { (T)geofence.Mount() };
                }
            }
            return Enumerable.Empty<T>();
        }
        internal static IEnumerable<T> MountGeofences<T>(this IEnumerable<IGeoJsonFeature<IGeoJsonObject, object>> features) where T : IGeofence => features.SelectMany(f => f.MountGeofence<T>()).Where(g => g != null);
        public static List<T> Deserialize<T>(string json, JsonSerializerOptions options = null) where T : IGeofence
        {
            var o = options ?? new JsonSerializerOptions();

            // build the inner properties converter 
            var geoJsonConverter = new GeoJsonPropertiesJsonConverter<object>();
            geoJsonConverter.BindType(GeoJsonExtendedPropertyNames.geofence, typeof(T));

            // register the converters into options
            o.Converters.Add(geoJsonConverter);
            o.Converters.Add(new JsonPolymorphicConverterFactory<IGeofencingItem>());

            // deserialize the geojson
            var geojson = JsonSerializer.Deserialize<IGeoJsonObject>(json, o);

            var list = new List<T>(1);
            if (geojson is IGeoJsonFeature<IGeoJsonObject, object> feature)
            {
                list.AddRange(feature.MountGeofence<T>());
            }
            else if (geojson is IGeoJsonFeatureCollection<object> featureCollection)
            {
                list.AddRange(featureCollection.Features.MountGeofences<T>());
            }
            return list;
        }
        public static async ValueTask<List<T>> DeserializeAsync<T>(Stream jsonStream, JsonSerializerOptions options = null, CancellationToken cancel = default) where T : IGeofence
        {
            var o = options ?? new JsonSerializerOptions();

            // build the inner properties converter 
            var geoJsonConverter = new GeoJsonPropertiesJsonConverter<object>();
            geoJsonConverter.BindType(GeoJsonExtendedPropertyNames.geofence, typeof(T));

            // register the converters into options
            o.Converters.Add(geoJsonConverter);
            o.Converters.Add(new JsonPolymorphicConverterFactory<IGeofencingItem>());

            // deserialize the geojson
            var geojson = await JsonSerializer.DeserializeAsync<IGeoJsonObject>(jsonStream, o, cancel);

            var list = new List<T>(1);
            if (geojson is IGeoJsonFeature feature)
            {
                list.AddRange(feature.MountGeofence<T>());
            }
            else if (geojson is IGeoJsonFeatureCollection featureCollection)
            {
                list.AddRange(featureCollection.Features.MountGeofences<T>());
            }
            return list;
        }
    }
}
