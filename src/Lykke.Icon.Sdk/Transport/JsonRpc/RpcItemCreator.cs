using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Lykke.Icon.Sdk.Data;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    public static class RpcItemCreator
    {
        public static RpcItem Create<T>(T item)
        {
            return ToRpcItem(item);
        }

        private static RpcItem ToRpcItem<T>(T item)
        {
            return item != null ? ToRpcItem(item.GetType(), item) : null;
        }

        private static RpcItem ToRpcItem<T>(Type type, T item)
        {
            var rpcValue = ToRpcValue(item);
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

        private static RpcObject ToRpcObject(object @object)
        {
            var builder = new RpcObject.Builder();
            
            var props = @object.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            AddObjectFields(builder, @object, props);
            
            return builder.Build();
        }

        private static string GetKeyFromObjectField(MemberInfo field)
        {
            return field.Name;
        }

        private static void AddObjectFields(RpcObject.Builder builder, object parent, IEnumerable<PropertyInfo> fields)
        {
            foreach (var field in fields)
            {
                var key = GetKeyFromObjectField(field);
                if (key.Equals("this$0")) continue;

                var type = field.PropertyType;
                
                object fieldObject;
                
                try
                {
                    fieldObject = field.GetValue(parent);
                }
                catch (Exception)
                {
                    fieldObject = null;
                }

                // ReSharper disable once ExpressionIsAlwaysNull
                if (fieldObject != null || !type.IsInstanceOfType(fieldObject))
                {
                    var rpcItem = ToRpcItem(type, fieldObject);
                    
                    if (rpcItem != null && !rpcItem.IsEmpty())
                    {
                        builder.Put(key, rpcItem);
                    }
                }
            }
        }

        private static RpcArray ToRpcArray(object obj)
        {
            var componentType = obj.GetType();
            if (componentType == typeof(bool) || !componentType.IsPrimitive)
            {
                var builder = new RpcArray.Builder();

                var castedArray = (IEnumerable)obj;
                foreach (var item in castedArray)
                {
                    builder.Add(ToRpcItem(item));
                }

                return builder.Build();
            }
            return null;
        }

        private static RpcValue ToRpcValue(object @object)
        {
            var objectType = @object.GetType();
            
            if (objectType.IsAssignableFrom(typeof(bool)))
            {
                return new RpcValue((bool)@object);
            }
            else if (objectType.IsAssignableFrom(typeof(string)))
            {
                return new RpcValue((string)@object);
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
