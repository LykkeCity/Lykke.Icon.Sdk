using System.Numerics;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Data;
using Xunit;

namespace Lykke.Icon.Sdk.Tests
{
    public class CallTest
    {
        [Fact]
        private void TestCallBuilder()
        {
            var from = new Address("hx0000000000000000000000000000000000000000");
            var to = new Address("cx1111111111111111111111111111111111111111");
            const string method = "myMethod";
            var person = new Person
            {
                Name = "gold bug",
                Age = BigInteger.Parse("20")
            };

            var call = new Call.Builder()
                    .From(from)
                    .To(to)
                    .Method(method)
                    .Params(person)
                    .BuildWith<PersonResponse>();

            var properties = call.GetProperties();
            var data = properties.GetItem("data").ToObject();
            var dataParams = data.GetItem("params").ToObject();

            Assert.Equal(from, properties.GetItem("from").ToAddress());
            Assert.Equal(to, properties.GetItem("to").ToAddress());
            Assert.Equal(method, data.GetItem("method").ToString());
            Assert.Equal(person.Name, dataParams.GetItem("Name").ToString());
            Assert.Equal(person.Age, dataParams.GetItem("Age").ToInteger());
            Assert.Equal(typeof(PersonResponse), call.GetResponseType());
        }

        [UsedImplicitly]
        public class Person
        {
            public string Name { get; set; }
            public BigInteger Age { get; set; }
        }

        [UsedImplicitly]
        public class PersonResponse
        {
            public bool IsOk { get; set; }
            public string Message { get; set; }
        }

    }
}
