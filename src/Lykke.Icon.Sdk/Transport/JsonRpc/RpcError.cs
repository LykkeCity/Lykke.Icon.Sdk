using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{

/**
 * RpcError defines the error that occurred during communicating through jsonrpc
 */
public class RpcError : Exception {
    private long code;
    private String message;

    public RpcError() {
        // jackson needs a default constructor
    }

    public RpcError(long code, String message) :base(message) {
        this.code = code;
        this.message = message;
    }

    /**
     * Returns the code of rpc error
     * @return error code
     */
    public long GetCode() {
        return code;
    }

    /**
     * Returns the message of rpc error
     * @return error message
     */
    public String GetMessage() {
        return message;
    }
}
