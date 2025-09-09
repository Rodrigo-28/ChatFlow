namespace ChatFlow.Application.DTOs.Responses
{
    public class ConversationResponseDto
    {
        public Guid Id { get; set; }
        public IEnumerable<SendMessageResponseDto> Messages { get; set; }
    }
}
