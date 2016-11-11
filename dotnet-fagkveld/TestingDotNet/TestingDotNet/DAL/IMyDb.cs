    namespace TestingDotNet.DAL
{
    public interface IMyDb
    {
        MyAsset AddOrUpdate(MyAsset asset);
        MyAsset TryGet(string messageKey);
    }
}