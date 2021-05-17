using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOfThings.Spatial.Geofencing.Text.Json
{
    public class Vector3JsonConverter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Read(ref reader, typeToConvert, options, 0);
        }

        protected virtual Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, float defaultValue)
        {
            float[] v = JsonSerializer.Deserialize<float[]>(ref reader, options);
            if (v == null) return new Vector3(defaultValue);
            var l = v.Length;
            int i = 0;
            return new Vector3()
            {
                X = l > i ? v[i++] : defaultValue,
                Y = l > i ? v[i++] : defaultValue,
                Z = l > i ? v[i  ] : defaultValue
            };
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            float[] v = { value.X, value.Y, value.Z };
            JsonSerializer.Serialize<float[]>(writer, v, options);
        }
    }
}
