using SignalRChatApp.Models;

namespace SignalRChatApp.Services
{
    public interface IChatRoomService
    {
        Task<ChatRoom> CreateChatRoomAsync(string connectionId, string userIdentifier);
        Task<ChatRoom> GetChatRoomForConnectionAsync(string connectionId);
        Task<ChatRoom> GetRoomByIdAsync(Guid roomId);
        Task<List<ChatRoom>> GetActiveRoomsAsync();
        Task UpdateUnreadCountAsync(Guid roomId, int count);
        Task MarkMessagesAsReadAsync(Guid roomId);
        Task RemoveConnectionFromRoomAsync(string connectionId);
        Task UpdateRoomActivityAsync(Guid roomId);
    }
}