using System;
using System.Threading;
using Microsoft.AspNet.SignalR.Client;

namespace JsConsole.Services
{
    public class ClientHubService
    {
        private readonly HubConnection _connection;
        private readonly IHubProxy _proxy;
        private readonly Action<string> _senDataAction;
        private readonly Action<string> _senFunctionsAction;
        
        private bool _isStarted;
        private CancellationTokenSource _tokenSource;


        public ClientHubService(string pathUrl, Action<string> senDataAction, Action<string> senFunctionsAction)
        {
            _senDataAction = senDataAction;
            _senFunctionsAction = senFunctionsAction;
            _connection = new HubConnection(pathUrl);
            _proxy = _connection.CreateHubProxy("BrowserKit");
            _proxy.On<string>("updateLog", OnSendData);
            _proxy.On<string>("updateSendFunctions", OnSendFunctions);
        }

        public bool IsStarted
        {
            get { return _isStarted; }
        }


        public void Start()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Dispose();
            }
            _tokenSource = new CancellationTokenSource();
            try
            {
                _connection.Start().Wait(_tokenSource.Token);
                _isStarted = true;
            }
            catch (Exception ex)
            {
            }
        }

        public void Stop()
        {
            _isStarted = false;
            _tokenSource.Cancel();

            //_isRunning = false;
            //if(_connection.State == ConnectionState.Connected)
            _connection.Stop();
            //_connection.Dispose();
        }

        private void OnSendData(string data)
        {
            if (_senDataAction != null)
            {
                _senDataAction(data);
            }
        }
        private void OnSendFunctions(string data)
        {
            if (_senFunctionsAction != null)
            {
                _senFunctionsAction(data);
            }
        }

        public void SendScript(string data, string idClient)
        {
            if (IsStarted)
                _proxy.Invoke<string>("exec", data, idClient);
        }
    }
}