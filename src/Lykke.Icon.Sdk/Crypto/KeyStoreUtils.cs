using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Crypto
{
    /**
     * Original Code
     * https://github.com/web3j/web3j/blob/master/crypto/src/main/java/org/web3j/crypto/WalletUtils.java
     * Utility functions for working with Keystore files.
     */
    public class KeyStoreUtils
    {

        private KeyStoreUtils() { }

        private static ObjectMapper objectMapper = new ObjectMapper();

        static KeyStoreUtils()
        {
            objectMapper.configure(JsonParser.Feature.ALLOW_UNQUOTED_FIELD_NAMES, true);
            objectMapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
        }


        public static String GenerateWalletFile(
                KeystoreFile file, File destinationDirectory)
        {
            String fileName = getWalletFileName(file);
            File destination = new File(destinationDirectory, fileName);
            objectMapper.writeValue(destination, file);
            return fileName;
        }

        public static Bytes LoadPrivateKey(String password, File source)
        {
            ObjectMapper mapper = new ObjectMapper();
            KeystoreFile keystoreFile = mapper.readValue(source, KeystoreFile.class);
        if (keystoreFile.getCoinType() == null || !keystoreFile.getCoinType().equalsIgnoreCase("icx"))
            throw new InputMismatchException("Invalid Keystore file");
        return Keystore.decrypt(password, keystoreFile);
    }

    private static String GetWalletFileName(KeystoreFile keystoreFile)
    {
        SimpleDateFormat dateFormat = new SimpleDateFormat("'UTC--'yyyy-MM-dd'T'HH-mm-ss.SSS'--'");
        dateFormat.setTimeZone(TimeZone.getTimeZone("UTC"));
        return dateFormat.format(new Date()) + keystoreFile.getAddress() + ".json";
    }

}
}
