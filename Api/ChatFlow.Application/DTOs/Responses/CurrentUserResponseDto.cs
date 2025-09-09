namespace ChatFlow.Application.DTOs.Responses;

public class CurrentUserResponseDto
{
    public Guid senderId { get; set; }
    public string senderName { get; set; }
}