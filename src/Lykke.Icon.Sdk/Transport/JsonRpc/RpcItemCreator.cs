using Lykke.Icon.Sdk.Data;
using System.Numerics;
using System;
using System.Collections;
using System.Reflection;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    public class RpcItemCreator
    {
        public static RpcItem Create<T>(T item)
        {
            return ToRpcItem(item);
        }

        static RpcItem ToRpcItem<T>(T item)
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

            if (type.IsArray)
            {
                return ToRpcArray(item);
            }

            if (!type.IsPrimitive)
            {
                return ToRpcObject(item);
            }

            return null;
        }

        static RpcObject ToRpcObject(Object @object)
        {
            RpcObject.Builder builder = new RpcObject.Builder();
            //AddObjectFields(builder, @object, @object.GetType().GetFields(BindingFlags.Instance 
            //                                                              | BindingFlags.Public));
            var props = @object.GetType().GetProperties(BindingFlags.Instance
                                                        | BindingFlags.Public 
                                                        | BindingFlags.NonPublic);
            AddObjectFields(builder, @object, props);
            return builder.Build();
        }

        static String GetKeyFromObjectField(MemberInfo field)
        {
            return field.Name;
        }

        static void AddObjectFields(RpcObject.Builder builder, Object parent, PropertyInfo[] fields)
        {
            foreach (var field in fields)
            {
                String key = GetKeyFromObjectField(field);
                if (key.Equals("this$0")) continue;

                Type type = field.PropertyType;
                Object fieldObject = null;
                try
                {
                    fieldObject = field.GetValue(parent);
                }
                catch (Exception ignored)
                {
                }

                if (fieldObject != null || !type.IsAssignableFrom(fieldObject.GetType()))
                {
                    RpcItem rpcItem = ToRpcItem(type, fieldObject);
                    if (rpcItem != null && !rpcItem.IsEmpty())
                    {
                        builder.Put(key, rpcItem);
                    }
                }
            }
        }

        static RpcArray ToRpcArray(Object obj)
        {
            Type componentType = obj.GetType();
            if (componentType == typeof(bool) || !componentType.IsPrimitive)
            {
                RpcArray.Builder builder = new RpcArray.Builder();

                var castedArray = (IEnumerable)obj;
                foreach (var item in castedArray)
                {
                    builder.Add(ToRpcItem(item));
                }

                return builder.Build();
            }
            return null;
        }

        static RpcValue ToRpcValue(Object @object)
        {
            var objectType = @object.GetType();
            if (objectType.IsAssignableFrom(typeof(bool)))
            {
                return new RpcValue((bool)@object);
            }
            else if (objectType.IsAssignableFrom(typeof(String)))
            {
                return new RpcValue((String)@object);
            }
            else if (objectType.IsAssignableFrom(typeof(BigInteger)))
            {
                return new RpcValue((BigInteger)@object);
            }
            else if (objectType.IsAssignableFrom(typeof(byte[])))
            {
                return new RpcValue((byte[])@object);
            }
            else if (objectType.IsAssignableFrom(typeof(Bytes)))
            {
                return new RpcValue((Bytes)@object);
            }
            else if (objectType.IsAssignableFrom(typeof(Address)))
            {
                return new RpcValue((Address)@object);
            }
            return null;
        }
    }
}
