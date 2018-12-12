using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Moq;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace Lykke.Icon.Sdk.Tests
{

    public class IconServiceTest
    {
        [Fact]
        public async Task TestIconServiceInit()
        {
            Mock<IProvider> provider = GetMockProvider();
            IconService iconService = new IconService(provider.Object);
            Assert.NotNull(iconService);
        }

        [Fact]
        public async Task TestGetTotalSupply()
        {
            Mock<IProvider> provider = GetMockProvider<BigInteger>();

            IconService iconService = new IconService(provider.Object);
            var result = await iconService.GetTotalSupply();

            provider.Verify(x =>
                x.Request(It.Is<Request>(request => IsRequestMatches(request, "icx_getTotalSupply", null)),
                    It.IsAny<RpcConverter<BigInteger>>()), Times.Once);
        }

        [Fact]
        public async Task TestGetBalance()
        {
            Address address = new Address("hx4873b94352c8c1f3b2f09aaeccea31ce9e90bd31");
            Dictionary<String, RpcValue> @params = new Dictionary<String, RpcValue>();
            @params["address"] = new RpcValue(address);
            Mock<IProvider> provider = GetMockProvider<BigInteger>();

            IconService iconService = new IconService(provider.Object);
            await iconService.GetBalance(address);

            provider.Verify(x =>
                x.Request(It.Is<Request>(request => IsRequestMatches(request, "icx_getBalance", @params)),
                    It.IsAny<RpcConverter<BigInteger>>()), Times.Once);
        }

        [Fact]
        public async Task TestGetBlockByHeight()
        {
            var provider = GetMockProvider<Block>();

            IconService iconService = new IconService(provider.Object);
            var result = await iconService.GetBlock(BigInteger.One);

            Dictionary<String, RpcValue> @params = new Dictionary<String, RpcValue>();
            @params["height"] = new RpcValue(BigInteger.One);

            provider.Verify(x =>
                x.Request(It.Is<Request>(request => IsRequestMatches(request, "icx_getBlockByHeight", @params)),
                    It.IsAny<RpcConverter<Block>>()), Times.Once);
        }

        private static Mock<IProvider> GetMockProvider()
        {
            var provider = new Mock<IProvider>();
            provider
                .Setup(x => x.Request(It.IsAny<Request>(), It.IsAny<RpcConverter<object>>()))
                .ReturnsAsync(null)
                .Verifiable();

            return provider;
        }

        private static Mock<IProvider> GetMockProvider<T>()
        {
            var provider = new Mock<IProvider>();
            provider
                .Setup(x => x.Request<T>(It.IsAny<Request>(), It.IsAny<RpcConverter<T>>()))
                .ReturnsAsync(default(T))
                .Verifiable();

            return provider;
        }

        private static Mock<IProvider> GetMockProvider(Func<Request, bool> expression)
        {
            var provider = new Mock<IProvider>();
            provider
                .Setup(x => x.Request(It.Is<Request>(request => expression(request)), It.IsAny<RpcConverter<object>>()))
                .ReturnsAsync(null)
                .Verifiable();

            return provider;
        }

        [Fact]
        public async Task TestGetBlockByHash()
        {
            var provider = GetMockProvider<Block>();

            Bytes hash = new Bytes("0x033f8d96045eb8301fd17cf078c28ae58a3ba329f6ada5cf128ee56dc2af26f7");

            IconService iconService = new IconService(provider.Object);
            var result = await iconService.GetBlock(hash);

            Dictionary<String, RpcValue> @params = new Dictionary<String, RpcValue>();
            @params["hash"] = new RpcValue(hash);

            provider.Verify(x =>
                x.Request(It.Is<Request>(request => IsRequestMatches(request, "icx_getBlockByHash", @params)),
                    It.IsAny<RpcConverter<Block>>()), Times.Once);
        }

        [Fact]
        public async Task TestGetLastBlock()
        {
            var provider = GetMockProvider<Block>();

            IconService iconService = new IconService(provider.Object);
            var result = await iconService.GetLastBlock();

            provider.Verify(x =>
                x.Request(It.Is<Request>(request => IsRequestMatches(request, "icx_getLastBlock", null)),
                    It.IsAny<RpcConverter<Block>>()), Times.Once);
        }

        [Fact]
        public async Task TestGetScoreApi()
        {
            var provider = GetMockProvider<List<ScoreApi>>();

            Address address = new Address("cx4873b94352c8c1f3b2f09aaeccea31ce9e90bd31");

            IconService iconService = new IconService(provider.Object);
            var result = await iconService.GetScoreApi(address);

            Dictionary<String, RpcValue> @params = new Dictionary<String, RpcValue>();
            @params["address"] = new RpcValue(address);

            provider.Verify(x =>
                x.Request(It.Is<Request>(request => IsRequestMatches(request, "icx_getScoreApi", @params)),
                    It.IsAny<RpcConverter<List<ScoreApi>>>()), Times.Once);
        }

        [Fact]
        public async Task TestGetTransaction()
        {
            var provider = GetMockProvider<ConfirmedTransaction>();

            Bytes hash = new Bytes("0x2600770376fbf291d3d445054d45ed15280dd33c2038931aace3f7ea2ab59dbc");

            IconService iconService = new IconService(provider.Object);
            var result = await iconService.GetTransaction(hash);

            Dictionary<String, RpcValue> @params = new Dictionary<String, RpcValue>();
            @params["txHash"] = new RpcValue(hash);

            provider.Verify(x =>
                x.Request(It.Is<Request>(request => IsRequestMatches(request, "icx_getTransactionByHash", @params)),
                    It.IsAny<RpcConverter<ConfirmedTransaction>>()), Times.Once);
        }

        [Fact]
        public async Task TestGetTransactionResult()
        {
            var provider = GetMockProvider<TransactionResult>();

            Bytes hash = new Bytes("0x2600770376fbf291d3d445054d45ed15280dd33c2038931aace3f7ea2ab59dbc");

            IconService iconService = new IconService(provider.Object);
            var result = await iconService.GetTransactionResult(hash);

            Dictionary<String, RpcValue> @params = new Dictionary<String, RpcValue>();
            @params["txHash"] = new RpcValue(hash);

            provider.Verify(x =>
                x.Request(It.Is<Request>(request => IsRequestMatches(request, "icx_getTransactionResult", @params)),
                    It.IsAny<RpcConverter<TransactionResult>>()), Times.Once);
        }

        internal class TestQueryRpcConverterFactory : RpcConverterFactory
        {
            public RpcConverter<PersonResponse> Create<PersonResponse>()
            {
                return (RpcConverter<PersonResponse>)new TestQueryRpcConverter();
            }

            internal class TestQueryRpcConverter : RpcConverter<PersonResponse>
            {
                public PersonResponse ConvertTo(RpcItem @object)
                {
                    return new PersonResponse();
                }

                public RpcItem ConvertFrom(PersonResponse @object)
                {
                    throw null;
                }
            }
        }

        [Fact]
        public async Task TestQuery()
        {
            var provider = GetMockProvider<PersonResponse>();

            IconService iconService = new IconService(provider.Object);
            iconService.AddConverterFactory<PersonResponse>(new TestQueryRpcConverterFactory());

            Person person = new Person();
            person.Name = "gold bug";
            person.Age = BigInteger.Parse("20");
            person.HasPermission = false;

            Call<PersonResponse> call = new Call<PersonResponse>.Builder()
                    .From(new Address("hxbe258ceb872e08851f1f59694dac2558708ece11"))
                    .To(new Address("cx5bfdb090f43a808005ffc27c25b213145e80b7cd"))
                    .Method("addUser")
                    .Params(person)
                    .BuildWith<PersonResponse>();

            var query = await iconService.CallAsync(call);

            provider.Verify(x =>
                x.Request(It.Is<Request>(request => TestQueryRequestCheck(request, person)),
                    It.IsAny<RpcConverter<PersonResponse>>()), Times.Once);
        }

        private bool TestQueryRequestCheck(Request request, Person person1)
        {
            if (!request.Method.Equals("icx_call"))
            {
                return false;
            }

            RpcObject @params = request.Params;
            RpcObject dataParams = @params.GetItem("data").ToObject()
                .GetItem("params").ToObject();
            return dataParams.GetItem("Name").ToString().Equals(person1.Name) &&
                   dataParams.GetItem("Age").ToInteger().Equals(person1.Age) &&
                   dataParams.GetItem("HasPermission").ToBoolean() == person1.HasPermission;
        }

        [Fact]
        public async Task TestSendIcxTransaction()
        {
            var provider = GetMockProvider<Bytes>();
            Address fromAddress = new Address("hxbe258ceb872e08851f1f59694dac2558708ece11");
            Address toAddress = new Address("hx5bfdb090f43a808005ffc27c25b213145e80b7cd");

            Transaction transaction = TransactionBuilder.NewBuilder()
                    .Nid(NetworkId.MAIN)
                    .From(fromAddress)
                    .To(toAddress)
                    .Value(BigInteger.Parse("0de0b6b3a7640000", NumberStyles.AllowHexSpecifier))
                    .StepLimit(BigInteger.Parse("012345", NumberStyles.AllowHexSpecifier))
                    .Timestamp(BigInteger.Parse("0563a6cf330136", NumberStyles.AllowHexSpecifier))
                    .Nonce(BigInteger.Parse("1"))
                    .Build();
            Wallet wallet = KeyWallet.Load(new Bytes(SampleKeys.PRIVATE_KEY_STRING));
            SignedTransaction signedTransaction = new SignedTransaction(transaction, wallet);

            IconService iconService = new IconService(provider.Object);
            await iconService.SendTransaction(signedTransaction);

            String expected = "xR6wKs+IA+7E91bT8966jFKlK5mayutXCvayuSMCrx9KB7670CsWa0B7LQzgsxU0GLXaovlAT2MLs1XuDiSaZQE=";
            provider.Verify(x =>
                x.Request(It.Is<Request>(request => RequestCheckTestSendIcxTransaction(request, expected)),
                    It.IsAny<RpcConverter<Bytes>>()), Times.Once);
        }

        private static bool RequestCheckTestSendIcxTransaction(Request request, string expected)
        {
            bool isMethodMathces = request.Method.Equals("icx_sendTransaction");
            bool isSignaureMatches = request.Params.GetItem("signature").ToString().Equals(expected);
            return isMethodMathces && isSignaureMatches;
        }

        [Fact]
        public async Task TestTransferTokenTransaction()
        {
            var provider = GetMockProvider<Bytes>();

            Address fromAddress = new Address("hxbe258ceb872e08851f1f59694dac2558708ece11");
            Address scoreAddress = new Address("cx982aed605b065b50a2a639c1ea5710ef5a0501a9");
            Address toAddress = new Address("hx5bfdb090f43a808005ffc27c25b213145e80b7cd");
            new Address("hxbe258ceb872e08851f1f59694dac2558708ece11");

            RpcObject @params = new RpcObject.Builder()
                            .Put("_to", new RpcValue(toAddress))
                            .Put("_value", new RpcValue(BigInteger.Parse("1")))
                            .Build();

            Transaction transaction = TransactionBuilder.NewBuilder()
                    .Nid(NetworkId.MAIN)
                    .From(fromAddress)
                    .To(scoreAddress)
                    .StepLimit(BigInteger.Parse("12345", NumberStyles.HexNumber))
                    .Timestamp(BigInteger.Parse("563a6cf330136", NumberStyles.HexNumber))
                    .Nonce(BigInteger.Parse("1"))
                    .Call("transfer")
                    .Params(@params)
                    .Build();

            Wallet wallet = KeyWallet.Load(new Bytes(SampleKeys.PRIVATE_KEY_STRING));
            SignedTransaction signedTransaction = new SignedTransaction(transaction, wallet);

            IconService iconService = new IconService(provider.Object);
            await iconService.SendTransaction(signedTransaction);

            String expected = "ITpAdh3bUV4Xj0WQIPlfhv+ppA+K+LtXqaYMjnt8pMwV7QJwyZNQuhH2ljdGPR+31wG+GpKEdOEuqeYOwODBVwA=";
            provider.Verify(x =>
                x.Request(It.Is<Request>(request => RequestCheckTestTransferTokenTransaction(request, expected)),
                    It.IsAny<RpcConverter<Bytes>>()), Times.Once);
        }

        private static bool RequestCheckTestTransferTokenTransaction(Request request, string expected)
        {
            bool isMethodMathces = request.Method.Equals("icx_sendTransaction");
            bool isSignaureMatches = request.Params.GetItem("signature").ToString().Equals(expected);
            return isMethodMathces && isSignaureMatches;
        }

        [Fact]
        public async Task TestConverterNotfound()
        {
            var provider = GetMockProvider();

            IconService iconService = new IconService(provider.Object);
            Person person = new Person();
            person.Name = "gold bug";
            person.Age = BigInteger.Parse("20");
            person.HasPermission = false;

            Call<PersonResponse> call = new Call<PersonResponse>.Builder()
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


        private static bool IsRequestMatches(Request request, String method, Dictionary<String, RpcValue> @params)
        {
            if (!request.Method.Equals(method)) return false;
            if (request.Params == null && @params == null) return true;
            if (request.Params != null && @params != null)
            {
                bool isParamMatches = true;
                IEnumerable<String> keys = @params.Keys;
                foreach (String key in keys)
                {
                    RpcValue value = (RpcValue)request.Params.GetItem(key);
                    isParamMatches = value.ToString().Equals(@params[key].ToString());
                    if (!isParamMatches) break;
                }
                return isParamMatches;
            }
            return false;
        }

        internal class Person
        {
            public String Name { get; set; }
            public BigInteger Age { get; set; }
            public bool HasPermission { get; set; }
        }

        internal class PersonResponse
        {
            public bool IsOk { get; set; }
            public String Message { get; set; }
        }
    }
}