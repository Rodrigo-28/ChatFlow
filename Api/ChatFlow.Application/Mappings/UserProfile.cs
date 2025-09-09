using ChatFlow.Application.DTOs.Requests;
using ChatFlow.Application.DTOs.Responses;
using ChatFlow.Domain.Models;
using Profile = AutoMapper.Profile;

namespace ChatFlow.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Responses
            CreateMap<User, UserResponseDto>();
            CreateMap<Conversation, ConversationResponseDto>();
            CreateMap<Message, SendMessageResponseDto>();

            // Request
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();
        }
    }
}
