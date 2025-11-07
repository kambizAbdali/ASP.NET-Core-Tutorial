using SignalRChatApp.Models;

namespace SignalRChatApp.Services
{
    public interface IMessageService
    {
        Task SaveMessageAsync(Guid roomId, MessageDto message);
        Task<List<MessageDto>> GetMessagesForRoomAsync(Guid roomId);
        Task<int> GetUnreadMessagesCountAsync(Guid roomId);
    }
}