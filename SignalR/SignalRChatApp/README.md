```markdown
# SignalRChatApp

A real-time chat application built with ASP.NET Core and SignalR, featuring both user chat rooms and operator support capabilities.

## 🚀 Features

### Core Functionality
- **Real-time Messaging**: Instant message delivery using SignalR
- **Multiple Chat Rooms**: Dynamic room creation and management
- **User & Operator Roles**: Separate interfaces for users and support operators
- **Message Persistence**: Chat history storage and retrieval
- **Auto-reconnect**: Robust connection handling with automatic reconnection

### User Features
- Join and participate in chat rooms
- Real-time message sending and receiving
- Clean, responsive chat interface
- Room activity tracking

### Support Operator Features
- Monitor multiple chat rooms simultaneously
- Real-time support message delivery
- Room switching without losing context
- Broadcast messages to specific rooms

## 🏗️ Architecture

### Technology Stack
- **Backend**: ASP.NET Core, SignalR, C#
- **Frontend**: JavaScript, HTML5, CSS3
- **Real-time Communication**: WebSockets with SignalR fallbacks
- **Data Persistence**: Entity Framework (configurable)


## 🛠️ Installation & Setup

### Prerequisites
- .NET 6.0 SDK or later
- Web browser with WebSocket support
- SQL Server (optional, for production)

### Quick Start
1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/SignalRChatApp.git
   cd SignalRChatApp
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure the application**
   - Update connection strings in `appsettings.json`
   - Configure CORS if needed for cross-origin requests

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**
   - User Chat: `https://localhost:7158`
   - Support Interface: `https://localhost:7158/Support`

### Configuration
Key settings in `appsettings.json`:
```json
{
  "SignalR": {
    "MaxMessageSize": 32768,
    "HandshakeTimeout": 15000
  },
  "ConnectionStrings": {
    "DefaultConnection": "Your connection string here"
  }
}
```

## 📡 SignalR Hubs

### ChatHub (`/chathub`)
- **JoinRoom(roomId)**: Join a specific chat room
- **SendMessage(roomId, sender, message)**: Send message to room
- **LeaveRoom(roomId)**: Leave current room

### SupportHub (`/supporthub`)
- **JoinSupport()**: Register as support operator
- **JoinSupportRoom(roomId)**: Monitor specific room
- **SendSupportMessage(roomId, message)**: Send message as operator

## 🎯 Usage

### For Users
1. Navigate to the home page
2. Select or create a chat room
3. Start sending messages in real-time
4. All messages are persisted and synced across clients

### For Support Operators
1. Access the support interface
2. View active chat rooms
3. Select a room to monitor and participate
4. Send support messages that appear as operator messages

## 🔧 API Endpoints

### Chat Endpoints
- `GET /api/rooms` - Get active chat rooms
- `GET /api/messages/{roomId}` - Get room message history
- `POST /api/messages` - Send new message (alternative to SignalR)

### Support Endpoints
- `GET /api/support/rooms` - Get rooms requiring support
- `GET /api/support/active` - Get active support sessions

## 🐛 Troubleshooting

### Common Issues

1. **Messages not updating in real-time**
   - Check SignalR connection status in browser console
   - Verify WebSocket support in browser
   - Check firewall/network restrictions

2. **Duplicate messages appearing**
   - Ensure event handlers are not registered multiple times
   - Check for multiple SignalR connections
   - Verify message ID uniqueness

3. **Connection drops frequently**
   - Check network stability
   - Review SignalR reconnection settings
   - Verify server resource availability

### Debugging
Enable detailed logging by setting log level to `Debug` in `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore.SignalR": "Debug"
    }
  }
}
```

## 📊 Monitoring

### Key Metrics
- Active connections per hub
- Messages sent/received per second
- Room creation/deletion rates
- Support operator response times

### Health Checks
- SignalR hub connectivity
- Database connection status
- Message queue depth (if applicable)

## 🔒 Security Considerations

- Implement authentication and authorization
- Validate message content and size
- Use HTTPS in production
- Sanitize user input
- Implement rate limiting
- Regular security updates

## 🚀 Deployment

### Development
```bash
dotnet run --environment Development
```

### Production
```bash
dotnet publish -c Release
cd bin/Release/net6.0/publish
dotnet SignalRChatApp.dll
```

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY published/ .
ENTRYPOINT ["dotnet", "SignalRChatApp.dll"]
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## 🆘 Support

For support and questions:
- Check the [troubleshooting section](#troubleshooting)
- Review application logs
- Open an issue on GitHub
- Contact the development team

## 🔄 Version History

- **v1.0.0** - Initial release with real-time chat and support features
- **v1.1.0** - Added message persistence and room management
- **v1.2.0** - Enhanced support operator interface and monitoring

---

**Built with ❤️ using ASP.NET Core and SignalR**
```