namespace News.Counter
{
    public interface ICounter
    {
        void Register();
        void Unregister();
        
        bool IsFull();
    }
}