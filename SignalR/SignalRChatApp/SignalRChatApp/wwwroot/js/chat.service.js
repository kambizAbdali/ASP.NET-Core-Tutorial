class ChatService {
    constructor() {
        this.connection = null;
        this.isConnected = false;
        this.activeRoomId = null;
    }

    // Method to start connection to Hub
    async startConnection() {
        console.log("Starting SignalR connection...");

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/chathub")
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Information)
            .build();

        // Setup event handlers
        this.setupEventHandlers();

        try {
            await this.connection.start();
            this.isConnected = true;
            console.log("✅ Connected to Chat Hub");
            this.updateConnectionStatus();
            return true;
        } catch (err) {
            console.error("❌ Connection failed:", err);
            this.isConnected = false;
            this.updateConnectionStatus();
            // Retry connection after 5 seconds
            setTimeout(() => this.startConnection(), 5000);
            return false;
        }
    }

    // Setup event handlers
    setupEventHandlers() {
        // Receive new messages
        this.connection.on("ReceiveMessage", (message) => {
            console.log("📨 Received message:", message);
            this.displayMessage(message);
        });

        // Handle reconnect
        this.connection.onreconnecting((error) => {
            console.log("🔄 Reconnecting to Chat Hub...", error);
            this.isConnected = false;
            this.updateConnectionStatus();
        });

        this.connection.onreconnected((connectionId) => {
            console.log("✅ Reconnected to Chat Hub. Connection ID:", connectionId);
            this.isConnected = true;
            this.updateConnectionStatus();
        });

        // Handle connection close
        this.connection.onclose((error) => {
            console.log("🔴 Connection to Chat Hub closed", error);
            this.isConnected = false;
            this.updateConnectionStatus();
        });
    }

    // Update connection status in UI
    updateConnectionStatus() {
        const statusElement = document.getElementById('connectionStatus');
        if (statusElement) {
            const status = this.isConnected ? 'Connected' : 'Disconnected';
            const badgeClass = this.isConnected ? 'bg-success' : 'bg-danger';
            statusElement.textContent = status;
            statusElement.className = `badge ${badgeClass}`;
        }
    }

    // Method to send message
    async sendMessage(messageText) {
        if (this.connection && this.isConnected) {
            try {
                console.log("📤 Sending message:", messageText);
                await this.connection.invoke("SendMessageToRoom", messageText);
                console.log("✅ Message sent successfully");
            } catch (err) {
                console.error("❌ Send message error:", err);
                alert("Failed to send message. Please check your connection.");
            }
        } else {
            console.warn("⚠️ Not connected, cannot send message");
            alert("Not connected to chat server. Please wait...");
        }
    }

    // Display message in UI
    displayMessage(message) {
        const messageList = document.getElementById('messageList');
        if (!messageList) {
            console.error("❌ messageList element not found");
            return;
        }

        // Remove loading message if it exists
        const loadingMessage = document.getElementById('loadingMessage');
        if (loadingMessage) {
            loadingMessage.remove();
        }

        const messageElement = this.createMessageElement(message);
        messageList.appendChild(messageElement);

        // Scroll to bottom
        messageList.scrollTop = messageList.scrollHeight;
    }

    // Create message element
    createMessageElement(message) {
        const div = document.createElement('div');
        div.className = `message ${message.sender === 'User' ? 'user-message' : 'support-message'}`;

        const time = new Date(message.time).toLocaleTimeString('en-US');

        div.innerHTML = `
            <div class="message-header">
                <strong>${message.sender}</strong>
                <small class="text-muted">${time}</small>
            </div>
            <div class="message-body">${message.message}</div>
        `;

        return div;
    }
}

// Create global instance
const chatService = new ChatService();