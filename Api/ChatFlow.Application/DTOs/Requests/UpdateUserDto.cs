namespace ChatFlow.Application.DTOs.Requests
{
    public class UpdateUserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}
