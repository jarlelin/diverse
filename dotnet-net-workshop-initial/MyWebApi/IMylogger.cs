namespace MyWebApi
{
    public interface IMyLogger
    {
        void Debug(string message);
        void Error(string message);
        void Warn(string message);
    }
}