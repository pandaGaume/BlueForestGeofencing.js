using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOfThings.Spatial.Geofencing.Text.Json
{
    public class QuaternionJsonConverter : JsonConverter<Quaternion>
    {
        public override Quaternion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            float[] v = JsonSerializer.Deserialize<float[]>(ref reader, options);
            if (v == null) return Quaternion.Identity;
            var l = v.Length;
            int i = 0;
            return new Quaternion()
            {
                X = l > i ? v[i++] : 0,
                Y = l > i ? v[i++] : 0,
                Z = l > i ? v[i++] : 0,
                W = l > i ? v[i++] : 1
            };
        }

        public override void Write(Utf8JsonWriter writer, Quaternion value, JsonSerializerOptions options)
        {
            float[] v = { value.X, value.Y, value.Z, value.W };
            JsonSerializer.Serialize<float[]>(writer, v, options);
        }
    }
}
