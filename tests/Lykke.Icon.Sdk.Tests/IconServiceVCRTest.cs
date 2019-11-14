using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.Http;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Org.BouncyCastle.Utilities.Encoders;
using Xunit;
using Xunit.Abstractions;

namespace Lykke.Icon.Sdk.Tests
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "CommentTypo")]
    public class IconServiceVCRTest //: TestWithEnvVariables
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private const string Url = "https://test-ctz.solidwallet.io/api/v3";
        private const string TestV2Url = "https://test-ctz.solidwallet.io/api/v2";
        private const string PrivateKeyString = "2d42994b2f7735bbc93a3e64381864d06747e574aa94655c516f9ad0a74eed79";


        private readonly Address _scoreAddress;
        private readonly IconService _iconService;
        private readonly IWallet _wallet;
        private readonly KeyWallet _secretWallet;

        public IconServiceVCRTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            //var pk1 = Environment.GetEnvironmentVariable("PK1");
            _scoreAddress = new Address("cxcc7ef86cdae93a89b6c08206a7962bcb9abb7bf4");
            var httpClient = new HttpClient();
            _iconService = new IconService(new HttpProvider(httpClient, Url));
            _wallet = KeyWallet.Load(new Bytes(PrivateKeyString));
            _secretWallet = null; //KeyWallet.Load(new Bytes(pk1));
        }

        [Fact]
        public async Task TestGetBalance()
        {
            var balance = await _iconService.GetBalance(_wallet.GetAddress());
            
            // ReSharper disable once UnusedVariable
            var amount = IconAmount.Of(balance, IconAmount.Unit.ICX);
            
            Assert.True(balance >= 0);
        }

        [Fact()]
        public async Task TestGetTotalSupply()
        {
            var totalSupply = await _iconService.GetTotalSupply();
            Assert.True(totalSupply > 0);
        }

        [Fact]
        public async Task TestGetBlockByHeight()
        {
            var block = await _iconService.GetBlock(3_599_591);
            var blockHash = block.GetBlockHash().ToHexString(false);
            Assert.Equal("641d7110b74142664a4b9784f53b68914e077f60c1eb42631c3e6ddde5461925", blockHash);
        }

        [Fact(Skip = "Check 503 response from icon node")]
        public async Task TestGetBlockByHeight__Wrong_Serialization()
        {
            var block = await _iconService.GetBlock(49_755);
            var result = Parallel.For(0, 1000, (i) =>
            {
                var block1 = _iconService.GetBlock(49_755).Result;
            });

            while (!result.IsCompleted)
            {
                await Task.Delay(1000);
            }
            var blockHash = block.GetBlockHash().ToHexString(false);
            Assert.Equal("d5629fe006104df557570ce2613c8df1901d8f6f322b9f251645c201fa1d1e9e", blockHash);
        }

        [Fact]
        public async Task TestGetBlockByHash()
        {
            var hash = new Bytes("0x641d7110b74142664a4b9784f53b68914e077f60c1eb42631c3e6ddde5461925");
            var block = await _iconService.GetBlock(hash);
            Assert.Equal(hash, block.GetBlockHash());
        }

        [Fact]
        public async Task TestGetLastBlock()
        {
            var block = await _iconService.GetLastBlock();
            Assert.True(block.GetHeight() > 100);
        }

        [Fact(Skip = "will be implemented soon")]
        public async Task TestGetScoreApi()
        {
            var apis = await _iconService.GetScoreApi(_scoreAddress);
            Assert.Equal("balanceOf", apis[0].GetScoreName());
        }

        [Fact]
        public async Task TestGetTransaction()
        {
            var txHash = new Bytes("0x0a0972b775a433b591121b4df4ffd8c70fa4c197e28e9e8fb21601f876c9c075");
            var tx = await _iconService.GetTransaction(txHash);
            Assert.Equal(txHash, tx.GetTxHash());
        }

        [Fact]
        public async Task TestGetTransactionResult()
        {
            var txHash = new Bytes("0x0a0972b775a433b591121b4df4ffd8c70fa4c197e28e9e8fb21601f876c9c075");
            var tx = await _iconService.GetTransactionResult(txHash);
            Assert.Equal(txHash, tx.GetTxHash());
        }

        [Fact(Skip = "Find transaction with internal transfers")]
        public async Task TestGetTransactionEventLog()
        {
            string bigIntVal = "500000000000000000";
            var expectedInternalTransaction = BigInteger.Parse(bigIntVal, NumberStyles.Integer);
            var txHash = new Bytes("0x88409e57c731b7a69f9b82b9818f26aeda0b1180dadbb8c742297c5989e73e02");
            var tx = await _iconService.GetTransactionResult(txHash);
            var events = tx.GetEventLogs();
            Assert.True(events?.Any(@event =>
            {
                var eventItems = @event.GetIndexed();
                var eventName = eventItems.FirstOrDefault();

                if (eventName.ToString() != "ICXTransfer(Address,Address,int)")
                    return false;

                var from = eventItems[1].ToAddress();
                var to = eventItems[2].ToAddress();
                var value = eventItems[3].ToInteger();

                Assert.True(from == new Address("cx8ce9a5cbbe3d54f49f32ff58b6018a6153a584ae"));
                Assert.True(to == new Address("hx30c3b03b67f95762ae13cab73cd6f26f8860667e"));
                Assert.True(value == expectedInternalTransaction);

                return true;
            }));
            Assert.Equal(txHash, tx.GetTxHash());
        }

        [Fact(Skip = "Remove skip parameter to check actual transfer")]
        public async Task TestSendIcxTransaction()
        {
            //1544619275191000
            var timestmap = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(5)).ToUnixTimeSeconds() * 1_000_000L;
            var balance = BigInteger.Parse("30000000000000000000");
            
            
            // ReSharper disable once UnusedVariable
            var amount = IconAmount.Of(balance, IconAmount.Unit.ICX);
            var transaction = TransactionBuilder.CreateBuilder()
                    .Nid(BigInteger.Parse("2"))
                    .From(_secretWallet.GetAddress())
                    .To(_wallet.GetAddress())
                    .Value(balance)
                    .StepLimit(BigInteger.Parse("1000000"))
                    .Timestamp(BigInteger.Parse(timestmap.ToString()))
                    .Nonce(BigInteger.Parse("9"))
                    .Build();

            var signedTransaction = new SignedTransaction(transaction, _wallet);
            
            // ReSharper disable once UnusedVariable
            var hash = await _iconService.SendTransaction(signedTransaction);
            //Assert.Equal("0xac705c771806cd0a04df9025993febdce1c1d8006d8043b01ed9adc86a395d08", hash.ToString());
        }

        [Fact(Skip = "will be implemented soon")]
        public async Task TestCall()
        {
            var @params = new RpcObject.Builder()
                    .Put("_owner", new RpcValue(_wallet.GetAddress()))
                    .Build();

            var call = new Call.Builder()
                    .From(_wallet.GetAddress())
                    .To(_scoreAddress)
                    .Method("balanceOf")
                    .Params(@params)
                    .Build();

            var result = await _iconService.CallAsync(call);
            Assert.Equal(BigInteger.Parse("99999999991999999999997740000"), result.ToInteger());
        }

        [Fact(Skip = "will be implemented soon")]
        public async Task TestCallWithClassParam()
        {
            var @params = new TokenBalance();
            @params.Owner = _wallet.GetAddress().ToString();

            //            _iconService.addConverterFactory(new RpcConverter.RpcConverterFactory() {
            //            @Override
            //            public <T> RpcConverter<T> create(Class<T> type) {
            //                if (BalanceResponse.class == type) {
            //                    return new RpcConverter<T>() {
            //                        @Override
            //                            public T convertTo(RpcItem object)
            //        {
            //            BalanceResponse response = new BalanceResponse();
            //            response.balance = object.asInteger();
            //            return (T)response;
            //        }

            //        @Override
            //                            public RpcItem convertFrom(T object)
            //        {
            //            return RpcItemCreator.create(object);
            //        }
            //    };
            //}
            //                return null;
            //            }
            //        });

            var call = new Call.Builder()
                    .From(_wallet.GetAddress())
                    .To(_scoreAddress)
                    .Method("balanceOf")
                    .Params(@params)
                    .BuildWith<BalanceResponse>();

            var result = await _iconService.CallAsync(call);
            Assert.Equal(BigInteger.Parse("99999999991999999999997740000"), result.Balance);
        }

        [Fact(Skip = "will be implemented soon")]
        public async Task TestSendToken()
        {
            // 893
            // ReSharper disable once UnusedVariable
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000L;
            var @params = new RpcObject.Builder()
                    .Put("_to", new RpcValue(new Address("hx4873b94352c8c1f3b2f09aaeccea31ce9e90bd31")))
                    .Put("_value", new RpcValue(BigInteger.Parse("1")))
                    .Build();

            var transaction = TransactionBuilder.CreateBuilder()
                    .Nid(BigInteger.Parse("3"))
                    .From(_wallet.GetAddress())
                    .To(_scoreAddress)
                    .StepLimit(BigInteger.Parse("75000"))
                    .Timestamp(BigInteger.Parse("574b2964095a8", NumberStyles.HexNumber))
                    //                .timestamp(BigInteger.Parse(Long.toString(timestmap)))
                    .Nonce(BigInteger.Parse("1"))
                    .Call("transfer")
                    .Params(@params)
                    .Build();

            var signedTransaction = new SignedTransaction(transaction, _wallet);
            var hash = await _iconService.SendTransaction(signedTransaction);
            Assert.Equal("0x38cb29ff07ceeed59bf07274e326325c04ed1e6e8dca877dcff193c2de535263", hash.ToString());
        }

        [Fact(Skip = "will be implemented soon")]
        public async Task TestDeployScore()
        {
            // 3872
            // ReSharper disable once UnusedVariable
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000L;
            const string content = "504B03040A0000000000C45DF34C0000000000000000000000000F0010007374616E646172645F746F6B656E2F55580C00C33A505B70FB4F5BF5011400504B03041400080008001C57F24C0000000000000000000000001A0010007374616E646172645F746F6B656E2F5F5F696E69745F5F2E707955580C005028505B679E4E5BF50114004B2BCACF55D02B2E49CC4B492C4A892FC9CF4ECD53C8CC2DC82F2A5108868A868004B900504B070869020BAC240000002A000000504B03040A00000000003D5AF24C0000000000000000000000001B0010007374616E646172645F746F6B656E2F5F5F707963616368655F5F2F55580C00C33A505B55A34E5BF5011400504B03041400080008003D5AF24C000000000000000000000000320010007374616E646172645F746F6B656E2F5F5F707963616368655F5F2F5F5F696E69745F5F2E63707974686F6E2D33362E70796355580C005028505B55A34E5BF501140033E6E5E54A9FE717ADC5C0C0F09801093001B10310170B0089148614C61C865CC62846468614A660064DE6978C40614DC65BBCC12589792989452921F9D9A9797E9A4CB7F88AA122F12520A12290412B198A58801498F8E5ACA7AF579C9C5F94AA6F6068609464686C6264916891989A6664669A9694989A9A646C6E6C6992629C986A616E649E669E6868A96F186FA01F1F9F999759121FAF5750798BC326373FA53427D50EE48E62908301504B07088ABD28B9AC000000CA000000504B03041400080008003D5AF24C000000000000000000000000380010007374616E646172645F746F6B656E2F5F5F707963616368655F5F2F7374616E646172645F746F6B656E2E63707974686F6E2D33362E70796355580C005028505B55A34E5BF5011400A557DF73DBC611C6E10010A4484AA269FD70EC4675D236CCA49225B991D371DA4A769D7147A533A59A99B29D4141DC51250D92EA0172220DFD24CDE4A5F9071AA5EFFD97F0DA27FF0B79EAEE02A000D292DA893D58018BE3EDB7DFED7DB7D8AE544A87FF6AFE599BD3B4FF68997F265CBF812BD8072334C17CAD8D7F759F1D6882B7F5CF346108F35C13A6D4FAFC8CB70DF058A2009E8234E1D982675B14E1B9280BD23833DAB6602DAD517A83F3371B2C621FB62B8393963B38F2E5C1E8A51C7A5904568AE01F60A4D666426BEB92F70D690A76C1842EF8B971C6DA56C68788D057209F4D3EC484BE62C687B8D05722DF1C784B17BA981365F296D12B16D12F2BA27AC1C5BC583837E14D555420835A5421B8ADD01D0A570948C552323C56438F25E059723DC1040CA2107EC89AFF06131981F4BB0A1926F3FDF3F58DF5C01B29B9F160F3C1566773FBE1D623F7912BBB5B1FFFA2DB71A5EC6CEF6C7FF2506CBBF2D1CED64E77C7DDFC6463D379B01124009C10E1AC1F9D44C6D01DC82206D511867E5ACB215DC7D7378224540DA6AC14616C0A60222B381974467E2513A29E0F110FF841416C21BDDEC0F583C54C98A57C9874C80F0A34178E42D76F1D1F1DF927B733B156F3B132A31A7A6439A32F8752298CE1E94974FDDAE87A1CBDFC1608C58EEBBB434FBEE8AE66002CE7014CC6341B3CE2B0DC00E295EB1FCBC874841BBA9E91C030AE8561C430AA68E6D12CBC8DFC50B9C3A02BD5BDABC94F87C0CC8DF9C8761CAC2BC7894A8E331889631FEFCB8EF3F763D74FDE70B7E34555B713C04FBD7020C3BF8D44C4E149D918194B36E2BD61A84AF838878F855D21940C0285C516999D9350066A318F78025B71303855B08237CC600B3C7FD5E0A251399599E8DC7D2DAB321214455AA0157CA2330561009566547BA2465F8A96EBCBE7C350AAAEEB495814D3E9AAD1E092D5FF6F45EE5EBB2215DADECF5CDFEFB8DECBF732CBB2368B653D37B8D9B0550D27BA85A64E05D74B872AAC38857CA93B68EE5DC5ED3B60EEA7F86DA633F2CCA8B546F4B34B1E056B037F6D54672E8C6F98B4E07F01D4D6BC3032BA5C423DBED0BFD6BA4C94C4DC79B93D07A30AE02D5F70F2820A83B70C9EF90B469E05B1089E8AAC8A9AB80533CFE31B5117B769CE8529FF925826FFE2C45F20FF8A58257F6DCA7F47BC43FE5B133FE01677E18CB8277E446FEAF0E6FD249B77A14AD6C48FE984B89DCBF1BE78EFDC682F7D0D55D05E8665FF495449F7106DA8B69DECEC202A93C83801A9CC1B4E9A15157A4321BF9202EAE4EEECCEF5CC4C115F5D6366A6C66ED8FA07C9BEFE7976EBE710AFA7439AA084BAE8CCAA204E87F083876042ED4CEBB3313B634C0BF531740A63161A827DC3C79A638616780A198F4DC0756C0E4AA46F4E7872241BC5C8045EA442A5E90D7B21E889F985AB9EEE4515E7E0C5C1EEBED3FAE3E79FEFFF896A197C592A23EB69CF0B6168D1D9DBDDDF6D3EF96D0B6E53DA53657E3FE6BBE8389EEF0681E364AAFF6760363121AA71566635364D4A0A0B84A08277BDF4B888E58C38E21941E02947CF261C9DC10A8E996063FD9656D75EF350EFC3DE19F36FB5EFF4D038D3E1ADD637FBD6982397634ECC697DBB5F7C1C2F376FBEC160A7ABA321A081D6C0F77FB99665E2D34649FD14092A5D8E88ACFDD1E121306B0AD9393E8CF8C1EE67EA431CC40319521D447C101CC2B92FA114552A581B6828B735624EADE71503F9FA18132C1167251D5883EB7425CFDB25109882589A3ECD89A56A8EA5A4B861E9289B224C727C04A7A04C0EF959348FC0FC7A52D2EC747906453CC1019480ADA42B4643FFE49AC6825191AA77D3286FEF2EE864DBCBECA45A3EEC0DBD581C24E2BF6F1D5C17044FCE679920F57C901BBBB138CC9BDA0DB9E03AFEEE6A55B8BE1BC31F43ADD2FAA56B1717D9A10CAF08882D40331370351F30D792C58DD58C12E9D3F1C76C3589CFA8B4AFE9CBA8E9F84326FE54CD4C3AB2069F56E5ECC96FA6105060C65C682F6D5516EC35273840485F1FC396874D6D2485AD37695D9AA321B61545276DB5146E27B5A3FD8F6D1C354A5F68A96AE9D8114D2F5A3AF5E41CC1A1A514F13F33A48DF94B681410AF2029BFF4AF24F78FE3BF3AFA96937BF0E97D5DFD95649F44ECB53936FB2664CCC7C619A779AC387F780606C242DF06D163DFC227E65827CBC91A608DEF8A6111859078329BA7D517C7E1DAA8BB96ACC569313D9B3E38D53F5AFB9E3540F048C1E0EBEC95546134D70B1C6F34A416345AF260A787D299B4430E7D83516B13B742D869A92D34DB68F03C6B58571EA6ED790F9BB1009AB164A6A915F914CC5F90D91A6D902AA8629DD59905274A952DCCA892936DB257A6BA38F5119A062667436EC3D01F1D4EB773083F5A7C0E09B710CE53F84EE8B881A4F38C049A74117EFF151030747DB5AC25A245A272D989D35E883B6FCC010ED8F894F4A4EFA72765221DEB936C511C3FC06CD1D8BAC10CA8C1255E37978A356E43479EBF6A5C674B26FDAA5154F82D06AB05D001EFAB1EB4AA0F492E76F79E500B1F5527FD6E6BB268516592EB1EE63991E79936E771FC91F22B968A83AD03425EBBB360FD17504B0708E9FCF1AD4507000011110000504B03041400080008001C57F24C0000000000000000000000001B0010007374616E646172645F746F6B656E2F7061636B6167652E6A736F6E55580C005D39505B679E4E5BF5011400ABE6520002A5B2D4A2E2CCFC3C252B0525033D033D43251D88786E62665E7C5A664E2A48A6B824312F25B12825BE243F3B350F454971727E11584D30544D085809572D00504B0708488DC46F4400000060000000504B0304140008000800FC81F34C000000000000000000000000200010007374616E646172645F746F6B656E2F7374616E646172645F746F6B656E2E707955580C00AB3A505BAB3A505BF5011400AD56516FA338107EE75758790974B3D19DEE2DBAAC4ADA5E55296D5722B7D26A55210326870A36B24DB751D5FFBE636313709236D91E2F608FE79BF17C1F1E1755CDB84438493D2FE7AC4245CAA820FCA948092A5AE399E7ADC26B3447E36A13E1AA2EC98A3D123AF63C2F2DB110480F2389698679E603D6345C5C04330FC173AE86381192E3545644FEC7323D9F911C515C115F90320FD0E72F0896B42EEAA901D77B17406CAA84951F82C8485A54B8145B9082CA53412493B88C9ABA2E371FC2497089694AEE738D324131FB49099FA130CB3811E2B7D3E3988A9C708B2A590709A3275C3664A670619061896728D94822E6778C92C08D6528BFE0EC6716E192DC5049788E53E2775F51CAB8753C2FEC6CAF58A0967F705926387DB42929ED1D93D4A17CACFAB414FD1B10B1CE628105990CF509081A225E84CBF0EEE22A52C2368517E3D6B4BA5F85CB38FAF7EBD7E57765D6FCC642136C965C5E5DDCDC864BED6D35644C77E1ED959A56F23653D1F7DBC5FD524DB6921D1BD2C813A1B2646BBFA0197926D9FCAFA0ABD3CA61CD2DD1712CEE14CCC2C771410B19C7063E4B66A82BDB25F827503A2D3825832D081401520AA69D7B96045B23404DE37EB560CBDF30BF5CF8ADA95F5815738274DEB1DCD4640EB9BB50B6B20E8C2DFE31108A05C75D11B4E30A7F8EEBDA72E538B7541EE36E45050097452A3B042BBCFDD97704310A251650CAD250A40A5ED873C6506D0B64866AB333750C42CCD1E09F184DCC69D9996FA3D5E80D827BE14D52EA71B81DA484CED09F7FA0B3B32EA9CE6BC9D66BC2A719499AB59F8FB7D0B301E0FCA53F7A1DC39F1B5EF782EFCA6B2A88F4FB138704A417DAC13E89E805EA63BF08B4B9FD3C44F30F3DACC41AD6C2DFCC1FA03EFDD406CC3635FCA3BDE6779003B330B007C6331CA714973E273863146AB6E20DD91E1A6FB4544E64C3697FCB6BD8D3B1C06FB6DA01B429D829E0EF34E1017CC7E82901DE6FD0831803899D12E7E4063E08DB29A9757B7082FECFFDBCC85B332A84233FF5B4A6394AC6CA3676FFC17E063DCDEB646C0E267CEF488BDDCC3FDCD360136EED14E603FADBBA0F76A5D772E8BA5CFAA3FB4622D67136DA39685CC8F981F9CF26D44177C9F639ABD94FAEAB2245B2692162E8C5FA2E37DC40AAEE5D02EE5DB1508DDAC2A6A04749E2EEB2D55A7D4DC6EE552D780B723ABC9FE93DEE303AD8697751B16BF748C03A389DA0737DD1BEAF13F402DEFAA5FDF59742780D06DDE0BCC61B9C94DB4B65DEBF4FEE395171C51A2A6DB994601DCA24968DB0F6227DD682DE95778BB3DD4FCDA1E6FEE84BFF316033D572F5D7A0DEBF00504B0708E01D7BCCBE030000720D0000504B03040A00000000001C57F24C000000000000000000000000150010007374616E646172645F746F6B656E2F74657374732F55580C00C33A505B679E4E5BF5011400504B03040A00000000001C57F24C000000000000000000000000200010007374616E646172645F746F6B656E2F74657374732F5F5F696E69745F5F2E707955580C005028505B679E4E5BF5011400504B03040A00000000001C57F24C0000000000000000000000002B0010007374616E646172645F746F6B656E2F74657374732F746573745F7374616E646172645F746F6B656E2E707955580C005028505B679E4E5BF5011400504B010215030A0000000000C45DF34C0000000000000000000000000F000C000000000000000040ED41000000007374616E646172645F746F6B656E2F55580800C33A505B70FB4F5B504B010215031400080008001C57F24C69020BAC240000002A0000001A000C000000000000000040A4813D0000007374616E646172645F746F6B656E2F5F5F696E69745F5F2E7079555808005028505B679E4E5B504B010215030A00000000003D5AF24C0000000000000000000000001B000C000000000000000040ED41B90000007374616E646172645F746F6B656E2F5F5F707963616368655F5F2F55580800C33A505B55A34E5B504B010215031400080008003D5AF24C8ABD28B9AC000000CA00000032000C000000000000000040A481020100007374616E646172645F746F6B656E2F5F5F707963616368655F5F2F5F5F696E69745F5F2E63707974686F6E2D33362E707963555808005028505B55A34E5B504B010215031400080008003D5AF24CE9FCF1AD450700001111000038000C000000000000000040A4811E0200007374616E646172645F746F6B656E2F5F5F707963616368655F5F2F7374616E646172645F746F6B656E2E63707974686F6E2D33362E707963555808005028505B55A34E5B504B010215031400080008001C57F24C488DC46F44000000600000001B000C000000000000000040A481D90900007374616E646172645F746F6B656E2F7061636B6167652E6A736F6E555808005D39505B679E4E5B504B01021503140008000800FC81F34CE01D7BCCBE030000720D000020000C000000000000000040A481760A00007374616E646172645F746F6B656E2F7374616E646172645F746F6B656E2E707955580800AB3A505BAB3A505B504B010215030A00000000001C57F24C00000000000000000000000015000C000000000000000040ED41920E00007374616E646172645F746F6B656E2F74657374732F55580800C33A505B679E4E5B504B010215030A00000000001C57F24C00000000000000000000000020000C000000000000000040A481D50E00007374616E646172645F746F6B656E2F74657374732F5F5F696E69745F5F2E7079555808005028505B679E4E5B504B010215030A00000000001C57F24C0000000000000000000000002B000C000000000000000040A481230F00007374616E646172645F746F6B656E2F74657374732F746573745F7374616E646172645F746F6B656E2E7079555808005028505B679E4E5B504B0506000000000A000A008D0300007C0F00000000";
            var toAddress = new Address("cx0000000000000000000000000000000000000000");

            var @params = new RpcObject.Builder()
                    .Put("initialSupply", new RpcValue(BigInteger.Parse("10000")))
                    .Put("decimals", new RpcValue(BigInteger.Parse("18")))
                    .Put("name", new RpcValue("ICON"))
                    .Put("symbol", new RpcValue("ICX"))
                    .Build();

            var transaction = TransactionBuilder.CreateBuilder()
                    .Nid(BigInteger.Parse("3"))
                    .From(_wallet.GetAddress())
                    .To(toAddress)
                    .StepLimit(BigInteger.Parse("e01348", NumberStyles.HexNumber))
                    .Timestamp(BigInteger.Parse("574b28ed4ca00", NumberStyles.HexNumber))
                    //                .timestamp(BigInteger.Parse(Long.toString(timestmap)))
                    .Nonce(BigInteger.Parse("1"))
                    .Deploy("application/zip", Hex.Decode(content))
                    .Params(@params)
                    .Build();

            var signedTransaction = new SignedTransaction(transaction, _wallet);
            var hash = await _iconService.SendTransaction(signedTransaction);
            Assert.Equal("0x3a47d71f2e54b63120a00ad7c2e7b86fa037aa1962f122c92e9e0593ce4e18f6", hash.ToString());
        }

        [Fact(Skip = "will be implemented soon")]
        public async Task TestSendMessage()
        {
            // 509
            // ReSharper disable once UnusedVariable
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000L;
            var toAddress = new Address("hx4873b94352c8c1f3b2f09aaeccea31ce9e90bd31");

            var transaction = TransactionBuilder.CreateBuilder()
                    .Nid(BigInteger.Parse("3"))
                    .From(_wallet.GetAddress())
                    .To(toAddress)
                    .StepLimit(BigInteger.Parse("75000"))
                    .Timestamp(BigInteger.Parse("574b28aee6810", NumberStyles.HexNumber))
                    //                .timestamp(BigInteger.Parse(Long.toString(timestmap)))
                    .Nonce(BigInteger.Parse("1"))
                    .Message("Hello World")
                    .Build();

            var signedTransaction = new SignedTransaction(transaction, _wallet);
            var hash = await _iconService.SendTransaction(signedTransaction);
            Assert.Equal("0xe436d4afac5a73cef8b1f88eadc66e6a1b39ef38f409ddea52ddfaea5e36b94e", hash.ToString());
        }

        [Fact(Skip = "will be implemented soon")]
        public async Task TestV2()
        {
            var httpClient = new HttpClient();
            var provider = new HttpProvider(httpClient, TestV2Url);

            var requestId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // address invalid : 20584, 13204, 13129
            // timestamp long type : 21265
            // value no prefix : 13331
            int[] heights = { 20584, 13204, 13129, 21265, 13331 };

            foreach (var h in heights)
            {
                var height = BigInteger.Parse(h.ToString());

                var @params = new RpcObject.Builder()
                        .Put("height", new RpcValue(height.ToString()))
                        .Build();
                var request = new Request(requestId, "icx_getBlockByHeight", @params);
                var result = await provider.SendRequestAsync(request, Converters.RpcItem);

                var block = Converters.Block.ConvertTo(result.ToObject().GetItem("block").ToObject());
                var txs = block.GetTransactions();
                foreach (var tx in txs)
                {
                    _testOutputHelper.WriteLine("addres:" + tx.GetTo());
                    _testOutputHelper.WriteLine("timestamp:" + tx.GetTimestamp());
                    _testOutputHelper.WriteLine("value:" + tx.GetValue());
                    _testOutputHelper.WriteLine("nonce:" + tx.GetNonce());
                }

                Thread.Sleep(100);
            }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public class TokenBalance
        {
            public string Owner { get; set; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public class BalanceResponse
        {
            public BigInteger Balance { get; set; }
        }
    }
}
