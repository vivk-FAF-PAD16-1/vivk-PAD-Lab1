using System;
using System.Net;
using System.Text;
using System.Threading;

namespace Gateway.Listener
{
    public class AsyncListener : IAsyncListener
    {
        private readonly HttpListener _listener;

        private bool _isRunning;
        
        public AsyncListener(string[] prefixes)
        {
            _listener = new HttpListener();
            for (var i = 0; i < prefixes.Length; i++)
            {
                _listener.Prefixes.Add(prefixes[i]);
            }

            _isRunning = false;
        }

        public void Schedule()
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;
            _listener.Start();
            _listener.BeginGetContext(ContextHandler, _listener);
        }

        public void Stop()
        {
            if (_isRunning == false)
            {
                return;
            }

            _isRunning = false;
            _listener.Stop();
        }

        private void ContextHandler(IAsyncResult result)
        {
            if (_isRunning == false)
            {
                return;
            }
            
            var listener = result.AsyncState as HttpListener;
            if (listener == null)
            {
                Stop();
                return;
            }

            var context = listener.EndGetContext(result);
            listener.BeginGetContext(ContextHandler, listener);
            
            var request = context.Request;
            var response = context.Response;

            // TODO: DO NORMAL ROUTING
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var buffer = Encoding.UTF8.GetBytes($"Hello from thread {threadId}!");
            response.ContentLength64 = buffer.Length;

            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            Thread.Sleep(10000);
        }
    }
}