using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using SignalRChatApp.Models;
using SignalRChatApp.Services;

namespace SignalRChatApp.Hubs
{
    [Authorize(Roles = "Operator")]
    public class SupportHub : Hub
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly ILogger<SupportHub> _logger;

        public SupportHub(IChatRoomService chatRoomService, IMessageService messageService,
                         IHubContext<ChatHub> chatHubContext, ILogger<SupportHub> logger)
        {
            _chatRoomService = chatRoomService;
            _messageService = messageService;
            _chatHubContext = chatHubContext;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Support operator connected: {Context.ConnectionId}");

            // Add operator to operators group
            await Groups.AddToGroupAsync(Context.ConnectionId, "Operators");

            // Send active rooms list to operator
            await SendRoomListToOperator();

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"Support operator disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        // Method for operator to join a specific room
        public async Task JoinRoom(Guid roomId)
        {
            _logger.LogInformation($"Operator {Context.ConnectionId} joining room {roomId}");

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

            // Load room message history
            var messages = await _messageService.GetMessagesForRoomAsync(roomId);
            await Clients.Caller.SendAsync("LoadMessages", messages);

            // Mark messages as read
            await _chatRoomService.MarkMessagesAsReadAsync(roomId);
            await _chatRoomService.UpdateUnreadCountAsync(roomId, 0);

            _logger.LogInformation($"Operator joined room {roomId}, {messages.Count} messages loaded");
        }

        // Method for operator to leave a room
        public async Task LeaveRoom(Guid roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
        }

        // Method to send message from operator
        public async Task SendMessageToRoom(Guid roomId, string messageText)
        {
            _logger.LogInformation($"Support sending message to room {roomId}: {messageText}");

            var senderName = Context.User.Identity.Name ?? "Support";
            var messageDto = new MessageDto
            {
                Sender = senderName,
                Message = messageText,
                Time = DateTime.Now,
                RoomId = roomId
            };

            // Save message to database
            await _messageService.SaveMessageAsync(roomId, messageDto);

            // Send message to user through ChatHub
            await _chatHubContext.Clients.Groups(roomId.ToString())
                .SendAsync("ReceiveMessage", messageDto);

            // Update room last activity
            await _chatRoomService.UpdateRoomActivityAsync(roomId);

            _logger.LogInformation($"Support message sent successfully to room {roomId}");
        }

        // Method to receive room updates
        public async Task RequestRoomUpdate()
        {
            await SendRoomListToOperator();
        }

        private async Task SendRoomListToOperator()
        {
            try
            {
                var rooms = await _chatRoomService.GetActiveRoomsAsync();
                _logger.LogInformation($"Sending {rooms.Count} rooms to operator");
                await Clients.Caller.SendAsync("LoadRooms", rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending room list to operator");
            }
        }
    }
}