using System;
using System.Collections.Generic;
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

        static RpcItem ToRpcItem(Type type, T item)
        {
            RpcValue rpcValue = toRpcValue(item);
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
            Class < ?> componentType = obj.getClass().getComponentType();
            if (componentType == boolean.class || !componentType.isPrimitive()) {
                RpcArray.Builder builder = new RpcArray.Builder();

                int length = Array.getLength(obj);
                for (int i = 0; i < length; i++)
                {
                    builder.add(toRpcItem(Array.get(obj, i)));
                }

                return builder.build();
            }
            return null;
        }

        static RpcValue toRpcValue(Object object)
        {
            if (object.getClass().isAssignableFrom(Boolean.class)) {
                return new RpcValue((Boolean) object);
            } else if (object.getClass().isAssignableFrom(String.class)) {
                return new RpcValue((String) object);
            } else if (object.getClass().isAssignableFrom(BigInteger.class)) {
                return new RpcValue((BigInteger) object);
            } else if (object.getClass().isAssignableFrom(byte[].class)) {
                return new RpcValue((byte[]) object);
            } else if (object.getClass().isAssignableFrom(Bytes.class)) {
                return new RpcValue((Bytes) object);
            } else if (object.getClass().isAssignableFrom(Address.class)) {
                return new RpcValue((Address) object);
            }
            return null;
        }
    }
}
