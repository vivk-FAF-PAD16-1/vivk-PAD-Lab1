namespace Auth.Listener
{
    public interface IAsyncListener
    {
        void Schedule();
        void Stop();
    }
}