using System;
using System.Collections.Generic;

namespace Discovery.Storage
{
    public struct Destination : IEquatable<Destination>
    {
        public string Uri { get; }

        public int Mark { get; set; }

        public Destination(string uri)
        {
            Uri = uri;
            Mark = 0;
        }
        
        public bool Equals(Destination other)
        {
            return Uri == other.Uri;
        }

        public override bool Equals(object obj)
        {
            return obj is Destination other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Uri != null ? Uri.GetHashCode() : 0);
        }
    }
    
    public class DestinationContainer
    {
        private const int MaxMarks = 3;
        
        private List<Destination> _container;

        private int _currentIndex;

        private readonly object _locker;
        
        private const int DefaultCapacity = (1 << 5);
        
        public DestinationContainer()
        {
            _locker = new object();
            lock (_locker)
            {
                _container = new List<Destination>(DefaultCapacity); 
                _currentIndex = 0;
            }
        }

        public void Add(string uri)
        {
            lock (_locker)
            {
                for (var i = 0; i < _container.Count; i++)
                {
                    var item = _container[i];
                    if (item.Uri == uri)
                    {
                        return;
                    }
                }

                var destination = new Destination(uri);
                _container.Add(destination);
            }
        }

        public void Remove(string uri)
        {
            lock (_locker)
            {
                for (var i = 0; i < _container.Count; i++)
                {
                    var item = _container[i];
                    if (item.Uri == uri)
                    {
                        _container.RemoveAt(i);
                        return;
                    }
                }
            } 
        }

        public (bool, string) TryGet()
        {
            lock (_locker)
            {
                if (_container.Count == 0)
                {
                    return (false, null);
                }

                _currentIndex = (_currentIndex + 1) % _container.Count;
                if (_container[_currentIndex].Mark > 0)
                {
                    var destination = _container[_currentIndex];
                    destination.Mark--;
                    _container[_currentIndex] = destination;
                }
                
                return (true, _container[_currentIndex].Uri);
            }
        }

        public void Mark(string uri)
        {
            lock (_locker)
            {
                if (_container.Count == 0)
                {
                    return;
                }
                
                for (var i = 0; i < _container.Count; i++)
                {
                    if (string.Equals(uri, _container[i].Uri))
                    {
                        var destination = _container[i];
                        destination.Mark += 2;

                        if (destination.Mark > MaxMarks)
                        {
                            Console.WriteLine($"Destination={uri} is removed!");
                            _container.RemoveAt(i);
                        }
                        else
                        {
                            Console.WriteLine($"INC: Mark of {destination.Uri} = {destination.Mark-1}");
                            _container[i] = destination;
                        }
                    }
                }
            }
        }
    }
}