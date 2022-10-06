namespace Auth.Counter
{
    public interface ICounter
    {
        void Register();
        void Unregister();
        
        bool IsFull();
    }
}