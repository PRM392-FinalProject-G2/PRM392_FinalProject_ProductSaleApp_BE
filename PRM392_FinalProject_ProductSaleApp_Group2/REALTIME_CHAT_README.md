# Real-Time Chat Feature Documentation

## Tổng quan
Tính năng Real-Time Chat cho phép người dùng chat trực tiếp với đại diện cửa hàng hoặc người dùng khác trong thời gian thực sử dụng SignalR.

## Công nghệ sử dụng
- **SignalR**: WebSocket-based real-time communication
- **ASP.NET Core**: Backend framework
- **Entity Framework Core**: Database ORM
- **Memory Cache**: Lưu trữ trạng thái online/offline

## Kiến trúc

### 1. SignalR Hub (`ChatHub.cs`)
Hub chính để xử lý các kết nối real-time và events:

**Endpoints:**
- `RegisterUser(userId)` - Đăng ký user khi connect
- `SendMessage(senderId, receiverId, message)` - Gửi tin nhắn
- `SendTypingIndicator(senderId, receiverId, isTyping)` - Hiển thị "đang gõ..."
- `MarkMessagesAsRead(userId, otherUserId)` - Đánh dấu đã đọc
- `GetOnlineUsers()` - Lấy danh sách users online
- `GetChatHistory(userId, otherUserId, pageNumber, pageSize)` - Lấy lịch sử chat

**Events (Client nhận):**
- `UserRegistered` - Xác nhận đăng ký thành công
- `ReceiveMessage` - Nhận tin nhắn mới
- `MessageSent` - Xác nhận tin nhắn đã gửi
- `UserTyping` - User đang gõ tin nhắn
- `MessagesRead` - Tin nhắn đã được đọc
- `OnlineUsers` - Danh sách users online
- `UserStatusChanged` - Trạng thái user thay đổi (online/offline)
- `ChatHistory` - Lịch sử chat
- `Error` - Thông báo lỗi

### 2. REST API Controller (`ChatRealtimeController.cs`)
Cung cấp REST API fallback và các chức năng bổ sung:

**Endpoints:**
- `GET /api/chatRealtime/history` - Lấy lịch sử chat
- `GET /api/chatRealtime/conversations/{userId}` - Lấy danh sách conversations
- `POST /api/chatRealtime/send` - Gửi tin nhắn (REST fallback)
- `GET /api/chatRealtime/connection-info` - Thông tin kết nối SignalR
- `GET /api/chatRealtime/test` - Test endpoint

### 3. Services
- `IChatRealtimeService` - Service xử lý logic chat
- `IChatMessageService` - Service quản lý messages trong database

## Cách sử dụng

### Backend Setup

1. **SignalR đã được cấu hình trong Program.cs:**
```csharp
builder.Services.AddSignalR();
app.MapHub<ChatHub>("/chatHub");
```

2. **CORS đã được cấu hình để cho phép frontend kết nối**

### Frontend Integration

#### 1. Cài đặt SignalR Client
```bash
npm install @microsoft/signalr
```

#### 2. Kết nối đến Hub
```javascript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/chatHub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .withAutomaticReconnect()
    .build();

// Kết nối
await connection.start();
console.log("Connected to ChatHub");

// Đăng ký user
await connection.invoke("RegisterUser", userId);
```

#### 3. Gửi tin nhắn
```javascript
await connection.invoke("SendMessage", senderId, receiverId, messageText);
```

#### 4. Nhận tin nhắn
```javascript
connection.on("ReceiveMessage", (message) => {
    console.log("New message:", message);
    // message = { ChatMessageId, SenderId, ReceiverId, Message, SentAt, SenderName, SenderAvatar }
    // Hiển thị tin nhắn trong UI
});
```

#### 5. Hiển thị "đang gõ..."
```javascript
// Gửi typing indicator
await connection.invoke("SendTypingIndicator", senderId, receiverId, true);

// Nhận typing indicator
connection.on("UserTyping", (userId, isTyping) => {
    if (isTyping) {
        console.log(`User ${userId} is typing...`);
    }
});
```

#### 6. Xác nhận tin nhắn đã gửi
```javascript
connection.on("MessageSent", (message) => {
    console.log("Message sent successfully:", message);
    // Cập nhật UI
});
```

#### 7. Lấy danh sách users online
```javascript
await connection.invoke("GetOnlineUsers");

connection.on("OnlineUsers", (userIds) => {
    console.log("Online users:", userIds);
});
```

#### 8. Theo dõi trạng thái user
```javascript
connection.on("UserStatusChanged", (userId, isOnline) => {
    console.log(`User ${userId} is now ${isOnline ? 'online' : 'offline'}`);
});
```

#### 9. Lấy lịch sử chat
```javascript
await connection.invoke("GetChatHistory", userId, otherUserId, 1, 50);

connection.on("ChatHistory", (messages) => {
    console.log("Chat history:", messages);
    // Hiển thị lịch sử trong UI
});
```

#### 10. Xử lý lỗi
```javascript
connection.on("Error", (errorMessage) => {
    console.error("Chat error:", errorMessage);
});
```

### REST API Usage (Fallback)

#### Lấy lịch sử chat
```http
GET /api/chatRealtime/history?userId=1&otherUserId=2&pageNumber=1&pageSize=50
```

#### Lấy danh sách conversations
```http
GET /api/chatRealtime/conversations/1
```

