
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOfThings.Spatial.Geofencing.Text.Json
{
    public class GeofencingExtensionsJsonConverter : JsonConverter<IList<IGeofencingExtension>>
    {
        public override IList<IGeofencingExtension> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var list = new List<IGeofencingExtension>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    switch (reader.TokenType)
                    {
                        case (JsonTokenType.PropertyName):
                            {
                                var extName = reader.GetString();
                                if (reader.Read())
                                {
                                    if (TryReadExtension(extName, ref reader, options, out IGeofencingExtension ext))
                                    {
                                        list.Add(ext);
                                    }
                                    break;
                                }
                                throw new JsonException();
                            }
                        default:
                            {
                                reader.Skip();
                                break;
                            }
                    }

                }
                return list;
            }
            reader.Skip();
            return default;
        }

        public override void Write(Utf8JsonWriter writer, IList<IGeofencingExtension> value, JsonSerializerOptions options)
        {
            foreach(var ext in value)
            {
                if( ext != null)
                {
                    var name = GeofencingExtensionTypeManager.Shared.ForType(value.GetType());
                    if( name != null)
                    {
                        writer.WritePropertyName(name);
                        JsonSerializer.Serialize(writer, ext, options);
                    }
                }
            }        
        }

        public bool TryReadExtension(string extensionName, ref Utf8JsonReader reader, JsonSerializerOptions options, out IGeofencingExtension extension)
        {
            var type = GeofencingExtensionTypeManager.Shared.ForName(extensionName);
            if( type != null && typeof(IGeofencingExtension).IsAssignableFrom(type))
            {
                extension = JsonSerializer.Deserialize(ref reader, type, options) as IGeofencingExtension;
                return extension != default;
            }
            reader.Skip();
            extension = default;
            return false;
        }
    }
}
