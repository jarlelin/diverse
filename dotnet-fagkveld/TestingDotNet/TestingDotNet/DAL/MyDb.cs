using System;
using System.Data.Entity;

namespace TestingDotNet.DAL
{
    public class MyDb : DbContext
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