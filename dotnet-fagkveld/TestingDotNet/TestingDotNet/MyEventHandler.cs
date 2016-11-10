using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Elasticsearch.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TestingDotNet.DAL;


namespace TestingDotNet
{
    public class MyEventHandler : IHandleEvents<MyEvent>
    {
        public readonly XNamespace Ns = "http://jarle.com/myschema";

        public void HandleMessage(MyEvent message)
        {
            var logger = new MyLogger();
            logger.Log($"Got message {message.MessageId}");
            var db = new MyDb();
            var asset = db.TryGet(message.Key);
            if (asset == null ||  asset.LastChanged < message.Updated)
            {
                logger.Log($"No up to date asset with key {asset.Key} found in cache");
                var client = new SoapClient();
                var assetData = client.GetAssetData(message.Key);
                var xelement = XElement.Parse(assetData);
                var root = xelement.Element(Ns + "root");
                asset = new MyAsset();
                asset.Key = message.Key;
                asset.LastChanged= DateTime.Parse(root.Element(Ns + "title").Value);
                asset.Owner = root.Element(Ns + "creator").Value;
                logger.Log($"Adding asset with key {asset.Key} to cache");
                db.AddOrUpdate(asset);
            }
            var myClient = new SoapClient();
            var extraInfo = myClient.GetAdditionalInfomation(message.Key);
            asset.UserCount = extraInfo.Users.Count();
            asset.AverageReviewScore = extraInfo.ReviewScores.Sum()/extraInfo.ReviewScores.Count;
            logger.Log($"Added additional info from service to asset with key {asset.Key}");
            var esClient = new ElasticLowLevelClient(new ConnectionConfiguration(new Uri("http://docker-local:9200/")));
            var json = JsonConvert.SerializeObject(asset, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            });
            logger.Log($"Writing asset with key {asset.Key} to index");
            var res = esClient.Index<MyAsset>("searching-index", nameof(MyAsset), asset.Key, json);
            if(!res.Success)
                throw new Exception(res.DebugInformation);

        }
    }
}
