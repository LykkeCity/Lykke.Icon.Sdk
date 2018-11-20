using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Data
{

    /**
     * Defines network ids
     */
    public enum NetworkId
    {

        MAIN = 1,
        TEST = 2

        //private BigInteger nid;

        //NetworkId(BigInteger nid)
        //{
        //    this.nid = nid;
        //}

        //public BigInteger getValue()
        //{
        //    return nid;
        //}
    }
}
