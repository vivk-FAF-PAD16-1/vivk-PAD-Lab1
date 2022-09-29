namespace News.Listener
{
    public interface IAsyncListener
    {
        void Schedule();
        void Stop();
    }
}