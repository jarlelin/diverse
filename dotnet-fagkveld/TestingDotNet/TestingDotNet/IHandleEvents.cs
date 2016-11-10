namespace TestingDotNet
{
    public interface IHandleEvents<T>
    {
        void HandleMessage(T message);
    }
}