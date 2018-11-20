using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    public interface ConverterName
    {
        /**
         * @return the desired name of the field when it is converted
         */
        String Value();
    }
}