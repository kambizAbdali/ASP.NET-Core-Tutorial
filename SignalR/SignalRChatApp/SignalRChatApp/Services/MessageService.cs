using Microsoft.EntityFrameworkCore;
using SignalRChatApp.Data;
using SignalRChatApp.Models;

namespace SignalRChatApp.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;

        public MessageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveMessageAsync(Guid roomId, MessageDto messageDto)
        {
            var chatRoom = await _context.ChatRooms.FindAsync(roomId);

            if (chatRoom != null)
            {
                var message = new ChatMessage
                {
                    Id = Guid.NewGuid(),
                    Sender = messageDto.Sender,
                    Message = messageDto.Message,
                    Time = messageDto.Time,
                    IsRead = false,
                    ChatRoomId = roomId
                };

                _context.ChatMessages.Add(message);

                // Update last activity and increment unread messages
                chatRoom.LastActivity = DateTime.UtcNow;
                chatRoom.UnreadMessages++;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<MessageDto>> GetMessagesForRoomAsync(Guid roomId)
        {
            return await _context.ChatMessages
                .Where(m => m.ChatRoomId == roomId)
                .OrderBy(m => m.Time)
                .Select(m => new MessageDto
                {
                    Sender = m.Sender,
                    Message = m.Message,
                    Time = m.Time,
                    RoomId = m.ChatRoomId
                })
                .ToListAsync();
        }

        public async Task<int> GetUnreadMessagesCountAsync(Guid roomId)
        {
            return await _context.ChatMessages
                .CountAsync(m => m.ChatRoomId == roomId && !m.IsRead);
        }
    }
}