using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    public class AnnotatedConverterFactory : RpcConverter.RpcConverterFactory
    {
        public sealed class GenericRpcConverter<T>
        {
            public T ConvertTo(RpcItem @object)
            {
                try
                {
                    T result;
                    var type = typeof(T);
                    try
                    {
                        result = GetClassInstance(type);
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException(e);
                    }

                    RpcObject o = @object.ToObject();
                    FieldInfo[] fields = type.GetFields();
                    foreach (Field field in fields)
                    {
                        field.setAccessible(true);
                        if (field.isAnnotationPresent(typeof(ConverterName))) {
                            ConverterName n = field.getAnnotation(typeof(ConverterName));
                            Object value = FromRpcItem(o.GetItem(n.value()), field.GetType());
                            if (value != null) field.set(result, value);
                        }
                    }
                    return result;
                } catch (Exception e)
                {
                    throw e;
                }
                return null;
            }

            public override RpcItem ConvertFrom(T @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        public RpcConverter<T> Create(Class<T> type)
        {
            return new GenericRpcConverter<T>();
        }

        private T GetClassInstance<T>()
        {
            var type = typeof(T);
            T instance = Activator.CreateInstance<T>();

            return instance;
        }
    }
}