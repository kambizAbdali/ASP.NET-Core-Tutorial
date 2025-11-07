using System.Collections.Concurrent;

namespace SignalRChatApp.Services
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<string, string> _connections;

        public ConnectionManager()
        {
            _connections = new ConcurrentDictionary<string, string>();
        }

        public void AddConnection(string connectionId, string userId)
        {
            _connections.TryAdd(connectionId, userId);
        }

        public void RemoveConnection(string connectionId)
        {
            _connections.TryRemove(connectionId, out _);
        }

        public string GetUserId(string connectionId)
        {
            return _connections.TryGetValue(connectionId, out var userId) ? userId : null;
        }

        public IEnumerable<string> GetAllConnections()
        {
            return _connections.Keys;
        }
    }
}