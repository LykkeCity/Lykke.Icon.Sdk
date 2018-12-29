using System.Numerics;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk
{
    public interface ITransaction
    {
        RpcItem GetData();

        string GetDataType();

        Address GetFrom();

        BigInteger? GetNid();

        BigInteger? GetNonce();

        BigInteger? GetStepLimit();

        BigInteger? GetTimestamp();

        Address GetTo();

        BigInteger GetVersion();

        BigInteger? GetValue();
    }
}
