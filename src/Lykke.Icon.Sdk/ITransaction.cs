using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using System.Numerics;
using System;

namespace Lykke.Icon.Sdk
{
    public interface ITransaction
    {
        BigInteger GetVersion();

        Address GetFrom();

        Address GetTo();

        BigInteger? GetValue();

        BigInteger? GetStepLimit();

        BigInteger? GetTimestamp();

        BigInteger? GetNid();

        BigInteger? GetNonce();

        String GetDataType();

        RpcItem GetData();
    }
}
