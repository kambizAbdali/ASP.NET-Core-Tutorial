class SupportService {
    constructor() {
        this.connection = null;
        this.chatConnection = null;
        this.isConnected = false;
        this.activeRoomId = null;
        this.rooms = [];
    }

    // Start connection to SupportHub
    async startConnection() {
        console.log("Starting Support Hub connection...");

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/supporthub")
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this.chatConnection = new signalR.HubConnectionBuilder()
            .withUrl("/chathub")
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Information)
            .build();

        // Setup event handlers BEFORE starting connection
        this.setupEventHandlers();

        try {
            await this.connection.start();
            await this.chatConnection.start();
            this.isConnected = true;
            console.log("✅ Connected to Support Hub & Chat Hub");
            this.updateSupportStatus();
            return true;
        } catch (err) {
            console.error("❌ Connection failed:", err);
            this.isConnected = false;
            this.updateSupportStatus();
            setTimeout(() => this.startConnection(), 5000);
            return false;
        }
    }

    // Setup event handlers
    setupEventHandlers() {
        // Receive rooms list
        this.connection.on("LoadRooms", (rooms) => {
            console.log("📋 Rooms loaded:", rooms);
            this.rooms = rooms;
            this.updateRoomList(rooms);
        });

        // Real-time room list updates
        this.connection.on("UpdateRoomList", async () => {
            console.log("🔄 Room list update requested");
            await this.refreshRoomList();
        });

        // Load room message history
        this.connection.on("LoadMessages", (messages) => {
            console.log("📨 Messages loaded for active room:", messages.length);
            this.displayMessages(messages);
        });

        // Receive real-time messages from ChatHub
        this.chatConnection.on("ReceiveMessage", (message) => {
            console.log("📬 New message received:", message);
            if (this.activeRoomId && this.activeRoomId === message.roomId) {
                console.log("📝 Displaying message in active room");
                this.displayMessage(message);
                this.markRoomAsRead(message.roomId);
            } else {
                console.log("🔔 New message in different room, updating badge");
                this.updateUnreadCount(message.roomId, true);
                // Auto-refresh room list when new message arrives
                this.refreshRoomList();
            }
        });
    }

    // Refresh room list from server
    async refreshRoomList() {
        if (this.connection && this.isConnected) {
            try {
                await this.connection.invoke("RequestRoomUpdate");
                console.log("🔄 Room list refreshed");
            } catch (err) {
                console.error("❌ Error refreshing room list:", err);
            }
        }
    }

    // Update support status in UI
    updateSupportStatus() {
        const statusElement = document.getElementById('supportStatus');
        if (statusElement) {
            const status = this.isConnected ? 'Connected' : 'Disconnected';
            const badgeClass = this.isConnected ? 'bg-success' : 'bg-danger';
            statusElement.textContent = status;
            statusElement.className = `badge ${badgeClass}`;
        }
    }

    // Update room list in UI
    updateRoomList(rooms) {
        const roomList = document.getElementById('roomList');
        if (!roomList) {
            console.error("❌ roomList element not found");
            return;
        }

        roomList.innerHTML = '';

        if (rooms.length === 0) {
            const noRoomsMessage = document.createElement('div');
            noRoomsMessage.className = 'text-center text-muted p-3';
            noRoomsMessage.textContent = 'No active chat rooms';
            noRoomsMessage.id = 'noRoomsMessage';
            roomList.appendChild(noRoomsMessage);
            return;
        }

        // Sort rooms by last activity (newest first)
        const sortedRooms = [...rooms].sort((a, b) =>
            new Date(b.lastActivity) - new Date(a.lastActivity)
        );

        sortedRooms.forEach(room => {
            const roomElement = this.createRoomElement(room);
            roomList.appendChild(roomElement);
        });
    }

    // Create room element
    createRoomElement(room) {
        const button = document.createElement('button');
        button.type = 'button';
        button.className = `room-btn list-group-item list-group-item-action d-flex justify-content-between align-items-center ${room.id === this.activeRoomId ? 'active' : ''
            }`;
        button.setAttribute('data-id', room.id);

        const unreadBadge = room.unreadMessages > 0 ?
            `<span class="badge bg-danger rounded-pill">${room.unreadMessages}</span>` : '';

        // Use shorter room identifier for display
        const roomDisplayId = room.userIdentifier ?
            room.userIdentifier.substring(0, 8) + '...' :
            room.id.substring(0, 8) + '...';

        // Format last activity time
        const lastActivity = new Date(room.lastActivity);
        const now = new Date();
        const diffMinutes = Math.floor((now - lastActivity) / (1000 * 60));

        let timeDisplay;
        if (diffMinutes < 1) {
            timeDisplay = 'Just now';
        } else if (diffMinutes < 60) {
            timeDisplay = `${diffMinutes}m ago`;
        } else {
            timeDisplay = lastActivity.toLocaleTimeString('en-US', {
                hour: '2-digit',
                minute: '2-digit'
            });
        }

        button.innerHTML = `
            <div class="room-info text-start flex-grow-1">
                <div class="room-name fw-bold">Room: ${roomDisplayId}</div>
                <small class="text-muted">Last: ${timeDisplay}</small>
            </div>
            ${unreadBadge}
        `;

        // Add click event listener
        button.addEventListener('click', () => {
            console.log(`🔄 Switching to room: ${room.id}`);
            this.switchToRoom(room.id);
        });

        return button;
    }

    // Switch to different room
    async switchToRoom(roomId) {
        if (this.activeRoomId === roomId) {
            console.log("⚠️ Already in this room");
            return;
        }

        // Leave previous room
        if (this.activeRoomId) {
            await this.connection.invoke("LeaveRoom", this.activeRoomId).catch(err =>
                console.error("LeaveRoom error:", err)
            );
        }

        this.activeRoomId = roomId;

        // Join new room
        await this.connection.invoke("JoinRoom", roomId).catch(err => {
            console.error("JoinRoom error:", err);
            alert("Failed to join room. Please try again.");
            return;
        });

        // Update UI
        this.updateActiveRoomUI(roomId);

        // Reset unread count
        this.updateUnreadCount(roomId, false);

        // Show message form
        this.toggleMessageForm(true);

        console.log(`✅ Successfully switched to room: ${roomId}`);
    }

    // Update active room UI
    updateActiveRoomUI(roomId) {
        // Remove active class from all buttons
        document.querySelectorAll('.room-btn').forEach(btn => {
            btn.classList.remove('active');
        });

        // Add active class to selected button
        const activeButton = document.querySelector(`[data-id="${roomId}"]`);
        if (activeButton) {
            activeButton.classList.add('active');
        }

        // Update title
        const titleElement = document.getElementById('activeRoomTitle');
        if (titleElement) {
            const roomDisplayId = roomId.substring(0, 8) + '...';
            titleElement.textContent = `Active Chat Room: ${roomDisplayId}`;
        }
    }

    // Toggle message form visibility
    toggleMessageForm(show) {
        const messageForm = document.getElementById('supportMessageForm');
        const messageInput = document.getElementById('supportMessageInput');
        const sendButton = document.getElementById('sendSupportButton');

        if (messageForm) {
            messageForm.style.display = show ? 'block' : 'none';
        }

        if (messageInput && sendButton) {
            messageInput.disabled = !show;
            sendButton.disabled = !show;

            if (show) {
                messageInput.focus();
            }
        }
    }

    // Update unread message count
    updateUnreadCount(roomId, increment = false) {
        const roomElement = document.querySelector(`[data-id="${roomId}"]`);
        if (roomElement) {
            let badge = roomElement.querySelector('.badge');
            let currentCount = badge ? parseInt(badge.textContent) : 0;

            if (increment) {
                currentCount++;
            } else {
                currentCount = 0;
            }

            if (currentCount > 0) {
                if (!badge) {
                    badge = document.createElement('span');
                    badge.className = 'badge bg-danger rounded-pill';
                    roomElement.appendChild(badge);
                }
                badge.textContent = currentCount;
            } else if (badge) {
                badge.remove();
            }
        }
    }

    // Mark room as read
    markRoomAsRead(roomId) {
        this.updateUnreadCount(roomId, false);
    }

    // Display messages
    displayMessages(messages) {
        const messageList = document.getElementById('roomMessageList');
        if (!messageList) {
            console.error("❌ roomMessageList element not found");
            return;
        }

        messageList.innerHTML = '';

        if (messages.length === 0) {
            messageList.innerHTML = '<div class="text-center text-muted mt-5">No messages in this room yet</div>';
            return;
        }

        // Sort messages by time
        const sortedMessages = [...messages].sort((a, b) =>
            new Date(a.time) - new Date(b.time)
        );

        sortedMessages.forEach(message => {
            const messageElement = this.createMessageElement(message);
            messageList.appendChild(messageElement);
        });

        // Scroll to bottom
        messageList.scrollTop = messageList.scrollHeight;
        console.log(`📋 Displayed ${messages.length} messages in active room`);
    }

    // Display a single message
    displayMessage(message) {
        const messageList = document.getElementById('roomMessageList');
        if (!messageList) {
            console.error("❌ roomMessageList element not found");
            return;
        }

        // Remove "no messages" message if it exists
        if (messageList.children.length === 1 &&
            messageList.children[0].classList.contains('text-center')) {
            messageList.innerHTML = '';
        }

        const messageElement = this.createMessageElement(message);
        messageList.appendChild(messageElement);

        // Scroll to bottom
        messageList.scrollTop = messageList.scrollHeight;
        console.log("📝 Added new message to active room");
    }

    // Create message element
    createMessageElement(message) {
        const div = document.createElement('div');
        div.className = `message ${message.sender === 'User' ? 'user-message' : 'support-message'}`;

        const time = new Date(message.time).toLocaleTimeString('en-US', {
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        });

        div.innerHTML = `
            <div class="message-header d-flex justify-content-between align-items-center">
                <strong class="sender-name">${message.sender}</strong>
                <small class="text-muted message-time">${time}</small>
            </div>
            <div class="message-body mt-1">${this.escapeHtml(message.message)}</div>
        `;

        return div;
    }

    // Escape HTML to prevent XSS
    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Send message as support operator
    async sendSupportMessage(messageText) {
        if (this.connection && this.isConnected && this.activeRoomId) {
            try {
                console.log(`📤 Sending support message to room ${this.activeRoomId}: ${messageText}`);
                await this.connection.invoke("SendMessageToRoom", this.activeRoomId, messageText);
                console.log("✅ Support message sent successfully");

                // Clear input
                const messageInput = document.getElementById('supportMessageInput');
                if (messageInput) {
                    messageInput.value = '';
                }
            } catch (err) {
                console.error("❌ Send support message error:", err);
                alert("Failed to send message. Please check your connection.");
            }
        } else {
            console.warn("⚠️ Not connected or no active room, cannot send message");
            alert("Please select a chat room first.");
        }
    }
}

// Create global instance
const supportService = new SupportService();