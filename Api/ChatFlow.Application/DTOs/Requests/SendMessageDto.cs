namespace ChatFlow.Application.DTOs.Requests
{
    public class SendMessageDto
    {
        public Guid ReceiverId { get; set; }
        public Guid ConversationId { get; set; }
        public required string Content { get; set; }
    }
}
