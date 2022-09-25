namespace News.Counter
{
    public class SyncCounter : ICounter
    {
        private readonly int _maxCount;
        private readonly object _locker;
        
        private int _counter;

        public SyncCounter(int maxCount)
        {
            _maxCount = maxCount;
            _locker = new object();

            _counter = 0;
        }
        
        public void Register()
        {
            lock (_locker)
            {
                _counter++;
            }
        }

        public void Unregister()
        {
            lock (_locker)
            {
                _counter--;
            }
        }

        public bool IsFull()
        {
            lock (_locker)
            {
                return _counter == _maxCount;
            }
        }
    }
}