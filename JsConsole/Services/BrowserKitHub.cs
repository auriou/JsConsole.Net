using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsConsole.Extensions;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace JsConsole.Services
{
    [HubName("BrowserKit")]
    public class BrowserKitHub : Hub
    {

        private readonly static HashSet<ClientConnected> _connections = new HashSet<ClientConnected>();
        private static ClientConnected _connectionControl;

        public static Action ConnectionsChanged { get; set; }

        private static void OnConnectionsChanged()
        {
            if (ConnectionsChanged != null)
            {
                ConnectionsChanged();
            }
        }

        public static HashSet<ClientConnected> Connections
        {
            get { return _connections; }
        }

        public override Task OnConnected()
        {
            var client = ClientConnected.Factory(Context);
            if (!client.IsClientDotNet())
            {
                var lstSame = _connections.Same(client);
                lstSame.ForEach(p => _connections.Remove(p));
                _connections.Add(client);
                OnConnectionsChanged();
                Clients.Caller.isConnected();
            }
            else
            {
                _connectionControl = client;
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var client = _connections.GetById(Context.ConnectionId);
            if (client != null)
            {
                _connections.Remove(client);
                OnConnectionsChanged();
            }
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            var client = ClientConnected.Factory(Context);
            if (!_connections.Contains(client))
            {
                if (!client.IsClientDotNet())
                {
                    var lstSame = _connections.Same(client);
                    lstSame.ForEach(p => _connections.Remove(p));
                    _connections.Add(client);
                    OnConnectionsChanged();
                }
                else
                {
                    _connectionControl = client;
                }
            }
            return base.OnReconnected();
        }

        public void Exec(string data, string idClient)
        {
            System.Diagnostics.Debug.WriteLine(data);
            if(idClient.IsNullOrEmpty())
                Clients.Others.updateExec(data);
            else
                Clients.Client(idClient).updateExec(data);
        }
        public void Log(string data)
        {
            if (_connectionControl != null)
                Clients.Client(_connectionControl.Id).updateLog(data);
        }
        public void SendFunctions(string data)
        {
            if (_connectionControl != null)
            {
                var client = _connections.GetById(Context.ConnectionId);
                if (client != null)
                {
                    client.Functions = data;
                }
                Clients.Client(_connectionControl.Id).updateSendFunctions(data);
            }
        }
    }
}