namespace SignalRChatApp.Services
{
    public interface IConnectionManager
    {
        void AddConnection(string connectionId, string userId);
        void RemoveConnection(string connectionId);
        string GetUserId(string connectionId);
        IEnumerable<string> GetAllConnections();
    }
}