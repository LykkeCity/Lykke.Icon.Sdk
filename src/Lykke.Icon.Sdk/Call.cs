using System;
using System.Collections.Generic;
using System.Text;
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
        private RpcObject _properties;
        private Type _responseType;

        private Call(RpcObject properties)
        {
            this.properties = properties;
            this.responseType = typeof(O);
        }


        RpcObject getProperties()
        {
            return properties;
        }

        Type responseType()
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
        public static class Builder
        {
            private Address from;
            private Address to;
            private String method;
            private RpcItem @params;

            public Builder()
            {
            }

            public Builder from(Address from)
            {
                this.from = from;
                return this;
            }

            public Builder to(Address to)
            {
                if (!IconKeys.isContractAddress(to))
                    throw new IllegalArgumentException("Only the contract address can be called.");
                this.to = to;
                return this;
            }

            public Builder method(String method)
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
            public Call<RpcItem> build()
            {
                checkArgument(to, "to not found");
                checkArgument(method, "method not found");
                return buildWith(typeof(RpcItem));
            }

            /**
             * Builds with User defined class. an object of the class would be returned
             *
             * @param responseType Response type
             * @param <O> responseType
             * @return Call
             */
            public Call<O> buildWith<O>()
            {
                RpcObject data = new RpcObject.Builder()
                        .put("method", new RpcValue(method))
                        .put("params", @params)
                        .build();

                RpcObject.Builder propertiesBuilder = new RpcObject.Builder()
                        .put("to", new RpcValue(to))
                        .put("data", data)
                        .put("dataType", new RpcValue("call"));

                // optional
                if (from != null)
                {
                    propertiesBuilder.put("from", new RpcValue(from));
                }

                return new Call<O>(propertiesBuilder.build());
            }
        }

    }
}