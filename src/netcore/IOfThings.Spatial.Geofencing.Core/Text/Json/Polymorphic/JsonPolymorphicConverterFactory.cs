using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOfThings.Spatial.Geofencing.Text.Json
{
    public class JsonPolymorphicConverterFactory<T> : JsonConverterFactory
    {
        readonly string[] _td;
        readonly IJsonPolymorphicTypeFactory _factory;

        public JsonPolymorphicConverterFactory(IJsonPolymorphicTypeFactory factory = null, string[] typeDescriminator = null)
        {
            _factory = factory ;
            _td = typeDescriminator;
        }

        public override bool CanConvert(Type typeToConvert)=> typeToConvert.IsInterface && typeof(T).IsAssignableFrom(typeToConvert);
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) => JsonPolymorphicConverter<T>.MakeGenericConverter(typeToConvert, _factory, _td);
    }
}
