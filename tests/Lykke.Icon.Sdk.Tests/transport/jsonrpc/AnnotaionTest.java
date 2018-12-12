//WTF? :)
public class AnnotaionTest {

    @Test
    void testConvert() {

        RpcObject rpcObject = new RpcObject.Builder()
                .put("boolean", new RpcValue(true))
                .put("string", new RpcValue("string value"))
                .put("BigInteger", new RpcValue(new BigInteger("1234")))
                .put("Address", new RpcValue(new Address("hx4873b94352c8c1f3b2f09aaeccea31ce9e90bd31")))
                .put("bytes", new RpcValue(new Bytes("0xf123")))
                .put("byteArray", new RpcValue(new byte[]{1, 2, 3, 4, 5}))
                .build();

        RpcConverter<AnnotaionClass> converter = new AnnotatedConverterFactory().create(AnnotaionClass.class);
        AnnotaionClass result = converter.convertTo(rpcObject);

        Assertions.assertEquals(rpcObject.getItem("boolean").asBoolean(), result.booleanType);
        Assertions.assertEquals(rpcObject.getItem("string").asString(), result.stringType);
        Assertions.assertEquals(rpcObject.getItem("BigInteger").asInteger(), result.bigIntegerType);
        Assertions.assertEquals(rpcObject.getItem("Address").asAddress(), result.addressType);
        Assertions.assertEquals(rpcObject.getItem("bytes").asBytes(), result.bytesType);
        Assertions.assertArrayEquals(rpcObject.getItem("byteArray").asByteArray(), result.byteArrayType);

    }


    @AnnotationConverter
    public class AnnotaionClass {

        @ConverterName("boolean")
        boolean booleanType;
        @ConverterName("string")
        String stringType;
        @ConverterName("BigInteger")
        BigInteger bigIntegerType;
        @ConverterName("Address")
        Address addressType;
        @ConverterName("bytes")
        Bytes bytesType;
        @ConverterName("byteArray")
        byte[] byteArrayType;

        public AnnotaionClass() {
        }

        @Override
        public String toString() {
            return "AnnotaionClass{" +
                    "booleanType=" + booleanType +
                    ", stringType='" + stringType + '\'' +
                    ", bigIntegerType=" + bigIntegerType +
                    ", addressType=" + addressType +
                    ", bytesType=" + bytesType +
                    ", byteArrayType=" + Arrays.toString(byteArrayType) +
                    '}';
        }
    }
}
