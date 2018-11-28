using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk
{
    public interface Transaction
    {
        BigInteger GetVersion();

        Address GetFrom();

        Address GetTo();

        BigInteger GetValue();

        BigInteger GetStepLimit();

        BigInteger GetTimestamp();

        BigInteger GetNid();

        BigInteger GetNonce();

        String GetDataType();

        RpcItem GetData();
    }
}
