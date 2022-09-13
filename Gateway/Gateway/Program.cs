using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Gateway.Listener;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Gateway
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var prefixes = new[] { "http://localhost:40404/" };
            var listener = new AsyncListener(prefixes) as IAsyncListener;
            listener.Schedule();
            
            Thread.Sleep(100000);
            
            listener.Stop();
        }
    }
}