using Newtonsoft.Json;
using System.Numerics;
using System;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    /**
     * Serializers for jsonrpc value
     */
    public class RpcItemSerializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var result = typeof(RpcItem).IsAssignableFrom(objectType);
            return result;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                RpcObject.Builder builder = new RpcObject.Builder();
                String fieldName = null;
                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            fieldName = reader.Value.ToString();
                            break;
                        case JsonToken.EndObject:
                            return builder.Build();
                        default:
                            builder.Put(fieldName, (RpcItem)this.ReadJson(reader, objectType, existingValue, serializer));
                            break;
                    }
                }
                return builder.Build();
            }
            else if (reader.TokenType == JsonToken.StartArray)
            {
                RpcArray.Builder builder = new RpcArray.Builder();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray)
                        return builder.Build();

                    var item = (RpcItem)this.ReadJson(reader, objectType, existingValue, serializer);
                    builder.Add(item);
                }
                return builder.Build();
            }
            else
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Boolean:
                        {
                            return new RpcValue((bool)reader.Value);
                        }

                    case JsonToken.Integer:
                        {
                            return new RpcValue(BigInteger.Parse(reader.Value.ToString()));
                        }

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
                RpcObject @object = rpcItem.ToObject();
                writer.WriteStartObject();
                foreach (String key in @object.GetKeys())
                {
                    RpcItem item = @object.GetItem(key);
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
                RpcArray array = rpcItem.ToArray();
                writer.WriteStartArray();
                foreach (RpcItem childItem in array)
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
