using Microsoft.EntityFrameworkCore;
using SignalRChatApp.Data;
using SignalRChatApp.Models;

namespace SignalRChatApp.Services
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly ApplicationDbContext _context;

        public ChatRoomService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChatRoom> CreateChatRoomAsync(string connectionId, string userIdentifier)
        {
            // Create a unique group name for this connection
            var groupName = $"Room_{userIdentifier}_{Guid.NewGuid()}";

            var room = new ChatRoom
            {
                Id = Guid.NewGuid(),
                ConnectionId = connectionId,
                UserIdentifier = userIdentifier,
                GroupName = groupName,
                CreatedAt = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow
            };

            _context.ChatRooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<ChatRoom> GetChatRoomForConnectionAsync(string connectionId)
        {
            return await _context.ChatRooms
                .FirstOrDefaultAsync(r => r.ConnectionId == connectionId && r.IsActive);
        }

        public async Task<ChatRoom> GetRoomByIdAsync(Guid roomId)
        {
            return await _context.ChatRooms.FindAsync(roomId);
        }

        public async Task<List<ChatRoom>> GetActiveRoomsAsync()
        {
            return await _context.ChatRooms
                .Where(r => r.IsActive && r.ChatMessages.Any())
                .OrderByDescending(r => r.LastActivity)
                .ToListAsync();
        }

        public async Task UpdateUnreadCountAsync(Guid roomId, int count)
        {
            var room = await _context.ChatRooms.FindAsync(roomId);
            if (room != null)
            {
                room.UnreadMessages = count;
                room.LastActivity = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkMessagesAsReadAsync(Guid roomId)
        {
            var unreadMessages = await _context.ChatMessages
                .Where(m => m.ChatRoomId == roomId && !m.IsRead)
                .ToListAsync();

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveConnectionFromRoomAsync(string connectionId)
        {
            var room = await GetChatRoomForConnectionAsync(connectionId);
            if (room != null)
            {
                room.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateRoomActivityAsync(Guid roomId)
        {
            var room = await _context.ChatRooms.FindAsync(roomId);
            if (room != null)
            {
                room.LastActivity = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}