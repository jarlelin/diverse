using System.Collections;
using System.Collections.Generic;

namespace TestingDotNet
{
    public class SoapClient : System.ServiceModel.ClientBase<IMySoapService>, IMySoapService
    {
        public string GetAssetData(string key)
        {
            throw new System.NotImplementedException();
        }

        public ExtraInfomation GetAdditionalInfomation(string messageKey)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ExtraInfomation
    {
        public IList<string> Users { get; set; }
        public IList<int> ReviewScores { get; set; }
    }
}   