#### Gửi tin nhắn (không realtime)
```http
POST /api/chatRealtime/send
Content-Type: application/json

{
  "senderId": 1,
  "receiverId": 2,
  "message": "Hello!"
}
```

## Example Frontend Component (React)

```javascript
import React, { useState, useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';

const ChatComponent = ({ currentUserId, chatWithUserId }) => {
    const [messages, setMessages] = useState([]);
    const [newMessage, setNewMessage] = useState('');
    const [connection, setConnection] = useState(null);
    const [isTyping, setIsTyping] = useState(false);

    useEffect(() => {
        // Tạo connection
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5000/chatHub")
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => {
                    console.log('Connected!');
                    
                    // Đăng ký user
                    connection.invoke("RegisterUser", currentUserId);

                    // Lắng nghe tin nhắn mới
                    connection.on("ReceiveMessage", (message) => {
                        setMessages(prev => [...prev, message]);
                    });

                    // Lắng nghe typing indicator
                    connection.on("UserTyping", (userId, isTyping) => {
                        if (userId === chatWithUserId) {
                            setIsTyping(isTyping);
                        }
                    });

                    // Lấy lịch sử chat
                    connection.invoke("GetChatHistory", currentUserId, chatWithUserId, 1, 50);
                    
                    connection.on("ChatHistory", (history) => {
                        setMessages(history.items || []);
                    });
                })
                .catch(err => console.error('Connection failed: ', err));
        }
    }, [connection]);

    const sendMessage = async () => {
        if (newMessage.trim() && connection) {
            await connection.invoke("SendMessage", currentUserId, chatWithUserId, newMessage);
            setNewMessage('');
        }
    };

    const handleTyping = () => {
        if (connection) {
            connection.invoke("SendTypingIndicator", currentUserId, chatWithUserId, true);
            
            // Stop typing sau 2 giây
            setTimeout(() => {
                connection.invoke("SendTypingIndicator", currentUserId, chatWithUserId, false);
            }, 2000);
        }
    };

    return (
        <div className="chat-container">
            <div className="messages">
                {messages.map(msg => (
                    <div key={msg.chatMessageId} className={msg.senderId === currentUserId ? 'sent' : 'received'}>
                        <p>{msg.message}</p>
                        <small>{new Date(msg.sentAt).toLocaleTimeString()}</small>
                    </div>
                ))}
                {isTyping && <div className="typing-indicator">User is typing...</div>}
            </div>
            
            <div className="input-area">
                <input 
                    type="text" 
                    value={newMessage}
                    onChange={(e) => {
                        setNewMessage(e.target.value);
                        handleTyping();
                    }}
                    onKeyPress={(e) => e.key === 'Enter' && sendMessage()}
                    placeholder="Type a message..."
                />
                <button onClick={sendMessage}>Send</button>
            </div>
        </div>
    );
};

export default ChatComponent;
```

## Database Schema

### ChatMessage Table
```sql
CREATE TABLE Chatmessage (
    Chatmessageid SERIAL PRIMARY KEY,
    Senderid INTEGER NOT NULL,
    Receiverid INTEGER NOT NULL,
    Message TEXT NOT NULL,
    Sentat TIMESTAMP NOT NULL,
    FOREIGN KEY (Senderid) REFERENCES Users(Userid),
    FOREIGN KEY (Receiverid) REFERENCES Users(Userid)
);
```

### Indexes (Recommended)
```sql
CREATE INDEX idx_chat_sender_receiver ON Chatmessage(Senderid, Receiverid);
CREATE INDEX idx_chat_receiver_sender ON Chatmessage(Receiverid, Senderid);
CREATE INDEX idx_chat_sentat ON Chatmessage(Sentat DESC);
```

## Testing

### Test SignalR Connection
```bash
GET http://localhost:5000/api/chatRealtime/connection-info
```

### Test Chat Service
```bash
GET http://localhost:5000/api/chatRealtime/test
```

## Security Considerations

1. **Authentication**: Nên thêm JWT authentication cho SignalR Hub
2. **Authorization**: Kiểm tra quyền truy cập tin nhắn
3. **Rate Limiting**: Giới hạn số tin nhắn gửi trong khoảng thời gian
4. **Input Validation**: Validate và sanitize tin nhắn
5. **CORS**: Chỉ cho phép origins đáng tin cậy

## Future Enhancements

1. ✅ **Typing Indicator** - Đã implement
2. ✅ **Online/Offline Status** - Đã implement
3. ⏳ **Read Receipts** - Cần thêm field IsRead vào database
4. ⏳ **Message Reactions** - Emoji reactions
5. ⏳ **File Sharing** - Gửi hình ảnh, file
6. ⏳ **Group Chat** - Chat nhóm
7. ⏳ **Message Search** - Tìm kiếm tin nhắn
8. ⏳ **Push Notifications** - Thông báo khi có tin nhắn mới
9. ⏳ **Message Encryption** - End-to-end encryption
10. ⏳ **Voice/Video Call** - Cuộc gọi thoại/video

## Troubleshooting

### Connection Issues
- Kiểm tra CORS settings
- Kiểm tra URL của Hub
- Kiểm tra firewall/network

### Messages Not Received
- Kiểm tra user đã RegisterUser chưa
- Kiểm tra receiverId có đúng không
- Kiểm tra connection status

### Performance Issues
- Thêm pagination cho chat history
- Implement message caching
- Optimize database queries với indexes

## API Reference

Xem chi tiết tại:
```
GET /api/chatRealtime/connection-info
```

## Support
Liên hệ team để được hỗ trợ.
