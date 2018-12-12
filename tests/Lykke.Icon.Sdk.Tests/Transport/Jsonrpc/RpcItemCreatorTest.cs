using System;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Xunit;

namespace Lykke.Icon.Sdk.Tests.Transport.Jsonrpc
{
    public class RpcItemCreatorTest
    {
        public const String outerVar = "outerVar";

        public class TokenBalance
        {
            public String Owner { get; set; }
        }

        [Fact]
        public void TestCreate()
        {
            String address = "hx4873b94352c8c1f3b2f09aaeccea31ce9e90bd31";
            TokenBalance p = new TokenBalance();
            p.Owner = address;
            RpcObject @params = (RpcObject)RpcItemCreator.Create(p);

            RpcObject expectedParams = new RpcObject.Builder()
                .Put("Owner", new RpcValue(address))
                .Build();

            Assert.Equal(@params.GetItem("Owner").ToString(),
            expectedParams.GetItem("Owner").ToString());

            Assert.Null(@params.GetItem("outerVar"));
        }
    }
}
