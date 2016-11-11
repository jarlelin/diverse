using System;
using System.Data.Entity;
using System.Threading;

namespace TestingDotNet.DAL
{
    public class MyDb : DbContext, IMyDb
    {
        public MyAsset TryGet(string messageKey)
        {
            throw new NotImplementedException();
        }

        public MyAsset AddOrUpdate(MyAsset asset)
        {
            throw new NotImplementedException();
        }

    }
}