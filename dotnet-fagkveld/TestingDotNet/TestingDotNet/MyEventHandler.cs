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
        private readonly IMyLogger _logger;
        private readonly IMyDb _db;
        private readonly IMySoapService _service;
        private readonly IElasticLowLevelClient _esClient;
        private readonly IMyExtraInfoAgent _extraInfoAgent;
        public readonly XNamespace Ns = "http://jarle.com/myschema";

        public MyEventHandler(IMyLogger logger, IMyDb db, IMySoapService service, IElasticLowLevelClient esClient, IMyExtraInfoAgent extraInfoAgent)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (db == null) throw new ArgumentNullException(nameof(db));
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (esClient == null) throw new ArgumentNullException(nameof(esClient));
            if (extraInfoAgent == null) throw new ArgumentNullException(nameof(extraInfoAgent));
            _logger = logger;
            _db = db;
            _service = service;
            _esClient = esClient;
            _extraInfoAgent = extraInfoAgent;
        }

        public void HandleMessage(MyEvent message)
        {
            _logger.Log($"Got message {message.MessageId}");
            MyAsset asset;
            asset = GetCachedAsset(message, _logger);

            asset = _extraInfoAgent.CalcluateExtraInfo(asset);


            _logger.Log($"Added additional info from service to asset with key {asset.Key}");
            var json = JsonConvert.SerializeObject(asset, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            });
            _logger.Log($"Writing asset with key {asset.Key} to index");
            var res = _esClient.Index<MyAsset>("searching-index", nameof(MyAsset), asset.Key, json, new Func<IndexRequestParameters, IndexRequestParameters>((p=>null)));
            if(!res.Success)
                throw new Exception(res.DebugInformation);
        }
        private MyAsset GetCachedAsset(MyEvent message, IMyLogger logger)
        {
            MyAsset asset;
            
            asset = _db.TryGet(message.Key);
            if (asset == null || asset.LastChanged < message.Updated)
            {
                logger.Log($"No up to date asset with key {asset.Key} found in cache");
                var assetData = _service.GetAssetData(message.Key);
                asset = new MyAsset();
                asset.Key = message.Key;

                ParseXml(assetData, asset);
                logger.Log($"Adding asset with key {asset.Key} to cache");
                _db.AddOrUpdate(asset);

            }
            return asset;
        }

        public void ParseXml(string  assetData, MyAsset asset)
        {
            var xelement = XElement.Parse(assetData);

            asset.Title = xelement.Element(Ns + "title").Value;
            asset.Owner = xelement.Element(Ns + "creator").Value;   
        }
    }
}
