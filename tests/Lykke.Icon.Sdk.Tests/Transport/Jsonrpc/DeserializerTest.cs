using Lykke.Icon.Sdk.Transport.JsonRpc;
using Newtonsoft.Json;
using System.Numerics;
using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Lykke.Icon.Sdk.Tests.Transport.Jsonrpc
{
    public class DeserializerTest
    {

        [Fact]
        public void TestRpcDeserializer()
        {
            String json = "{\"stringValue\":\"string\",\"array\":[{\"longValue\":1533018344753765,\"stringValue\":\"string\",\"intValue\":\"0x4d2\",\"booleanValue\":\"0x0\",\"bytesValue\":\"0x010203\"},\"0x4d2\",\"0x0\",\"string\",\"0x010203\"],\"intValue\":\"0x4d2\",\"booleanValue\":\"0x0\",\"bytesValue\":\"0x010203\",\"object\":{\"stringValue\":\"string\",\"intValue\":\"0x4d2\",\"booleanValue\":\"0x0\",\"bytesValue\":\"0x010203\"}}";
            RpcObject root = (RpcObject)(RpcItem)JsonConvert.DeserializeObject(json, typeof(RpcObject), new RpcItemSerializer());

            RpcValue rpcValue;

            RpcArray array = (RpcArray)root.GetItem("array");

            RpcObject obj = (RpcObject)array.Get(0);
            rpcValue = (RpcValue)obj.GetItem("intValue");
            Assert.Equal(BigInteger.Parse("4d2", NumberStyles.HexNumber), rpcValue.ToInteger());
            rpcValue = (RpcValue)obj.GetItem("booleanValue");
            Assert.False(rpcValue.ToBoolean());
            rpcValue = (RpcValue)obj.GetItem("stringValue");
            Assert.Equal("string", rpcValue.ToString());
            rpcValue = (RpcValue)obj.GetItem("bytesValue");
            Assert.True(Enumerable.SequenceEqual(new byte[] { 0x1, 0x2, 0x3 }, rpcValue.ToByteArray()));
            rpcValue = (RpcValue)obj.GetItem("longValue");
            Assert.Equal(BigInteger.Parse(1533018344753765L.ToString()), rpcValue.ToInteger());

            rpcValue = (RpcValue)array.Get(1);
            Assert.Equal(BigInteger.Parse("4d2", NumberStyles.HexNumber), rpcValue.ToInteger());
            rpcValue = (RpcValue)array.Get(2);
            Assert.False(rpcValue.ToBoolean());
            rpcValue = (RpcValue)array.Get(3);
            Assert.Equal("string", rpcValue.ToString());
            rpcValue = (RpcValue)array.Get(4);
            Assert.True(Enumerable.SequenceEqual(new byte[] { 0x1, 0x2, 0x3 }, rpcValue.ToByteArray()));

            rpcValue = (RpcValue)root.GetItem("intValue");
            Assert.Equal(BigInteger.Parse("4d2", NumberStyles.HexNumber), rpcValue.ToInteger());
            rpcValue = (RpcValue)root.GetItem("booleanValue");
            Assert.False(rpcValue.ToBoolean());
            rpcValue = (RpcValue)root.GetItem("stringValue");
            Assert.Equal("string", rpcValue.ToString());
            rpcValue = (RpcValue)root.GetItem("bytesValue");
            Assert.True(Enumerable.SequenceEqual(new byte[] { 0x1, 0x2, 0x3 }, rpcValue.ToByteArray()));
        }

        [Fact]
        public void TestRpcValue()
        {
            String json = "\"0x1234\"";
            RpcItem root = (RpcItem)JsonConvert.DeserializeObject(json, typeof(RpcItem), new RpcItemSerializer());

            Assert.True(root is RpcValue);
            Assert.Equal("0x1234", ((RpcValue)root).ToString());
        }
    }
}
