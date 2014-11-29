using System.Collections.Generic;
using System.Linq;

namespace JsConsole.Services
{
    public class ConnectionMapping<T> : Dictionary<T, HashSet<string>>
    {
        public void Add(T key, string connectionId)
        {
            lock (this)
            {
                HashSet<string> connections;
                if (!this.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    this.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (this.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (this)
            {
                HashSet<string> connections;
                if (!this.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        this.Remove(key);
                    }
                }
            }
        }
    }
}