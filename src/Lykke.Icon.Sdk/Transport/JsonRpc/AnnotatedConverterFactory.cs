using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{

public class AnnotatedConverterFactory : RpcConverter.RpcConverterFactory {

    public RpcConverter<T> Create(Class<T> type) {
        return new RpcConverter<T>() {
            public T convertTo(RpcItem @object) {
                try {
                    T result;
                    try {
                        result = getClassInstance(type);
                    } catch (ClassNotFoundException | NoSuchMethodException | InvocationTargetException e) {
                        throw new IllegalArgumentException(e);
                    }

                    RpcObject o = object.asObject();
                    Field[] fields = type.getDeclaredFields();
                    for (Field field : fields) {
                        field.setAccessible(true);
                        if (field.isAnnotationPresent(ConverterName.class)) {
                            ConverterName n = field.getAnnotation(ConverterName.class);
                            Object value = fromRpcItem(o.getItem(n.value()), field.getType());
                            if (value != null) field.set(result, value);
                        }
                    }
                    return result;
                } catch (InstantiationException | IllegalAccessException e) {
                    e.printStackTrace();
                }
                return null;
            }

            @Override
            public RpcItem convertFrom(T object) {
                return RpcItemCreator.create(object);
            }
        };

    }

    private <T> T getClassInstance(Class<T> type) throws IllegalAccessException, InstantiationException, ClassNotFoundException, NoSuchMethodException, InvocationTargetException {
        if (isInnerClass(type)) {
            String className = type.getCanonicalName().subSequence(0, type.getCanonicalName().length() - type.getSimpleName().length() - 1).toString();
            Class m = Class.forName(className);
            return type.getConstructor(m).newInstance(m.newInstance());
        }
        return type.newInstance();
    }

    private bool isInnerClass(Class<?> clazz) {
        return clazz.isMemberClass() && !Modifier.isStatic(clazz.getModifiers());
    }
}
}