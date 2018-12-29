using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Xunit;

namespace Lykke.Icon.Sdk.Tests.Utils
{
    public static class TransactionAssertion
    {
        public static void CompareTransactions(ITransaction transaction, ITransaction deserialized)
        {
            Assert.Equal(transaction.GetVersion(), deserialized.GetVersion());
            Assert.Equal(transaction.GetDataType(), deserialized.GetDataType());
            Assert.Equal(transaction.GetFrom(), deserialized.GetFrom());
            Assert.Equal(transaction.GetNid(), deserialized.GetNid());
            Assert.Equal(transaction.GetNonce(), deserialized.GetNonce());
            Assert.Equal(transaction.GetStepLimit(), deserialized.GetStepLimit());
            Assert.Equal(transaction.GetTo(), deserialized.GetTo());
            Assert.Equal(transaction.GetTimestamp(), deserialized.GetTimestamp());
            Assert.Equal(transaction.GetValue(), deserialized.GetValue());

            CompareTransactionsData(transaction.GetData(), deserialized.GetData());
        }

        private static void CompareTransactionsData(RpcItem expectedData, RpcItem actualData)
        {
            if (expectedData == null && actualData == null)
                return;

            if (expectedData != null && actualData == null)
                throw new ArgumentException("actual data is null");

            if (expectedData is RpcValue expectedRpcValue &&
                actualData is RpcValue actualRpcValue)
            {
                Assert.Equal(expectedRpcValue.ToString(), actualRpcValue.ToString());
                return;
            }

            if (expectedData is RpcObject expectedRpcObject &&
                actualData is RpcObject actualRpcObject)
            {
                CompareTransactionsDataRpcObjects(expectedRpcObject, actualRpcObject);
                return;
            }


            throw new ArgumentException("can't compare transactions data");
        }

        private static void CompareTransactionsDataRpcObjects(RpcObject expectedRpcObject, RpcObject actualRpcObject)
        {
            var expectedKeys = expectedRpcObject.GetKeys().OrderBy(x => x).ToList();
            var actualKeys = actualRpcObject.GetKeys().OrderBy(x => x);
            Assert.True(expectedKeys.SequenceEqual(actualKeys));

            foreach (var key in expectedKeys)
            {
                var expected = expectedRpcObject.GetItem(key);
                var actual = actualRpcObject.GetItem(key);

                if (expected is RpcValue expectedRpcValue &&
                    actual is RpcValue actualRpcValue)
                {
                    Assert.Equal(expectedRpcValue.ToString(), actualRpcValue.ToString());
                    continue;
                }

                if (expected is RpcObject expectedObj &&
                    actual is RpcObject actualObj)
                {
                    CompareTransactionsDataRpcObjects(expectedObj, actualObj);
                    continue;
                }

                throw new ArgumentException("Can't compare key values");
            }
        }

    }
}
