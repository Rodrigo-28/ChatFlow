using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace ChatFlow.Application.Services
{
    public class WebSocketHandler
    {
        private static readonly Dictionary<Guid, WebSocket> _connections = new Dictionary<Guid, WebSocket>();

        public async Task HandleWebSocketAsync(HttpContext context)
        {
            var userId = Guid.Parse(context.Request.Query["userId"]);
            using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
            {
                _connections[userId] = webSocket;
                await ReceiveMessageAsync(webSocket, userId);
            };
        }

        // This method listens for messages from the client over the WebSocket connection
        private async Task ReceiveMessageAsync(WebSocket webSocket, Guid userId)
        {
            // Buffer to store the received data, with a size of 4 KB
            var buffer = new byte[1024 * 4];

            // Asynchronously wait for a message from the client
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            // Keep listening while the WebSocket connection remains open
            while (!result.CloseStatus.HasValue)
            {
                // Convert the received byte array into a string (the message content)
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                // You can handle the message here (e.g., process it, echo it back to the client, etc.)

                // Continue listening for the next message
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            // Remove the WebSocket connection from the dictionary when the client closes the connection
            _connections.Remove(userId);

            // Close the WebSocket connection properly
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        // This method allows the server to send a message to a specific user by their userId
        public async Task SendMessageToUser(Guid receiverId, Guid conversationId, string senderName, string messageContent)
        {
            // Check if there is an active WebSocket connection for the receiverId
            if (_connections.ContainsKey(receiverId))
            {
                var webSocket = _connections[receiverId];

                // Ensure that the WebSocket connection is still open
                if (webSocket.State == WebSocketState.Open)
                {
                    var messageToSend = new
                    {
                        ConversationId = conversationId,
                        SenderName = senderName,
                        Message = messageContent,
                        SendAt = DateTime.UtcNow
                    };
                    // Serialize the message object to JSON
                    var jsonMessage = JsonConvert.SerializeObject(messageToSend);
                    // Convert the message content into a byte array
                    var messageBytes = Encoding.UTF8.GetBytes(jsonMessage);

                    // Send the message to the client over the WebSocket connection
                    await webSocket.SendAsync(new ArraySegment<byte>(messageBytes, 0, messageBytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
