using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOfThings.Spatial.Geofencing.Text.Json
{
    public class RecurrenceTypeJsonConverter : JsonConverter<Nullable<RecurrenceType>>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(RecurrenceType) || typeToConvert == typeof(Nullable<RecurrenceType>);
        }
        public override Nullable<RecurrenceType> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            if( reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if( Enum.TryParse(typeof(RecurrenceType), str, true, out var rt))
                {
                    return (Nullable < RecurrenceType >)rt;
                }
            }
            if (reader.TokenType == JsonTokenType.Number)
            {
                try
                {
                    return (Nullable<RecurrenceType>) Enum.ToObject(typeof(RecurrenceType), reader.GetInt32());
                }
                catch 
                { 
                }
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, Nullable<RecurrenceType> value, JsonSerializerOptions options)
        {
            if(value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString());
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
