using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Tests.Utils;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Org.BouncyCastle.Utilities.Encoders;
using Xunit;

namespace Lykke.Icon.Sdk.Tests
{
    public class TransactionDeserializerTest
    {
        [Fact]
        public void TestIcxDeserialize()
        {
            var from = new Address("hxbe258ceb872e08851f1f59694dac2558708ece11");
            var to = new Address("hx5bfdb090f43a808005ffc27c25b213145e80b7cd");
            var transaction = TransactionBuilder.CreateBuilder()
                .Nid(NetworkId.Main)
                .From(from)
                .To(to)
                .Value(BigInteger.Parse("0de0b6b3a7640000", NumberStyles.AllowHexSpecifier))
                .StepLimit(BigInteger.Parse("12345", NumberStyles.AllowHexSpecifier))
                .Timestamp(BigInteger.Parse("563a6cf330136", NumberStyles.AllowHexSpecifier))
                .Nonce(BigInteger.Parse("1"))
                .Build();

            var serialized =
                "icx_sendTransaction.from.hxbe258ceb872e08851f1f59694dac2558708ece11.nid.0x1.nonce.0x1.stepLimit.0x12345.timestamp.0x563a6cf330136.to.hx5bfdb090f43a808005ffc27c25b213145e80b7cd.value.0xde0b6b3a7640000.version.0x3";

            var deserialized = TransactionDeserializer.Deserialize(serialized);

            TransactionAssertion.CompareTransactions(transaction, deserialized);
        }

        [Fact]
        public void TestCallTransactionDeserialize()
        {
            var @params = new RpcObject.Builder()
                .Put("_to", new RpcValue("hx5bfdb090f43a808005ffc27c25b213145e80b7cd"))
                .Put("_value", new RpcValue(BigInteger.Parse("1")))
                .Build();

            var from = new Address("hxbe258ceb872e08851f1f59694dac2558708ece11");
            var to = new Address("cx982aed605b065b50a2a639c1ea5710ef5a0501a9");

            var transaction = TransactionBuilder.CreateBuilder()
                .Nid(NetworkId.Main)
                .From(from)
                .To(to)
                .Value(BigInteger.One)
                .StepLimit(BigInteger.Parse("75000"))
                .Timestamp(BigInteger.Parse("5727e42882650", NumberStyles.HexNumber))
                .Nonce(BigInteger.Parse("1"))
                .Call("transfer")
                .Params(@params)
                .Build();

            var serialized =
                "icx_sendTransaction.data.{method.transfer.params.{_to.hx5bfdb090f43a808005ffc27c25b213145e80b7cd._value.0x1}}.dataType.call.from.hxbe258ceb872e08851f1f59694dac2558708ece11.nid.0x1.nonce.0x1.stepLimit.0x124f8.timestamp.0x5727e42882650.to.cx982aed605b065b50a2a639c1ea5710ef5a0501a9.value.0x1.version.0x3";
            var deserialized = TransactionDeserializer.Deserialize(serialized);

            TransactionAssertion.CompareTransactions(transaction, deserialized);
        }

