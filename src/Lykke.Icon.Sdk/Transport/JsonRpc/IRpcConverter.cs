namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    public interface IRpcConverter<T>
    {
        T ConvertTo(RpcItem @object);
        
        RpcItem ConvertFrom(T @object);
    }
}
