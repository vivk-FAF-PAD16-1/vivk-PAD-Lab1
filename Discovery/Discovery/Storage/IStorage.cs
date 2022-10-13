namespace Discovery.Storage
{
    public interface IStorage
    {
        void Register(string endpoint, string destinationUri);
        void Unregister(string endpoint, string destinationUri);

        (bool, string, string, string) TryGet(string endpoint);
        void TryMark(string uri, string endpoint);
    }
}