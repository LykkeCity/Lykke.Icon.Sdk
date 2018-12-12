using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using System.Numerics;
using System;
using Xunit;

namespace Lykke.Icon.Sdk.Tests
{
    public class CallTest
    {
        [Fact]
        void TestCallBuilder()
        {
            Address from = new Address("hx0000000000000000000000000000000000000000");
            Address to = new Address("cx1111111111111111111111111111111111111111");
            String method = "myMethod";
            Person person = new Person();
            person.Name = "gold bug";
            person.Age = BigInteger.Parse("20");

            Call<PersonResponse> call = new Call<PersonResponse>.Builder()
                    .From(from)
                    .To(to)
                    .Method(method)
                    .Params(person)
                    .BuildWith<PersonResponse>();

            RpcObject properties = call.GetProperties();
            RpcObject data = properties.GetItem("data").ToObject();
            RpcObject dataParams = data.GetItem("params").ToObject();

            Assert.Equal(from, properties.GetItem("from").ToAddress());
            Assert.Equal(to, properties.GetItem("to").ToAddress());
            Assert.Equal(method, data.GetItem("method").ToString());
            Assert.Equal(person.Name, dataParams.GetItem("Name").ToString());
            Assert.Equal(person.Age, dataParams.GetItem("Age").ToInteger());
            Assert.Equal(typeof(PersonResponse), call.ResponseType());
        }


        internal class Person
        {
            public String Name { get; set; }
            public BigInteger Age { get; set; }
        }

        internal class PersonResponse
        {
            public bool IsOk { get; set; }
            public String Message { get; set; }
        }

    }
}
