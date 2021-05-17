using System;
using System.Numerics;
using System.Text.Json;

namespace IOfThings.Spatial.Geofencing.Text.Json
{
    public class ScaleJsonConverter : Vector3JsonConverter
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Read(ref reader, typeToConvert, options, 1);
        }
    }
}
