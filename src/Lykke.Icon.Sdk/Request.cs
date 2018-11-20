using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk
{
    /**
     * Request class executes the request that has been prepared
     */
    public interface Request<T>
    {
        /**
         * Executes synchronously
         *
         * @return Response
         * @throws IOException an exception if there exist errors
         */
        T Execute();

        /**
         * Executes asynchronously
         *
         * @param callback the callback is invoked when the execution is completed
         */
        void Execute(Callback<T> callback);
    }
}