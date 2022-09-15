using System.Collections.Generic;

namespace Gateway.Storage
{
    public class RouteStorage : IStorage
    {
        private Dictionary<string, DestinationContainer> _storage;
        
        private readonly object _locker;
        
        private const int DefaultCapacity = (1 << 5);

        public RouteStorage()
        {
            _locker = new object();
            lock (_locker)
            {
                _storage = new Dictionary<string, DestinationContainer>(DefaultCapacity);
            }
        }

        public void Register(string endpoint, string destinationUri)
        {
            lock (_locker)
            {
                var contains = _storage.ContainsKey(endpoint);
                DestinationContainer destinationContainer = null;
                
                if (contains == false)
                {
                    destinationContainer = new DestinationContainer();
                    _storage.Add(endpoint, destinationContainer);
                }
                else
                {
                    destinationContainer = _storage[endpoint];
                }
                
                destinationContainer.Add(destinationUri);
            }
        }
        
        public void Unregister(string endpoint, string destinationUri)
        {
            lock (_locker)
            {
                var contains = _storage.ContainsKey(endpoint);
                if (contains == false)
                {
                    return;
                }
                
                var destinationContainer = _storage[endpoint];
                destinationContainer.Remove(destinationUri);
            }
        }

        public (bool, string) TryGet(string endpoint)
        {
            lock (_locker)
            {
                var contains = _storage.ContainsKey(endpoint);
                if (contains == false)
                {
                    return (false, null);
                }

                var destinationContainer = _storage[endpoint];
                return destinationContainer.TryGet();
            }
        }
    }
}