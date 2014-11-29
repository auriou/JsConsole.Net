using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace JsConsole.Services
{
    public class WebServerService// : IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource;
        private IDisposable _server;
        private readonly int _port;
        private Task _task;

        public WebServerService(int  port)
        {
            _port = port;
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _task = Task.Factory.StartNew(RunServer, TaskCreationOptions.LongRunning
                , _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            
            _cancellationTokenSource.Cancel();
            _task.Dispose();
            _cancellationTokenSource.Dispose();
            //_server.Dispose();
            
        }

        public void RunServer(object obj)
        {
            var options = CreateOptions(_port);
            _server = WebApp.Start<Startup>(options);
        }

        public static StartOptions CreateOptions(int port)
        {
            var options = new StartOptions();
            options.Urls.Add(string.Format("http://{0}:{1}", "localhost", port));

                var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            if (ipHostInfo != null)
            {
                options.Urls.Add(string.Format("http://{0}:{1}", ipHostInfo.HostName, port));
                foreach (var address in ipHostInfo.AddressList)
                {
                    if (address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        options.Urls.Add(string.Format("http://{0}:{1}", address, port));
                    }
                }
            }

            return options;
        }

        bool _disposed = false;


        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_server != null)
                {
                    _server.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
