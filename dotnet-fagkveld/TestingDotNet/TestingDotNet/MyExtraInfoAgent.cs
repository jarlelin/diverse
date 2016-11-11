using System;
using System.Collections.Generic;
using System.Linq;

namespace TestingDotNet
{
    public class MyExtraInfoAgent : IMyExtraInfoAgent
    {
        private readonly IMySoapService _service;

        public MyExtraInfoAgent(IMySoapService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            _service = service;
        }

        public MyAsset CalcluateExtraInfo(MyAsset asset)
        {

            var extraInfo = _service.GetAdditionalInfomation(asset.Key);
            asset.UserCount = extraInfo.Users.Count();


            IEnumerable<int> validScores = extraInfo.ReviewScores.
                Where(s=>s<=10).
                Where(s=>s>0);
            asset.AverageReviewScore = 
                validScores
                .Sum() /(double) validScores.Count();
            return asset;

        }
    }
}