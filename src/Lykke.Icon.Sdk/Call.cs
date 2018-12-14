using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Crypto;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk
{
    /**
     * Call contains parameters for querying request.
     *
     * @param <O> Response type
     */
    public class Call<O>
    {
        private RpcObject properties;
        private Type responseType;

        private Call(RpcObject properties)
        {
            this.properties = properties;
            this.responseType = typeof(O);
        }


        public RpcObject GetProperties()
        {
            return properties;
        }

        public Type ResponseType()
        {
            return responseType;
        }

        /**
         * Builder for creating immutable object of Call.<br>
         * It has following properties<br>
         * - {@link #from(Address)} the request account<br>
         * - {@link #to(Address)} the SCORE address to call<br>
         * - {@link #method(String)}  the method name to call<br>
         * - {@link #params(Object)}  the parameter of call<br>
         */
        public class Builder
        {
            private Address from;
            private Address to;
            private String method;
            private RpcItem @params;

            public Builder()
            {
            }

            public Builder From(Address from)
            {
                this.from = from;
                return this;
            }

            public Builder To(Address to)
            {
                if (!IconKeys.IsContractAddress(to))
                    throw new ArgumentException("Only the contract address can be called.");
                this.to = to;
                return this;
            }

            public Builder Method(String method)
            {
                this.method = method;
                return this;
            }

            public Builder Params<I>(I @params)
            {
                this.@params = RpcItemCreator.Create(@params);
                return this;
            }

            public Builder Params(RpcItem @params)
            {
                this.@params = @params;
                return this;
            }

            /**
             * Builds with RpcItem. that means the return type is RpcItem
             *
             * @return Call
             */
            public Call<RpcItem> Build()
            {
                TransactionBuilder.CheckArgument(to, "to not found");
                TransactionBuilder.CheckArgument(method, "method not found");
                return BuildWith<RpcItem>();
            }

            /**
             * Builds with User defined class. an object of the class would be returned
             *
             * @param responseType Response type
             * @param <O> responseType
             * @return Call
             */
            public Call<T> BuildWith<T>()
            {
                RpcObject data = new RpcObject.Builder()
                        .Put("method", new RpcValue(method))
                        .Put("params", @params)
                        .Build();

                RpcObject.Builder propertiesBuilder = new RpcObject.Builder()
                        .Put("to", new RpcValue(to))
                        .Put("data", data)
                        .Put("dataType", new RpcValue("call"));

                // optional
                if (from != null)
                {
                    propertiesBuilder.Put("from", new RpcValue(from));
                }

                return new Call<T>(propertiesBuilder.Build());
            }
        }

    }
}