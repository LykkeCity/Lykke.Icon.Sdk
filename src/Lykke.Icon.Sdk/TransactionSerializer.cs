using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk
{
    /// <summary>
    ///    Transaction Serializer for generating a signature with transaction properties
    /// </summary>
    public static class TransactionSerializer
    {
        /// <summary>
        ///    Serializes properties to string
        /// </summary>
        public static string Serialize(RpcObject properties)
        {
            var builder = new StringBuilder();
            builder.Append("icx_sendTransaction.");
            SerializeObjectItems(builder, properties);
            return builder.ToString();
        }

        private static string Escape(string @string)
        {
            return Regex.Escape(@string);
        }

        private static void Serialize(StringBuilder builder, RpcItem item)
        {
            switch (item)
            {
                case RpcObject _:
                    builder.Append("{");
                    SerializeObjectItems(builder, item.ToObject());
                    builder.Append("}");
                    break;
                case RpcArray _:
                    builder.Append("[");
                    SerializeArrayItems(builder, item.ToArray());
                    builder.Append("]");
                    break;
                case null:
                    builder.Append("\\0");
                    break;
                default:
                    builder.Append(Escape(item.ToString()));
                    break;
            }
        }

        private static void SerializeObjectItems(StringBuilder builder, RpcObject @object)
        {
            var firstItem = true;

            foreach (var key in @object.GetKeys().OrderBy(x => x))
            {
                if (firstItem)
                {
                    firstItem = false;
                }
                else
                {
                    builder.Append(".");
                }

                Serialize(builder.Append(key).Append("."), @object.GetItem(key));
            }
        }

        private static void SerializeArrayItems(StringBuilder builder, RpcArray array)
        {
            var firstItem = true;
            foreach (var child in array)
            {
                if (firstItem)
                {
                    firstItem = false;
                }
                else
                {
                    builder.Append(".");
                }

                Serialize(builder, child);
            }
        }
    }
}