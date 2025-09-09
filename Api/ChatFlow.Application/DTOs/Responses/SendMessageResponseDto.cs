namespace ChatFlow.Application.DTOs.Responses
{
    public class SendMessageResponseDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid SenderId { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
