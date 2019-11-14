using System;
using System.Numerics;
using Newtonsoft.Json;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    public class RpcItemSerializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var result = typeof(RpcItem).IsAssignableFrom(objectType);
            return result;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (reader.TokenType == JsonToken.StartObject)
            {
                var builder = new RpcObject.Builder();
                string fieldName = null;
                while (reader.Read())
                {
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            fieldName = reader.Value.ToString();
                            break;
                        case JsonToken.EndObject:
                            return builder.Build();
                        default:
                            builder.Put(fieldName, (RpcItem)ReadJson(reader, objectType, existingValue, serializer));
                            break;
                    }
                }
                return builder.Build();
            }
            else if (reader.TokenType == JsonToken.StartArray)
            {
                var builder = new RpcArray.Builder();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray)
                        return builder.Build();

                    var item = (RpcItem)ReadJson(reader, objectType, existingValue, serializer);
                    builder.Add(item);
                }
                return builder.Build();
            }
            else
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (reader.TokenType)
                {
                    case JsonToken.Boolean:
                        return new RpcValue((bool)reader.Value);

                    case JsonToken.Integer:
                        return new RpcValue(BigInteger.Parse(reader.Value.ToString()));

                    case JsonToken.Null:
                        return new RpcValue((string)null);
                    default:
                        return new RpcValue(reader.Value.ToString());
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                serializer.Serialize(writer, null);
                
                return;
            }

            var rpcItem = value as RpcItem;
            
            if (rpcItem is RpcObject)
            {
                var @object = rpcItem.ToObject();
                
                writer.WriteStartObject();
                
                foreach (var key in @object.GetKeys())
                {
                    var item = @object.GetItem(key);
                    if (item != null)
                    {
                        writer.WritePropertyName(key);
                        serializer.Serialize(writer, item);
                    }
                }
                writer.WriteEndObject();
            }
            else if (rpcItem is RpcArray)
            {
                var array = rpcItem.ToArray();
                
                writer.WriteStartArray();
                
                foreach (var childItem in array)
                {
                    serializer.Serialize(writer, childItem);
                }
                
                writer.WriteEndArray();
            }
            else
            {
                serializer.Serialize(writer, rpcItem?.ToString());
            }
        }
    }
}
