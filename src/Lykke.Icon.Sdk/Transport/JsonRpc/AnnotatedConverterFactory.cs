using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    //public class AnnotatedConverterFactory : RpcConverterFactory
    //{
    //    public sealed class GenericRpcConverter<T> : RpcConverter<T>
    //    {
    //        public T ConvertTo(RpcItem @object)
    //        {
    //            try
    //            {
    //                T result;
    //                var type = typeof(T);
    //                try
    //                {
    //                    result = Activator.CreateInstance<T>();
    //                }
    //                catch (Exception e)
    //                {
    //                    throw e;
    //                }

    //                RpcObject o = @object.ToObject();
    //                FieldInfo[] fields = type.GetFields();
    //                foreach (var field in fields)
    //                {
    //                    //field.setAccessible(true);
    //                    if (field.isAnnotationPresent(typeof(ConverterName))) {
    //                        ConverterName n = field.getAnnotation(typeof(ConverterName));
    //                        Object value = FromRpcItem(o.GetItem(n.value()), field.GetType());
    //                        if (value != null) field.set(result, value);
    //                    }
    //                }
    //                return result;
    //            } catch (Exception e)
    //            {
    //                throw e;
    //            }

    //            return default(T);
    //        }

    //        public RpcItem ConvertFrom(T @object)
    //        {
    //            return RpcItemCreator.Create(@object);
    //        }
    //    }

    //    public RpcConverter<T> Create<T>()
    //    {
    //        return new GenericRpcConverter<T>();
    //    }

    //    private T GetClassInstance<T>()
    //    {
    //        var type = typeof(T);
    //        T instance = Activator.CreateInstance<T>();

    //        return instance;
    //    }
    //}
}