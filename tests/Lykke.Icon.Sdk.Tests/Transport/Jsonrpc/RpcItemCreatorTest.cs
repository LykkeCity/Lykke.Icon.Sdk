using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Xunit;

namespace Lykke.Icon.Sdk.Tests.Transport.Jsonrpc
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class RpcItemCreatorTest
    {
        [UsedImplicitly]
        public class TokenBalance
        {
            [UsedImplicitly]
            public string Owner { get; set; }
        }

        [Fact]
        public void TestCreate()
        {
            const string address = "hx4873b94352c8c1f3b2f09aaeccea31ce9e90bd31";
            var p = new TokenBalance
            {
                Owner = address
            };
            
            var @params = (RpcObject)RpcItemCreator.Create(p);

            var expectedParams = new RpcObject.Builder()
                .Put("Owner", new RpcValue(address))
                .Build();

            Assert.Equal(@params.GetItem("Owner").ToString(),
            expectedParams.GetItem("Owner").ToString());

            Assert.Null(@params.GetItem("outerVar"));
        }
    }
}
