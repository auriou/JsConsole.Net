using System;
using System.Linq;
using Microsoft.AspNet.SignalR.Hubs;

namespace JsConsole.Services
{
    public class ClientConnected 
    {
        public static ClientConnected Factory(HubCallerContext context)
        {
            object tempObject;
            var userAgent = "";
            var referer = "";

            context.Request.Environment.TryGetValue("server.RemoteIpAddress", out tempObject);
            try
            {
                userAgent = context.Headers.Where(p => p.Key == "User-Agent").Select(p => p.Value).FirstOrDefault();
                referer = context.Headers.Where(p => p.Key == "Referer").Select(p => p.Value).FirstOrDefault();
            }
            catch (Exception)
            {

            }
            
            var ipAddress = (string)tempObject ?? "";
            return new ClientConnected() { Id = context.ConnectionId, Ip = ipAddress, UserAgent = userAgent, Referer = referer };
        }

        public bool IsClientDotNet()
        {
            if (UserAgent != null)
            {
                if (UserAgent.StartsWith("SignalR.Client.NET45"))
                    return true;
            }
            return false;
        }

        public bool IsSame(ClientConnected other)
        {
            return this.Ip == other.Ip && this.Referer == other.Referer && this.UserAgent == other.UserAgent;
        }

        protected bool Equals(ClientConnected other)
        {
            return string.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public string Ip { get; set; }
        public string Id { get; set; }
        public string UserAgent { get; set; }
        public string Referer { get; set; }
        public string Functions { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClientConnected) obj);
        }

        public override string ToString()
        {
            return String.Format("{0} - {1} - {2} - {3}", Ip, UserAgent,Referer, Id);
        }
    }
}