        [Fact]
        public void TestDeployTransactionDeserialize()
        {
            var content = "504B03040A0000000000C45DF34C0000000000000000000000000F0010007374616E646172645F746F6B656E2F55580C00C33A505B70FB4F5BF5011400504B03041400080008001C57F24C0000000000000000000000001A0010007374616E646172645F746F6B656E2F5F5F696E69745F5F2E707955580C005028505B679E4E5BF50114004B2BCACF55D02B2E49CC4B492C4A892FC9CF4ECD53C8CC2DC82F2A5108868A868004B900504B070869020BAC240000002A000000504B03040A00000000003D5AF24C0000000000000000000000001B0010007374616E646172645F746F6B656E2F5F5F707963616368655F5F2F55580C00C33A505B55A34E5BF5011400504B03041400080008003D5AF24C000000000000000000000000320010007374616E646172645F746F6B656E2F5F5F707963616368655F5F2F5F5F696E69745F5F2E63707974686F6E2D33362E70796355580C005028505B55A34E5BF501140033E6E5E54A9FE717ADC5C0C0F09801093001B10310170B0089148614C61C865CC62846468614A660064DE6978C40614DC65BBCC12589792989452921F9D9A9797E9A4CB7F88AA122F12520A12290412B198A58801498F8E5ACA7AF579C9C5F94AA6F6068609464686C6264916891989A6664669A9694989A9A646C6E6C6992629C986A616E649E669E6868A96F186FA01F1F9F999759121FAF5750798BC326373FA53427D50EE48E62908301504B07088ABD28B9AC000000CA000000504B03041400080008003D5AF24C000000000000000000000000380010007374616E646172645F746F6B656E2F5F5F707963616368655F5F2F7374616E646172645F746F6B656E2E63707974686F6E2D33362E70796355580C005028505B55A34E5BF5011400A557DF73DBC611C6E10010A4484AA269FD70EC4675D236CCA49225B991D371DA4A769D7147A533A59A99B29D4141DC51250D92EA0172220DFD24CDE4A5F9071AA5EFFD97F0DA27FF0B79EAEE02A000D292DA893D58018BE3EDB7DFED7DB7D8AE544A87FF6AFE599BD3B4FF68997F265CBF812BD8072334C17CAD8D7F759F1D6882B7F5CF346108F35C13A6D4FAFC8CB70DF058A2009E8234E1D982675B14E1B9280BD23833DAB6602DAD517A83F3371B2C621FB62B8393963B38F2E5C1E8A51C7A5904568AE01F60A4D666426BEB92F70D690A76C1842EF8B971C6DA56C68788D057209F4D3EC484BE62C687B8D05722DF1C784B17BA981365F296D12B16D12F2BA27AC1C5BC583837E14D555420835A5421B8ADD01D0A570948C552323C56438F25E059723DC1040CA2107EC89AFF06131981F4BB0A1926F3FDF3F58DF5C01B29B9F160F3C1566773FBE1D623F7912BBB5B1FFFA2DB71A5EC6CEF6C7FF2506CBBF2D1CED64E77C7DDFC6463D379B01124009C10E1AC1F9D44C6D01DC82206D511867E5ACB215DC7D7378224540DA6AC14616C0A60222B381974467E2513A29E0F110FF841416C21BDDEC0F583C54C98A57C9874C80F0A34178E42D76F1D1F1DF927B733B156F3B132A31A7A6439A32F8752298CE1E94974FDDAE87A1CBDFC1608C58EEBBB434FBEE8AE66002CE7014CC6341B3CE2B0DC00E295EB1FCBC874841BBA9E91C030AE8561C430AA68E6D12CBC8DFC50B9C3A02BD5BDABC94F87C0CC8DF9C8761CAC2BC7894A8E331889631FEFCB8EF3F763D74FDE70B7E34555B713C04FBD7020C3BF8D44C4E149D918194B36E2BD61A84AF838878F855D21940C0285C516999D9350066A318F78025B71303855B08237CC600B3C7FD5E0A251399599E8DC7D2DAB321214455AA0157CA2330561009566547BA2465F8A96EBCBE7C350AAAEEB495814D3E9AAD1E092D5FF6F45EE5EBB2215DADECF5CDFEFB8DECBF732CBB2368B653D37B8D9B0550D27BA85A64E05D74B872AAC38857CA93B68EE5DC5ED3B60EEA7F86DA633F2CCA8B546F4B34B1E056B037F6D54672E8C6F98B4E07F01D4D6BC3032BA5C423DBED0BFD6BA4C94C4DC79B93D07A30AE02D5F70F2820A83B70C9EF90B469E05B1089E8AAC8A9AB80533CFE31B5117B769CE8529FF925826FFE2C45F20FF8A58257F6DCA7F47BC43FE5B133FE01677E18CB8277E446FEAF0E6FD249B77A14AD6C48FE984B89DCBF1BE78EFDC682F7D0D55D05E8665FF495449F7106DA8B69DECEC202A93C83801A9CC1B4E9A15157A4321BF9202EAE4EEECCEF5CC4C115F5D6366A6C66ED8FA07C9BEFE7976EBE710AFA7439AA084BAE8CCAA204E87F083876042ED4CEBB3313B634C0BF531740A63161A827DC3C79A638616780A198F4DC0756C0E4AA46F4E7872241BC5C8045EA442A5E90D7B21E889F985AB9EEE4515E7E0C5C1EEBED3FAE3E79FEFFF896A197C592A23EB69CF0B6168D1D9DBDDDF6D3EF96D0B6E53DA53657E3FE6BBE8389EEF0681E364AAFF6760363121AA71566635364D4A0A0B84A08277BDF4B888E58C38E21941E02947CF261C9DC10A8E996063FD9656D75EF350EFC3DE19F36FB5EFF4D038D3E1ADD637FBD6982397634ECC697DBB5F7C1C2F376FBEC160A7ABA321A081D6C0F77FB99665E2D34649FD14092A5D8E88ACFDD1E121306B0AD9393E8CF8C1EE67EA431CC40319521D447C101CC2B92FA114552A581B6828B735624EADE71503F9FA18132C1167251D5883EB7425CFDB25109882589A3ECD89A56A8EA5A4B861E9289B224C727C04A7A04C0EF959348FC0FC7A52D2EC747906453CC1019480ADA42B4643FFE49AC6825191AA77D3286FEF2EE864DBCBECA45A3EEC0DBD581C24E2BF6F1D5C17044FCE679920F57C901BBBB138CC9BDA0DB9E03AFEEE6A55B8BE1BC31F43ADD2FAA56B1717D9A10CAF08882D40331370351F30D792C58DD58C12E9D3F1C76C3589CFA8B4AFE9CBA8E9F84326FE54CD4C3AB2069F56E5ECC96FA6105060C65C682F6D5516EC35273840485F1FC396874D6D2485AD37695D9AA321B61545276DB5146E27B5A3FD8F6D1C354A5F68A96AE9D8114D2F5A3AF5E41CC1A1A514F13F33A48DF94B681410AF2029BFF4AF24F78FE3BF3AFA96937BF0E97D5DFD95649F44ECB53936FB2664CCC7C619A779AC387F780606C242DF06D163DFC227E65827CBC91A608DEF8A6111859078329BA7D517C7E1DAA8BB96ACC569313D9B3E38D53F5AFB9E3540F048C1E0EBEC95546134D70B1C6F34A416345AF260A787D299B4430E7D83516B13B742D869A92D34DB68F03C6B58571EA6ED790F9BB1009AB164A6A915F914CC5F90D91A6D902AA8629DD59905274A952DCCA892936DB257A6BA38F5119A062667436EC3D01F1D4EB773083F5A7C0E09B710CE53F84EE8B881A4F38C049A74117EFF151030747DB5AC25A245A272D989D35E883B6FCC010ED8F894F4A4EFA72765221DEB936C511C3FC06CD1D8BAC10CA8C1255E37978A356E43479EBF6A5C674B26FDAA5154F82D06AB05D001EFAB1EB4AA0F492E76F79E500B1F5527FD6E6BB268516592EB1EE63991E79936E771FC91F22B968A83AD03425EBBB360FD17504B0708E9FCF1AD4507000011110000504B03041400080008001C57F24C0000000000000000000000001B0010007374616E646172645F746F6B656E2F7061636B6167652E6A736F6E55580C005D39505B679E4E5BF5011400ABE6520002A5B2D4A2E2CCFC3C252B0525033D033D43251D88786E62665E7C5A664E2A48A6B824312F25B12825BE243F3B350F454971727E11584D30544D085809572D00504B0708488DC46F4400000060000000504B0304140008000800FC81F34C000000000000000000000000200010007374616E646172645F746F6B656E2F7374616E646172645F746F6B656E2E707955580C00AB3A505BAB3A505BF5011400AD56516FA338107EE75758790974B3D19DEE2DBAAC4ADA5E55296D5722B7D26A55210326870A36B24DB751D5FFBE636313709236D91E2F608FE79BF17C1F1E1755CDB84438493D2FE7AC4245CAA820FCA948092A5AE399E7ADC26B3447E36A13E1AA2EC98A3D123AF63C2F2DB110480F2389698679E603D6345C5C04330FC173AE86381192E3545644FEC7323D9F911C515C115F90320FD0E72F0896B42EEAA901D77B17406CAA84951F82C8485A54B8145B9082CA53412493B88C9ABA2E371FC2497089694AEE738D324131FB49099FA130CB3811E2B7D3E3988A9C708B2A590709A3275C3664A670619061896728D94822E6778C92C08D6528BFE0EC6716E192DC5049788E53E2775F51CAB8753C2FEC6CAF58A0967F705926387DB42929ED1D93D4A17CACFAB414FD1B10B1CE628105990CF509081A225E84CBF0EEE22A52C2368517E3D6B4BA5F85CB38FAF7EBD7E57765D6FCC642136C965C5E5DDCDC864BED6D35644C77E1ED959A56F23653D1F7DBC5FD524DB6921D1BD2C813A1B2646BBFA0197926D9FCAFA0ABD3CA61CD2DD1712CEE14CCC2C771410B19C7063E4B66A82BDB25F827503A2D3825832D081401520AA69D7B96045B23404DE37EB560CBDF30BF5CF8ADA95F5815738274DEB1DCD4640EB9BB50B6B20E8C2DFE31108A05C75D11B4E30A7F8EEBDA72E538B7541EE36E45050097452A3B042BBCFDD97704310A251650CAD250A40A5ED873C6506D0B64866AB333750C42CCD1E09F184DCC69D9996FA3D5E80D827BE14D52EA71B81DA484CED09F7FA0B3B32EA9CE6BC9D66BC2A719499AB59F8FB7D0B301E0FCA53F7A1DC39F1B5EF782EFCA6B2A88F4FB138704A417DAC13E89E805EA63BF08B4B9FD3C44F30F3DACC41AD6C2DFCC1FA03EFDD406CC3635FCA3BDE6779003B330B007C6331CA714973E273863146AB6E20DD91E1A6FB4544E64C3697FCB6BD8D3B1C06FB6DA01B429D829E0EF34E1017CC7E82901DE6FD0831803899D12E7E4063E08DB29A9757B7082FECFFDBCC85B332A84233FF5B4A6394AC6CA3676FFC17E063DCDEB646C0E267CEF488BDDCC3FDCD360136EED14E603FADBBA0F76A5D772E8BA5CFAA3FB4622D67136DA39685CC8F981F9CF26D44177C9F639ABD94FAEAB2245B2692162E8C5FA2E37DC40AAEE5D02EE5DB1508DDAC2A6A04749E2EEB2D55A7D4DC6EE552D780B723ABC9FE93DEE303AD8697751B16BF748C03A389DA0737DD1BEAF13F402DEFAA5FDF59742780D06DDE0BCC61B9C94DB4B65DEBF4FEE395171C51A2A6DB994601DCA24968DB0F6227DD682DE95778BB3DD4FCDA1E6FEE84BFF316033D572F5D7A0DEBF00504B0708E01D7BCCBE030000720D0000504B03040A00000000001C57F24C000000000000000000000000150010007374616E646172645F746F6B656E2F74657374732F55580C00C33A505B679E4E5BF5011400504B03040A00000000001C57F24C000000000000000000000000200010007374616E646172645F746F6B656E2F74657374732F5F5F696E69745F5F2E707955580C005028505B679E4E5BF5011400504B03040A00000000001C57F24C0000000000000000000000002B0010007374616E646172645F746F6B656E2F74657374732F746573745F7374616E646172645F746F6B656E2E707955580C005028505B679E4E5BF5011400504B010215030A0000000000C45DF34C0000000000000000000000000F000C000000000000000040ED41000000007374616E646172645F746F6B656E2F55580800C33A505B70FB4F5B504B010215031400080008001C57F24C69020BAC240000002A0000001A000C000000000000000040A4813D0000007374616E646172645F746F6B656E2F5F5F696E69745F5F2E7079555808005028505B679E4E5B504B010215030A00000000003D5AF24C0000000000000000000000001B000C000000000000000040ED41B90000007374616E646172645F746F6B656E2F5F5F707963616368655F5F2F55580800C33A505B55A34E5B504B010215031400080008003D5AF24C8ABD28B9AC000000CA00000032000C000000000000000040A481020100007374616E646172645F746F6B656E2F5F5F707963616368655F5F2F5F5F696E69745F5F2E63707974686F6E2D33362E707963555808005028505B55A34E5B504B010215031400080008003D5AF24CE9FCF1AD450700001111000038000C000000000000000040A4811E0200007374616E646172645F746F6B656E2F5F5F707963616368655F5F2F7374616E646172645F746F6B656E2E63707974686F6E2D33362E707963555808005028505B55A34E5B504B010215031400080008001C57F24C488DC46F44000000600000001B000C000000000000000040A481D90900007374616E646172645F746F6B656E2F7061636B6167652E6A736F6E555808005D39505B679E4E5B504B01021503140008000800FC81F34CE01D7BCCBE030000720D000020000C000000000000000040A481760A00007374616E646172645F746F6B656E2F7374616E646172645F746F6B656E2E707955580800AB3A505BAB3A505B504B010215030A00000000001C57F24C00000000000000000000000015000C000000000000000040ED41920E00007374616E646172645F746F6B656E2F74657374732F55580800C33A505B679E4E5B504B010215030A00000000001C57F24C00000000000000000000000020000C000000000000000040A481D50E00007374616E646172645F746F6B656E2F74657374732F5F5F696E69745F5F2E7079555808005028505B679E4E5B504B010215030A00000000001C57F24C0000000000000000000000002B000C000000000000000040A481230F00007374616E646172645F746F6B656E2F74657374732F746573745F7374616E646172645F746F6B656E2E7079555808005028505B679E4E5B504B0506000000000A000A008D0300007C0F00000000";

            var @params = new RpcObject.Builder()
                    .Put("initialSupply", new RpcValue(BigInteger.Parse("10000")))
                    .Put("decimals", new RpcValue(BigInteger.Parse("18")))
                    .Put("name", new RpcValue("ICON"))
                    .Put("symbol", new RpcValue("ICX"))
                    .Build();

            var from = new Address("hxbe258ceb872e08851f1f59694dac2558708ece11");
            var to = new Address("cx0000000000000000000000000000000000000000");

            var transaction = TransactionBuilder.CreateBuilder()
                    .Nid(NetworkId.Main)
                    .From(from)
                    .To(to)
                    .StepLimit(BigInteger.Parse("0e01348", NumberStyles.HexNumber))
                    .Timestamp(BigInteger.Parse("05727e42882650", NumberStyles.HexNumber))
                    .Nonce(BigInteger.Parse("1"))
                    .Deploy("application/zip", Hex.Decode(content))
                    .Params(@params)
                    .Build();

            var serialized = "icx_sendTransaction.data.{content.0x504b03040a0000000000c45df34c0000000000000000000000000f0010007374616e646172645f746f6b656e2f55580c00c33a505b70fb4f5bf5011400504b03041400080008001c57f24c0000000000000000000000001a0010007374616e646172645f746f6b656e2f5f5f696e69745f5f2e707955580c005028505b679e4e5bf50114004b2bcacf55d02b2e49cc4b492c4a892fc9cf4ecd53c8cc2dc82f2a5108868a868004b900504b070869020bac240000002a000000504b03040a00000000003d5af24c0000000000000000000000001b0010007374616e646172645f746f6b656e2f5f5f707963616368655f5f2f55580c00c33a505b55a34e5bf5011400504b03041400080008003d5af24c000000000000000000000000320010007374616e646172645f746f6b656e2f5f5f707963616368655f5f2f5f5f696e69745f5f2e63707974686f6e2d33362e70796355580c005028505b55a34e5bf501140033e6e5e54a9fe717adc5c0c0f09801093001b10310170b0089148614c61c865cc62846468614a660064de6978c40614dc65bbcc12589792989452921f9d9a9797e9a4cb7f88aa122f12520a12290412b198a58801498f8e5aca7af579c9c5f94aa6f6068609464686c6264916891989a6664669a9694989a9a646c6e6c6992629c986a616e649e669e6868a96f186fa01f1f9f999759121faf5750798bc326373fa53427d50ee48e62908301504b07088abd28b9ac000000ca000000504b03041400080008003d5af24c000000000000000000000000380010007374616e646172645f746f6b656e2f5f5f707963616368655f5f2f7374616e646172645f746f6b656e2e63707974686f6e2d33362e70796355580c005028505b55a34e5bf5011400a557df73dbc611c6e10010a4484aa269fd70ec4675d236cca49225b991d371da4a769d7147a533a59a99b29d4141dc51250d92ea0172220dfd24cde4a5f9071aa5effd97f0da27ff0b79eaee02a000d292da893d58018be3edb7dfed7db7d8ae544a87ff6afe599bd3b4ff68997f265cbf812bd8072334c17cad8d7f759f1d6882b7f5cf346108f35c13a6d4fafc8cb70df058a2009e8234e1d982675b14e1b9280bd23833dab6602dad517a83f3371b2c621fb62b8393963b38f2e5c1e8a51c7a5904568ae01f60a4d666426beb92f70d690a76c1842ef8b971c6da56c68788d057209f4d3ec484be62c687b8d05722df1c784b17ba981365f296d12b16d12f2ba27ac1c5bc583837e14d555420835a5421b8add01d0a570948c552323c56438f25e059723dc1040ca2107ec89aff06131981f4bb0a1926f3fdf3f58df5c01b29b9f160f3c1566773fbe1d623f7912bbb5b1fffa2db71a5ec6cef6c7ff2506cbbf2d1ced64e77c7ddfc6463d379b01124009c10e1ac1f9d44c6d01dc82206d511867e5acb215dc7d7378224540da6ac14616c0a60222b381974467e2513a29e0f110ff841416c21bddec0f583c54c98a57c9874c80f0a34178e42d76f1d1f1df927b733b156f3b132a31a7a6439a32f8752298ce1e94974fddae87a1cbdfc1608c58eebbb434fbee8ae66002ce7014cc6341b3ce2b0dc00e295eb1fcbc874841bba9e91c030ae8561c430aa68e6d12cbc8dfc50b9c3a02bd5bdabc94f87c0cc8df9c8761cac2bc7894a8e331889631fefcb8ef3f763d74fde70b7e34555b713c04fbd7020c3bf8d44c4e149d918194b36e2bd61a84af838878f855d21940c0285c516999d9350066a318f78025b71303855b08237cc600b3c7fd5e0a251399599e8dc7d2dab321214455aa0157ca2330561009566547ba2465f8a96ebcbe7c350aaaeeb495814d3e9aad1e092d5ff6f45ee5ebb2215dadecf5cdfefb8decbf732cbb2368b653d37b8d9b0550d27ba85a64e05d74b872aac38857ca93b68ee5dc5ed3b60eea7f86da633f2cca8b546f4b34b1e056b037f6d54672e8c6f98b4e07f01d4d6bc3032ba5c423dbed0bfd6ba4c94c4dc79b93d07a30ae02d5f70f2820a83b70c9ef90b469e05b1089e8aac8a9ab80533cfe31b5117b769ce8529ff925826ffe2c45f20ff8a58257f6dca7f47bc43fe5b133fe01677e18cb8277e446feaf0e6fd249b77a14ad6c48fe984b89dcbf1be78efdc682f7d0d55d05e8665ff495449f7106da8b69decec202a93c83801a9cc1b4e9a15157a4321bf9202eae4eeeccef5cc4c115f5d6366a6c66ed8fa07c9befe7976ebe710afa7439aa084bae8ccaa204e87f083876042ed4cebb3313b634c0bf531740a63161a827dc3c79a638616780a198f4dc0756c0e4aa46f4e7872241bc5c8045ea442a5e90d7b21e889f985ab9eee4515e7e0c5c1eebed3fae3e79fefff896a197c592a23eb69cf0b6168d1d9dbdddf6d3ef96d0b6e53da53657e3fe6bbe8389eef0681e364aaff6760363121aa71566635364d4a0a0b84a08277bdf4b888e58c38e21941e02947cf261c9dc10a8e996063fd9656d75ef350efc3de19f36fb5eff4d038d3e1add637fbd6982397634ecc697dbb5f7c1c2f376fbec160a7aba321a081d6c0f77fb99665e2d34649fd14092a5d8e88acfdd1e121306b0ad9393e8cf8c1ee67ea431cc40319521d447c101cc2b92fa114552a581b6828b735624eade71503f9fa18132c1167251d5883eb7425cfdb25109882589a3ecd89a56a8ea5a4b861e9289b224c727c04a7a04c0ef959348fc0fc7a52d2ec747906453cc1019480ada42b4643ffe49ac6825191aa77d3286fef2ee864dbcbeca45a3eec0dbd581c24e2bf6f1d5c17044fce679920f57c901bbbb138cc9bda0db9e03afeee6a55b8be1bc31f43add2faa56b1717d9a10caf08882d40331370351f30d792c58dd58c12e9d3f1c76c3589cfa8b4afe9cba8e9f84326fe54cd4c3ab2069f56e5ecc96fa6105060c65c682f6d5516ec35273840485f1fc396874d6d2485ad37695d9aa321b61545276db5146e27b5a3fd8f6d1c354a5f68a96ae9d8114d2f5a3af5e41cc1a1a514f13f33a48df94b681410af2029bff4af24f78fe3bf3afa96937bf0e97d5dfd95649f44ecb53936fb2664ccc7c619a779ac387f780606c242df06d163dfc227e65827cbc91a608def8a6111859078329ba7d517c7e1daa8bb96acc569313d9b3e38d53f5afb9e3540f048c1e0ebec95546134d70b1c6f34a416345af260a787d299b4430e7d83516b13b742d869a92d34db68f03c6b58571ea6ed790f9bb1009ab164a6a915f914cc5f90d91a6d902aa8629dd59905274a952dcca892936db257a6ba38f5119a062667436ec3d01f1d4eb773083f5a7c0e09b710ce53f84ee8b881a4f38c049a74117eff151030747db5ac25a245a272d989d35e883b6fcc010ed8f894f4a4efa72765221deb936c511c3fc06cd1d8bac10ca8c1255e37978a356e43479ebf6a5c674b26fdaa5154f82d06ab05d001efab1eb4aa0f492e76f79e500b1f5527fd6e6bb268516592eb1ee63991e79936e771fc91f22b968a83ad03425ebbb360fd17504b0708e9fcf1ad4507000011110000504b03041400080008001c57f24c0000000000000000000000001b0010007374616e646172645f746f6b656e2f7061636b6167652e6a736f6e55580c005d39505b679e4e5bf5011400abe6520002a5b2d4a2e2ccfc3c252b0525033d033d43251d88786e62665e7c5a664e2a48a6b824312f25b12825be243f3b350f454971727e11584d30544d085809572d00504b0708488dc46f4400000060000000504b0304140008000800fc81f34c000000000000000000000000200010007374616e646172645f746f6b656e2f7374616e646172645f746f6b656e2e707955580c00ab3a505bab3a505bf5011400ad56516fa338107ee75758790974b3d19dee2dbaac4ada5e55296d5722b7d26a55210326870a36b24db751d5ffbe636313709236d91e2f608fe79bf17c1f1e1755cdb84438493d2fe7ac4245caa820fca948092a5ae399e7adc26b3447e36a13e1aa2ec98a3d123af63c2f2db110480f2389698679e603d6345c5c04330fc173ae86381192e3545644fec7323d9f911c515c115f90320fd0e72f0896b42eeaa901d77b17406caa84951f82c8485a54b8145b9082ca53412493b88c9aba2e371fc2497089694aee738d324131fb49099fa130cb3811e2b7d3e3988a9c708b2a590709a3275c3664a670619061896728d94822e6778c92c08d6528bfe0ec6716e192dc5049788e53e2775f51cab8753c2fec6caf58a0967f705926387db42929ed1d93d4a17cacfab414fd1b10b1ce628105990cf509081a225e84cbf0eee22a52c2368517e3d6b4ba5f85cb38faf7ebd7e57765d6fcc642136c965c5e5ddcdc864bed6d35644c77e1ed959a56f23653d1f7dbc5fd524db6921d1bd2c813a1b2646bbfa0197926d9fcafa0abd3ca61cd2dd1712cee14ccc2c771410b19c7063e4b66a82bdb25f827503a2d3825832d081401520aa69d7b96045b23404de37eb560cbdf30bf5cf8ada95f5815738274deb1dcd4640eb9bb50b6b20e8c2dfe31108a05c75d11b4e30a7f8eebda72e538b7541ee36e45050097452a3b042bbcfdd97704310a251650cad250a40a5ed873c6506d0b64866ab333750c42ccd1e09f184dcc69d9996fa3d5e80d827be14d52ea71b81da484ced09f7fa0b3b32ea9ce6bc9d66bc2a719499ab59f8fb7d0b301e0fca53f7a1dc39f1b5ef782efca6b2a88f4fb138704a417dac13e89e805ea63bf08b4b9fd3c44f30f3dacc41ad6c2dfcc1fa03efdd406cc3635fca3bde6779003b330b007c6331ca714973e273863146ab6e20dd91e1a6fb4544e64c3697fcb6bd8d3b1c06fb6da01b429d829e0ef34e1017cc7e82901de6fd0831803899d12e7e4063e08db29a9757b7082fecffdbcc85b332a84233ff5b4a6394ac6ca3676ffc17e063dcdeb646c0e267cef488bddcc3fdcd360136eed14e603fadbba0f76a5d772e8ba5cfaa3fb4622d67136da39685cc8f981f9cf26d44177c9f639abd94faeab2245b2692162e8c5fa2e37dc40aaee5d02ee5db1508ddac2a6a04749e2eeb2d55a7d4dc6ee552d780b723abc9fe93dee303ad8697751b16bf748c03a389da0737dd1beaf13f402defaa5fdf59742780d06dde0bcc61b9c94db4b65debf4fee395171c51a2a6db994601dca24968db0f6227dd682de95778bb3dd4fcda1e6fee84bff316033d572f5d7a0debf00504b0708e01d7bccbe030000720d0000504b03040a00000000001c57f24c000000000000000000000000150010007374616e646172645f746f6b656e2f74657374732f55580c00c33a505b679e4e5bf5011400504b03040a00000000001c57f24c000000000000000000000000200010007374616e646172645f746f6b656e2f74657374732f5f5f696e69745f5f2e707955580c005028505b679e4e5bf5011400504b03040a00000000001c57f24c0000000000000000000000002b0010007374616e646172645f746f6b656e2f74657374732f746573745f7374616e646172645f746f6b656e2e707955580c005028505b679e4e5bf5011400504b010215030a0000000000c45df34c0000000000000000000000000f000c000000000000000040ed41000000007374616e646172645f746f6b656e2f55580800c33a505b70fb4f5b504b010215031400080008001c57f24c69020bac240000002a0000001a000c000000000000000040a4813d0000007374616e646172645f746f6b656e2f5f5f696e69745f5f2e7079555808005028505b679e4e5b504b010215030a00000000003d5af24c0000000000000000000000001b000c000000000000000040ed41b90000007374616e646172645f746f6b656e2f5f5f707963616368655f5f2f55580800c33a505b55a34e5b504b010215031400080008003d5af24c8abd28b9ac000000ca00000032000c000000000000000040a481020100007374616e646172645f746f6b656e2f5f5f707963616368655f5f2f5f5f696e69745f5f2e63707974686f6e2d33362e707963555808005028505b55a34e5b504b010215031400080008003d5af24ce9fcf1ad450700001111000038000c000000000000000040a4811e0200007374616e646172645f746f6b656e2f5f5f707963616368655f5f2f7374616e646172645f746f6b656e2e63707974686f6e2d33362e707963555808005028505b55a34e5b504b010215031400080008001c57f24c488dc46f44000000600000001b000c000000000000000040a481d90900007374616e646172645f746f6b656e2f7061636b6167652e6a736f6e555808005d39505b679e4e5b504b01021503140008000800fc81f34ce01d7bccbe030000720d000020000c000000000000000040a481760a00007374616e646172645f746f6b656e2f7374616e646172645f746f6b656e2e707955580800ab3a505bab3a505b504b010215030a00000000001c57f24c00000000000000000000000015000c000000000000000040ed41920e00007374616e646172645f746f6b656e2f74657374732f55580800c33a505b679e4e5b504b010215030a00000000001c57f24c00000000000000000000000020000c000000000000000040a481d50e00007374616e646172645f746f6b656e2f74657374732f5f5f696e69745f5f2e7079555808005028505b679e4e5b504b010215030a00000000001c57f24c0000000000000000000000002b000c000000000000000040a481230f00007374616e646172645f746f6b656e2f74657374732f746573745f7374616e646172645f746f6b656e2e7079555808005028505b679e4e5b504b0506000000000a000a008d0300007c0f00000000.contentType.application/zip.params.{decimals.0x12.initialSupply.0x2710.name.ICON.symbol.ICX}}.dataType.deploy.from.hxbe258ceb872e08851f1f59694dac2558708ece11.nid.0x1.nonce.0x1.stepLimit.0xe01348.timestamp.0x5727e42882650.to.cx0000000000000000000000000000000000000000.version.0x3";
            var deserialized = TransactionDeserializer.Deserialize(serialized);

            TransactionAssertion.CompareTransactions(transaction, deserialized);
        }

        [Fact]
        public void TestMessageTransactionDeserialize()
        {
            var from = new Address("hxbe258ceb872e08851f1f59694dac2558708ece11");
            var to = new Address("hx5bfdb090f43a808005ffc27c25b213145e80b7cd");

            var transaction = TransactionBuilder.CreateBuilder()
                .Nid(NetworkId.Main)
                .From(from)
                .To(to)
                .StepLimit(BigInteger.Parse("0e01348", NumberStyles.HexNumber))
                .Timestamp(BigInteger.Parse("05727e42882650", NumberStyles.HexNumber))
                .Nonce(BigInteger.Parse("1"))
                .Message("Hello World")
                .Build();

            var serialized =
                "icx_sendTransaction.data.0x48656c6c6f20576f726c64.dataType.message.from.hxbe258ceb872e08851f1f59694dac2558708ece11.nid.0x1.nonce.0x1.stepLimit.0xe01348.timestamp.0x5727e42882650.to.hx5bfdb090f43a808005ffc27c25b213145e80b7cd.version.0x3";

            var deserialized = TransactionDeserializer.Deserialize(serialized);

            TransactionAssertion.CompareTransactions(transaction, deserialized);
        }
    }
}
