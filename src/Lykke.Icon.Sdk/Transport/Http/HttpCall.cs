//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using Lykke.Icon.Sdk;
//using Lykke.Icon.Sdk.Transport.JsonRpc;
//using Org.BouncyCastle.Utilities;
//using Org.BouncyCastle.Utilities.Encoders;

//namespace Lykke.Icon.Sdk.Transport.Http {
///**
// * Http call can be executed by this class
// *
// * @param <T> the data type of the response
// */
//public class HttpCall<T> : Request<T> {

//    private readonly okhttp3.Call httpCall;
//    private readonly RpcConverter<T> converter;

//    HttpCall(okhttp3.Call httpCall, RpcConverter<T> converter) {
//        this.httpCall = httpCall;
//        this.converter = converter;
//    }

//    public T Execute() {
//        return ConvertResponse(httpCall.execute());
//    }

//    public void execute(Callback<T> callback) {
//        httpCall.enqueue(new okhttp3.Callback() {
//            public void onFailure(okhttp3.Call call, IOException e) {
//                callback.onFailure(e);
//            }

//            public void onResponse(
//                    okhttp3.Call call, okhttp3.Response response) {
//                try {
//                    T result = ConvertResponse(response);
//                    callback.onSuccess(result);
//                } catch (IOException e) {
//                    callback.onFailure(e);
//                }
//            }
//        });
//    }

//    // converts the response data from the okhttp response
//    private T ConvertResponse(okhttp3.Response httpResponse){
//        ResponseBody body = httpResponse.body();
//        if (body != null) {
//            ObjectMapper mapper = new ObjectMapper();
//            mapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
//            mapper.registerModule(createDeserializerModule());
//            String content = body.string();
//            Response response = mapper.readValue(content, Response.class);
//            if (converter == null) {
//                throw new IllegalArgumentException("There is no converter for response:'" + content + "'");
//            }
//            if (response.getResult() != null) {
//                return converter.convertTo(response.getResult());
//            } else {
//                throw response.getError();
//            }
//        } else {
//            throw new RpcError(httpResponse.code(), httpResponse.message());
//        }
//    }

//    private SimpleModule createDeserializerModule() {
//        SimpleModule module = new SimpleModule();
//        module.addDeserializer(RpcItem.class, new RpcItemDeserializer());
//        return module;
//    }
//}
//}