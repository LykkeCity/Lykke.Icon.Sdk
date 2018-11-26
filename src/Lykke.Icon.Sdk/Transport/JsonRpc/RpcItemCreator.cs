using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    public class RpcItemCreator
    {
        public static RpcItem Create<T>(T item)
        {
            return ToRpcItem(item);
        }

        static RpcItem ToRpcItem(T item)
        {
            return item != null ? ToRpcItem(item.GetType(), item) : null;
        }

        static RpcItem ToRpcItem<T>(Type type, T item)
        {
            RpcValue rpcValue = ToRpcValue(item);
            if (rpcValue != null)
            {
                return rpcValue;
            }

            if (type.IsArray())
            {
                return toRpcArray(item);
            }

            if (!type.IsPrimitive())
            {
                return toRpcObject(item);
            }

            return null;
        }

        static RpcObject ToRpcObject(Object @object)
        {
            RpcObject.Builder builder = new RpcObject.Builder();
            AddObjectFields(builder, @object, object.getClass().getDeclaredFields());
            AddObjectFields(builder, @object, object.getClass().getFields());
            return builder.build();
        }

        static String GetKeyFromObjectField(Field field)
        {
            return field.getName();
        }

        static void AddObjectFields(RpcObject.Builder builder, Object parent, Field[] fields)
        {
            foreach (Field field in fields)
            {
                String key = GetKeyFromObjectField(field);
                if (key.equals("this$0")) continue;

                Type type = field.GetType();
                Object fieldObject = null;
                try
                {
                    field.SetAccessible(true);
                    fieldObject = field.Get(parent);
                }
                catch (Exception ignored)
                {
                }

                if (fieldObject != null || !type.isInstance(fieldObject))
                {
                    RpcItem rpcItem = toRpcItem(type, fieldObject);
                    if (rpcItem != null && !rpcItem.isEmpty())
                    {
                        builder.put(key, rpcItem);
                    }
                }
            }
        }

        static RpcArray ToRpcArray(Object obj)
        {
            Type componentType = obj.GetType();
            if (componentType == typeof(bool) || !componentType.IsPrimitive) {
                RpcArray.Builder builder = new RpcArray.Builder();

                int length = ((IEnumerable)obj).Count();
                for (int i = 0; i < length; i++)
                {
                    builder.add(ToRpcItem(Array.get(obj, i)));
                }

                return builder.build();
            }
            return null;
        }

        static RpcValue ToRpcValue(Object @object)
        {
            var objectType = @object.GetType();
            if (objectType.IsAssignableFrom(typeof(bool))) {
                return new RpcValue((bool) @object);
            } else if (objectType.IsAssignableFrom(typeof(String))) {
                return new RpcValue((String) @object);
            } else if (objectType.IsAssignableFrom(typeof(BigInteger))) {
                return new RpcValue((BigInteger) @object);
            } else if (objectType.IsAssignableFrom(typeof(byte[]))) {
                return new RpcValue((byte[]) @object);
            } else if (objectType.IsAssignableFrom(typeof(Bytes))) {
                return new RpcValue((Bytes) @object);
            } else if (objectType.IsAssignableFrom(typeof(Address))) {
                return new RpcValue((Address) @object);
            }
            return null;
        }
    }
}
