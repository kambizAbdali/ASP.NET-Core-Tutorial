using Microsoft.AspNetCore.SignalR;
using SignalRChatApp.Models;
using SignalRChatApp.Services;

namespace SignalRChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IMessageService _messageService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatRoomService chatRoomService, IMessageService messageService, ILogger<ChatHub> logger)
        {
            _chatRoomService = chatRoomService;
            _messageService = messageService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");

            try
            {
                // Only create rooms for regular users (not operators)
                if (!Context.User.Identity.IsAuthenticated)
                {
                    var room = await _chatRoomService.CreateChatRoomAsync(
                        Context.ConnectionId,
                        Context.ConnectionId
                    );

                    await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());

                    // Send welcome message
                    await Clients.Caller.SendAsync("ReceiveMessage",
                        new MessageDto
                        {
                            Sender = "System",
                            Message = "Welcome to online support! Please ask your question.",
                            Time = DateTime.Now
                        });
                }

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnConnectedAsync");
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");

            try
            {
                await _chatRoomService.RemoveConnectionFromRoomAsync(Context.ConnectionId);
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnDisconnectedAsync");
            }
        }

        public async Task SendMessageToRoom(string message)
        {
            try
            {
                _logger.LogInformation($"Message received from {Context.ConnectionId}: {message}");

                var room = await _chatRoomService.GetChatRoomForConnectionAsync(Context.ConnectionId);

                if (room != null)
                {
                    var messageDto = new MessageDto
                    {
                        Sender = "User",
                        Message = message,
                        Time = DateTime.Now,
                        RoomId = room.Id
                    };

                    // Save message to database
                    await _messageService.SaveMessageAsync(room.Id, messageDto);

                    // Send message to all group members (user and support)
                    await Clients.Groups(room.Id.ToString()).SendAsync("ReceiveMessage", messageDto);

                    // Update unread messages count for all operators
                    var unreadCount = await _messageService.GetUnreadMessagesCountAsync(room.Id);
                    await _chatRoomService.UpdateUnreadCountAsync(room.Id, unreadCount);

                    // Notify operators to update room list
                    await Clients.Group("Operators").SendAsync("UpdateRoomList");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendMessageToRoom");
                throw;
            }
        }
    }
}