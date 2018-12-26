namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    public interface IRpcConverterFactory
    {
        IRpcConverter<T> Create<T>();
    }
}