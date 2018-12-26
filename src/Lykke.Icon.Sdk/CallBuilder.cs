using System;
using Lykke.Icon.Sdk.Crypto;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk
{
    public partial class Call
    {
        public class Builder
        {
            private Address _from;
            private string _method;
            private RpcItem _params;
            private Address _to;

            public Builder From(Address from)
            {
                _from = from;
                return this;
            }

            public Builder To(Address to)
            {
                if (!IconKeys.IsContractAddress(to))
                {
                    throw new ArgumentException("Only the contract address can be called.");
                }
                
                _to = to;
                
                return this;
            }

            public Builder Method(string method)
            {
                _method = method;
                
                return this;
            }

            public Builder Params(RpcItem @params)
            {
                _params = @params;
                
                return this;
            }

            public Builder Params<T>(T @params)
            {
                _params = RpcItemCreator.Create(@params);
                
                return this;
            }

            /// <summary>
            ///    Builds call with RpcItem type response. 
            /// </summary>
            /// <returns>
            ///    Call instance
            /// </returns>
            public Call<RpcItem> Build()
            {
                TransactionBuilder.CheckArgument(_to, "to not found");
                TransactionBuilder.CheckArgument(_method, "method not found");
                
                return BuildWith<RpcItem>();
            }

            /// <summary>
            ///    Builds call with response of specified type.
            /// </summary>
            /// <typeparam name="T">
            ///    Response type
            /// </typeparam>
            /// <returns>
            ///    Call instance
            /// </returns>
            public Call<T> BuildWith<T>()
            {
                var data = new RpcObject.Builder()
                        .Put("method", new RpcValue(_method))
                        .Put("params", _params)
                        .Build();

                var propertiesBuilder = new RpcObject.Builder()
                        .Put("to", new RpcValue(_to))
                        .Put("data", data)
                        .Put("dataType", new RpcValue("call"));

                // optional
                if (_from != null)
                {
                    propertiesBuilder.Put("from", new RpcValue(_from));
                }

                return new Call<T>(propertiesBuilder.Build());
            }
        }
    }
}