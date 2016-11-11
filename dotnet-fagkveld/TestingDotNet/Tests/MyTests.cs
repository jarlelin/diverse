using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Moq;
using NUnit.Framework;
using TestingDotNet;
using TestingDotNet.DAL;

namespace Tests
{
    [TestFixture, Category("Unit")]
    public class MyTests
    {
        private Mock<IMyLogger> _logger;
        private Mock<IMySoapService> _service;
        private Mock<IMyDb> _db;
        private Mock<IElasticLowLevelClient> _es;
        private MyEventHandler _handler;
        string key = "123";


        [SetUp]
        public void TestSetup()
        {

            _logger = new Mock<IMyLogger>();
            _service = new Mock<IMySoapService>();
            _db = new Mock<IMyDb>();
            _es = new Mock<IElasticLowLevelClient>();
            _extraClient = new Mock<IMyExtraInfoAgent>();
            _handler = new MyEventHandler(_logger.Object, _db.Object, _service.Object, _es.Object, _extraClient.Object);
        }

        private string json;
        private PostData<object> postdata;
        private Mock<IMyExtraInfoAgent> _extraClient;

        [Test]
        public void Teset1()
        {
            var myAsset = new MyAsset() { Key = key, LastChanged = DateTime.Now - TimeSpan.FromDays(1) };

            _service.Setup(m => m.GetAssetData(It.IsAny<string>())).Returns(Resource.asset);

            _db.Setup(m => m.TryGet(It.IsAny<string>()))
                .Returns(myAsset);
            _es.Setup(
                    m =>
                        m.Index<MyAsset>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                            It.IsAny<PostData<object>>(),
                            It.IsAny<Func<IndexRequestParameters, IndexRequestParameters>>()))
                .Returns(new ElasticsearchResponse<MyAsset>(200, new List<int>() {200}))
                .Callback
                <string, string, string, PostData<Object>, Func<IndexRequestParameters, IndexRequestParameters>>(
                    (a, b, c, str, d) => { postdata = str; });
            _extraClient.Setup(m => m.CalcluateExtraInfo(It.IsAny<MyAsset>()))
                .Returns(myAsset);

            var myEvent = new MyEvent() {Key = key, Updated = DateTime.Now-TimeSpan.FromDays(2)};
            MyEvent message = myEvent;
            _handler.HandleMessage(message);

            _service.Verify(m => m.GetAssetData(It.IsAny<string>()), Times.Never);




        }


        [Test]
        public void ParseXmlTest()
        {
            var handler = new MyEventHandler(_logger.Object, _db.Object, _service.Object, _es.Object, _extraClient.Object);
            var asset = new MyAsset();
            handler.ParseXml(Resource.asset, asset);
            Assert.AreEqual("mytitle", asset.Title);
            Assert.AreEqual("jarle", asset.Owner);

        }


        [Test]
        public void CalculateExtraInfoTest()
        {
            _service.Setup(m => m.GetAdditionalInfomation(It.IsAny<string>()))
                .Returns(new ExtraInfomation()
                {
                    Users = new List<string>() { "jarle", "tone" },
                    ReviewScores = new List<int>() { 9, 10, 11 },
                });
            var handler = new MyExtraInfoAgent(_service.Object);
            var res = handler.CalcluateExtraInfo(new MyAsset());

            Assert.AreEqual(9.5, res.AverageReviewScore);

        }
    }
}   
