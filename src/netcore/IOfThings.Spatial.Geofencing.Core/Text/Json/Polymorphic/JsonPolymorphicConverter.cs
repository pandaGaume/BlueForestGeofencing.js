using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOfThings.Spatial.Geofencing.Text.Json
{
    public class JsonPolymorphicConverter<T> : JsonConverter<T>
    {
        public static JsonConverter MakeGenericConverter(Type t, params object?[]? args) => (JsonConverter)Activator.CreateInstance(typeof(JsonPolymorphicConverter<>).MakeGenericType(t), args);


        static string[] TypeDiscriminatorDefault = { "type", "@type" };

        string[] _td;
        IJsonPolymorphicTypeFactory _factory;

        public JsonPolymorphicConverter(IJsonPolymorphicTypeFactory factory=null, string[] typeDescriminator = null )
        {
            _factory = factory?? JsonPolymorphicTypeFactory.Shared;
            _td = typeDescriminator ?? TypeDiscriminatorDefault;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

            var snapshot = reader;

            if (!reader.Read() || reader.TokenType != JsonTokenType.PropertyName || !_td.Contains(reader.GetString())) throw new JsonException();
            if (!reader.Read()) throw new JsonException();

            Type t = default;

            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    {
                        var name = reader.GetString();
                        t = GetType(name);
                        break;
                    }
                default: throw new JsonException();
            }

            if (t == default) throw new JsonException();

            if (typeof(T).IsAssignableFrom(t))
            {
                var result = JsonSerializer.Deserialize(ref snapshot, t, options);
                // copy back
                reader = snapshot;
                return (T)result;
            }
            reader.Skip();

            if (reader.TokenType != JsonTokenType.EndObject) throw new JsonException();

            return default;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }

        public Type GetType(string typeName)=> _factory.ForName(typeName);
    }
}
