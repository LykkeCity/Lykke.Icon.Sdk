using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Newtonsoft.Json;
using System.Numerics;
using System;
using Xunit;

namespace Lykke.Icon.Sdk.Tests.Transport.Jsonrpc
{
    public class SerializerTest
    {
        [Fact]
        public void TestRpcSerializer()
        {
            RpcItem intValue = new RpcValue(BigInteger.Parse("1234"));
            RpcItem booleanValue = new RpcValue(false);
            RpcItem stringValue = new RpcValue("string");
            RpcItem bytesValue = new RpcValue(new byte[] { 0x1, 0x2, 0x3 });
            RpcItem escapeValue = new RpcValue("\\.{}[]");

            RpcItem @object = new RpcObject.Builder()
                    .Put("intValue", intValue)
                    .Put("booleanValue", booleanValue)
                    .Put("stringValue", stringValue)
                    .Put("bytesValue", bytesValue)
                    .Put("escapeValue", escapeValue)
                    .Build();

            RpcItem array = new RpcArray.Builder()
                    .Add(@object)
                    .Add(intValue)
                    .Add(booleanValue)
                    .Add(stringValue)
                    .Add(bytesValue)
                    .Build();

            RpcItem root = new RpcObject.Builder()
                    .Put("object", @object)
                    .Put("array", array)
                    .Put("intValue", intValue)
                    .Put("booleanValue", booleanValue)
                    .Put("stringValue", stringValue)
                    .Put("bytesValue", bytesValue)
                    .Build();

            String json = JsonConvert.SerializeObject(root, Formatting.Indented, new RpcItemSerializer());
            Assert.True(json.Length > 2);
        }

        [Fact]
        public void TestRequestSerialization()
        {
            var wallet = KeyWallet.Load(new Bytes(SampleKeys.PRIVATE_KEY_STRING));
            var requestId = 123;
            RpcObject @params = new RpcObject.Builder()
                .Put("address", new RpcValue(wallet.GetAddress()))
                .Build();
            var request = new Request(requestId, "icx_getBalance", @params);

            String json = JsonConvert.SerializeObject(@params, Formatting.Indented, new RpcItemSerializer());
            Assert.True(json.Length > 2);
        }
    }
}
