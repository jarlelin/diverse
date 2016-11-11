namespace TestingDotNet
{
    public interface IMySoapService
    {
        string GetAssetData(string key);
        ExtraInfomation GetAdditionalInfomation(string messageKey);
    }
}