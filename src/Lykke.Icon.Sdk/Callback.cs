using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk
{

/**
 *  A callback of the asynchronous execution
 */
public interface Callback<T> {

    /**
     * Invoked when the execution is successful
     * @param result a result of the execution
     */
    void OnSuccess(T result);

    /**
     * Invoked when the execution is completed with an exception
     * @param exception an exception thrown during the execution
     */
    void OnFailure(Exception exception);
}
}