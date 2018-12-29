using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Moq;
using Xunit;

namespace Lykke.Icon.Sdk.Tests
{

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class IconServiceTest
    {
        [Fact]
        public void TestIconServiceInit()
        {
            var provider = GetMockProvider();
            var iconService = new IconService(provider.Object);
            Assert.NotNull(iconService);
        }

        [Fact]
        public async Task TestGetTotalSupply()
        {
            var provider = GetMockProvider<BigInteger>();

            var iconService = new IconService(provider.Object);
            
            // ReSharper disable once UnusedVariable
            var result = await iconService.GetTotalSupply();

            provider.Verify(x =>
                x.SendRequestAsync(It.Is<Request>(request => IsRequestMatches(request, "icx_getTotalSupply", null)),
                    It.IsAny<IRpcConverter<BigInteger>>()), Times.Once);
        }

        [Fact]
        public async Task TestGetBalance()
        {
            var address = new Address("hx4873b94352c8c1f3b2f09aaeccea31ce9e90bd31");
            var @params = new Dictionary<string, RpcValue>
            {
                ["address"] = new RpcValue(address)
            };
            
            var provider = GetMockProvider<BigInteger>();

            var iconService = new IconService(provider.Object);
            await iconService.GetBalance(address);

            provider.Verify(x =>
                x.SendRequestAsync(It.Is<Request>(request => IsRequestMatches(request, "icx_getBalance", @params)),
                    It.IsAny<IRpcConverter<BigInteger>>()), Times.Once);
        }

        [Fact]
        public async Task TestGetBlockByHeight()
        {
            var provider = GetMockProvider<Block>();

            var iconService = new IconService(provider.Object);
            
            // ReSharper disable once UnusedVariable
            var result = await iconService.GetBlock(BigInteger.One);

            var @params = new Dictionary<string, RpcValue>
            {
                ["height"] = new RpcValue(BigInteger.One)
            };

            provider.Verify(x =>
                x.SendRequestAsync(It.Is<Request>(request => IsRequestMatches(request, "icx_getBlockByHeight", @params)),
                    It.IsAny<IRpcConverter<Block>>()), Times.Once);
        }

        private static Mock<IProvider> GetMockProvider()
        {
            var provider = new Mock<IProvider>();
            provider
                .Setup(x => x.SendRequestAsync(It.IsAny<Request>(), It.IsAny<IRpcConverter<object>>()))
                .ReturnsAsync(null)
                .Verifiable();

            return provider;
        }

        private static Mock<IProvider> GetMockProvider<T>()
        {
            var provider = new Mock<IProvider>();
            provider
                .Setup(x => x.SendRequestAsync(It.IsAny<Request>(), It.IsAny<IRpcConverter<T>>()))
                .ReturnsAsync(default(T))
                .Verifiable();

            return provider;
        }

        [Fact]
        public async Task TestGetBlockByHash()
        {
            var provider = GetMockProvider<Block>();

            var hash = new Bytes("0x033f8d96045eb8301fd17cf078c28ae58a3ba329f6ada5cf128ee56dc2af26f7");

            var iconService = new IconService(provider.Object);
            
            // ReSharper disable once UnusedVariable
            var result = await iconService.GetBlock(hash);

            var @params = new Dictionary<string, RpcValue>
            {
                ["hash"] = new RpcValue(hash)
            };

            provider.Verify(x =>
                x.SendRequestAsync(It.Is<Request>(request => IsRequestMatches(request, "icx_getBlockByHash", @params)),
                    It.IsAny<IRpcConverter<Block>>()), Times.Once);
        }

        [Fact]
        public async Task TestGetLastBlock()
        {
            var provider = GetMockProvider<Block>();

            var iconService = new IconService(provider.Object);
            
            // ReSharper disable once UnusedVariable
            var result = await iconService.GetLastBlock();

            provider.Verify(x =>
                x.SendRequestAsync(It.Is<Request>(request => IsRequestMatches(request, "icx_getLastBlock", null)),
                    It.IsAny<IRpcConverter<Block>>()), Times.Once);
        }

        [Fact]
        public async Task TestGetScoreApi()
        {
            var provider = GetMockProvider<List<ScoreApi>>();

            var address = new Address("cx4873b94352c8c1f3b2f09aaeccea31ce9e90bd31");

            var iconService = new IconService(provider.Object);
            
            // ReSharper disable once UnusedVariable
            var result = await iconService.GetScoreApi(address);

            var @params = new Dictionary<string, RpcValue>
            {
                ["address"] = new RpcValue(address)
            };

            provider.Verify(x =>
                x.SendRequestAsync(It.Is<Request>(request => IsRequestMatches(request, "icx_getScoreApi", @params)),
                    It.IsAny<IRpcConverter<List<ScoreApi>>>()), Times.Once);
        }

        [Fact]
        public async Task TestGetTransaction()
        {
            var provider = GetMockProvider<ConfirmedTransaction>();

            var hash = new Bytes("0x2600770376fbf291d3d445054d45ed15280dd33c2038931aace3f7ea2ab59dbc");

            var iconService = new IconService(provider.Object);
            
            // ReSharper disable once UnusedVariable
            var result = await iconService.GetTransaction(hash);

            var @params = new Dictionary<string, RpcValue>
            {
                ["txHash"] = new RpcValue(hash)
            };

            provider.Verify(x =>
                x.SendRequestAsync(It.Is<Request>(request => IsRequestMatches(request, "icx_getTransactionByHash", @params)),
                    It.IsAny<IRpcConverter<ConfirmedTransaction>>()), Times.Once);
        }

        [Fact]
        public async Task TestGetTransactionResult()
        {
            var provider = GetMockProvider<TransactionResult>();

            var hash = new Bytes("0x2600770376fbf291d3d445054d45ed15280dd33c2038931aace3f7ea2ab59dbc");

            var iconService = new IconService(provider.Object);
            
            // ReSharper disable once UnusedVariable
            var result = await iconService.GetTransactionResult(hash);

            var @params = new Dictionary<string, RpcValue>
            {
                ["txHash"] = new RpcValue(hash)
            };

            provider.Verify(x =>
                x.SendRequestAsync(It.Is<Request>(request => IsRequestMatches(request, "icx_getTransactionResult", @params)),
                    It.IsAny<IRpcConverter<TransactionResult>>()), Times.Once);
        }

        private class TestQueryRpcConverterFactory : IRpcConverterFactory
        {
            public IRpcConverter<TPersonResponse> Create<TPersonResponse>()
            {
                return (IRpcConverter<TPersonResponse>)new TestQueryRpcConverter();
            }

            private class TestQueryRpcConverter : IRpcConverter<PersonResponse>
            {
                public PersonResponse ConvertTo(RpcItem @object)
                {
                    return new PersonResponse();
                }

                public RpcItem ConvertFrom(PersonResponse @object)
                {
                    throw new Exception();
                }
            }
        }

        [Fact]
        public async Task TestQuery()
        {
            var provider = GetMockProvider<PersonResponse>();

            var iconService = new IconService(provider.Object);
            iconService.AddConverterFactory<PersonResponse>(new TestQueryRpcConverterFactory());

            var person = new Person
            {
                Name = "gold bug", 
                Age = BigInteger.Parse("20"), 
                HasPermission = false
            };

            var call = new Call.Builder()
                    .From(new Address("hxbe258ceb872e08851f1f59694dac2558708ece11"))
                    .To(new Address("cx5bfdb090f43a808005ffc27c25b213145e80b7cd"))
                    .Method("addUser")
                    .Params(person)
                    .BuildWith<PersonResponse>();

            // ReSharper disable once UnusedVariable
            var query = await iconService.CallAsync(call);

            provider.Verify(x =>
                x.SendRequestAsync(It.Is<Request>(request => TestQueryRequestCheck(request, person)),
                    It.IsAny<IRpcConverter<PersonResponse>>()), Times.Once);
        }

        private static bool TestQueryRequestCheck(Request request, Person person1)
        {
            if (!request.Method.Equals("icx_call"))
            {
                return false;
            }

            var @params = request.Params;
            var dataParams = @params.GetItem("data").ToObject()
                .GetItem("params").ToObject();
            return dataParams.GetItem("Name").ToString().Equals(person1.Name) &&
                   dataParams.GetItem("Age").ToInteger().Equals(person1.Age) &&
                   dataParams.GetItem("HasPermission").ToBoolean() == person1.HasPermission;
        }

        [Fact]
        public async Task TestSendIcxTransaction()
        {
            var provider = GetMockProvider<Bytes>();
            var fromAddress = new Address("hxbe258ceb872e08851f1f59694dac2558708ece11");
            var toAddress = new Address("hx5bfdb090f43a808005ffc27c25b213145e80b7cd");

            var transaction = TransactionBuilder.CreateBuilder()
                    .Nid(NetworkId.Main)
                    .From(fromAddress)
                    .To(toAddress)
                    .Value(BigInteger.Parse("0de0b6b3a7640000", NumberStyles.AllowHexSpecifier))
                    .StepLimit(BigInteger.Parse("012345", NumberStyles.AllowHexSpecifier))
                    .Timestamp(BigInteger.Parse("0563a6cf330136", NumberStyles.AllowHexSpecifier))
                    .Nonce(BigInteger.Parse("1"))
                    .Build();
            IWallet wallet = KeyWallet.Load(new Bytes(SampleKeys.PrivateKeyString));
            var signedTransaction = new SignedTransaction(transaction, wallet);

            var iconService = new IconService(provider.Object);
            await iconService.SendTransaction(signedTransaction);

            var expected = "xR6wKs+IA+7E91bT8966jFKlK5mayutXCvayuSMCrx9KB7670CsWa0B7LQzgsxU0GLXaovlAT2MLs1XuDiSaZQE=";
            provider.Verify(x =>
                x.SendRequestAsync(It.Is<Request>(request => RequestCheckTestSendIcxTransaction(request, expected)),
                    It.IsAny<IRpcConverter<Bytes>>()), Times.Once);
        }

        private static bool RequestCheckTestSendIcxTransaction(Request request, string expected)
        {
            var isMethodMatch = request.Method.Equals("icx_sendTransaction");
            var isSignatureMatch = request.Params.GetItem("signature").ToString().Equals(expected);
            return isMethodMatch && isSignatureMatch;
        }

        [Fact]
        public async Task TestTransferTokenTransaction()
        {
            var provider = GetMockProvider<Bytes>();

            var fromAddress = new Address("hxbe258ceb872e08851f1f59694dac2558708ece11");
            var scoreAddress = new Address("cx982aed605b065b50a2a639c1ea5710ef5a0501a9");
            var toAddress = new Address("hx5bfdb090f43a808005ffc27c25b213145e80b7cd");
            
            // ReSharper disable once ObjectCreationAsStatement
            new Address("hxbe258ceb872e08851f1f59694dac2558708ece11");

            var @params = new RpcObject.Builder()
                            .Put("_to", new RpcValue(toAddress))
                            .Put("_value", new RpcValue(BigInteger.Parse("1")))
                            .Build();

            var transaction = TransactionBuilder.CreateBuilder()
                    .Nid(NetworkId.Main)
                    .From(fromAddress)
                    .To(scoreAddress)
                    .StepLimit(BigInteger.Parse("12345", NumberStyles.HexNumber))
                    .Timestamp(BigInteger.Parse("563a6cf330136", NumberStyles.HexNumber))
                    .Nonce(BigInteger.Parse("1"))
                    .Call("transfer")
                    .Params(@params)
                    .Build();

            IWallet wallet = KeyWallet.Load(new Bytes(SampleKeys.PrivateKeyString));
            var signedTransaction = new SignedTransaction(transaction, wallet);

            var iconService = new IconService(provider.Object);
            await iconService.SendTransaction(signedTransaction);

            const string expected = "ITpAdh3bUV4Xj0WQIPlfhv+ppA+K+LtXqaYMjnt8pMwV7QJwyZNQuhH2ljdGPR+31wG+GpKEdOEuqeYOwODBVwA=";
            provider.Verify(x =>
                x.SendRequestAsync(It.Is<Request>(request => RequestCheckTestTransferTokenTransaction(request, expected)),
                    It.IsAny<IRpcConverter<Bytes>>()), Times.Once);
        }

        private static bool RequestCheckTestTransferTokenTransaction(Request request, string expected)
        {
            var isMethodMatch = request.Method.Equals("icx_sendTransaction");
            var isSignatureMatch = request.Params.GetItem("signature").ToString().Equals(expected);
            return isMethodMatch && isSignatureMatch;
        }

        [Fact]
        public async Task TestConverterNotfound()
        {
            var provider = GetMockProvider();

            var iconService = new IconService(provider.Object);
            var person = new Person
            {
                Name = "gold bug",
                Age = BigInteger.Parse("20"),
                HasPermission = false
            };

            var call = new Call.Builder()
                    .From(new Address("hxbe258ceb872e08851f1f59694dac2558708ece11"))
                    .To(new Address("cx5bfdb090f43a808005ffc27c25b213145e80b7cd"))
                    .Method("addUser")
                    .Params(person)
                    .BuildWith<PersonResponse>();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await iconService.CallAsync(call);
            });
        }


        private static bool IsRequestMatches(Request request, string method, Dictionary<string, RpcValue> @params)
        {
            if (!request.Method.Equals(method)) return false;
            if (request.Params == null && @params == null) return true;
            if (request.Params != null && @params != null)
            {
                var isParamMatches = true;
                IEnumerable<string> keys = @params.Keys;
                foreach (var key in keys)
                {
                    var value = (RpcValue)request.Params.GetItem(key);
                    isParamMatches = value.ToString().Equals(@params[key].ToString());
                    if (!isParamMatches) break;
                }
                return isParamMatches;
            }
            return false;
        }

        [UsedImplicitly]
        public class Person
        {
            public string Name { get; set; }
            public BigInteger Age { get; set; }
            public bool HasPermission { get; set; }
        }

        [UsedImplicitly]
        public class PersonResponse
        {
            public bool IsOk { get; set; }
            public string Message { get; set; }
        }
    }
}