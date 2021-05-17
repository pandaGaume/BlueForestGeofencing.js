using IOfThings.Spatial.Text.GeoJson;
using IOfThings.Spatial.Text.GeoJson.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace IOfThings.Spatial.Geofencing.Text.Json
{
    public static class GeofencingJsonSerializer
    {
        public static class GeoJsonExtendedPropertyNames
        {
            public const string geofence = "geofence";
        }

        internal static IEnumerable<IGeofence> MountGeofence(this IGeoJsonFeature<object> feature)
        {
            if (feature.Properties.TryGetValue(GeoJsonExtendedPropertyNames.geofence, out var obj))
            {
                if (obj is IGeofence geofence)
                {
                    geofence.Geometry = feature.Geometry;
                    yield return geofence.Shapes.BindGeofence(geofence)
                            .Nodes.BindGeofence(geofence)
                            .Modifiers.BindGeofence(geofence)
                            .Alerts.BindGeofence(geofence)
                            .Primitives.BindGeofence(geofence);
                }
            }
        }

        internal static IEnumerable<IGeofence> PrepareGeofences(this IEnumerable<IGeoJsonFeature<object>> features) => features.SelectMany(f => f.MountGeofence()).Where(g => g != default);

        public static List<IGeofence> Deserialize(string json, JsonSerializerOptions options)
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
            if (geojson is IGeoJsonFeature<object> feature)
            {
                list.AddRange(feature.MountGeofence());
            }
            else if (geojson is IGeoJsonFeatureCollection<object> featureCollection)
            {
                list.AddRange(featureCollection.Features.PrepareGeofences());
            }
            return list;
        }
    }
}
