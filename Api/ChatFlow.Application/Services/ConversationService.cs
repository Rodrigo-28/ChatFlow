using AutoMapper;
using ChatFlow.Application.DTOs.Responses;
using ChatFlow.Application.Interfaces;
using ChatFlow.Domain.Interfaces;
using ChatFlow.Domain.Models;

namespace ChatFlow.Application.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public ConversationService(IConversationRepository conversationRepository, IMessageService messageService, IMapper mapper, IUserService usersService)
        {
            _conversationRepository = conversationRepository;
            _messageService = messageService;
            _mapper = mapper;
            _userService = usersService;
        }
        public async Task<ConversationResultDto> GetOneOrCreate(Guid receiverId, Guid senderId)
        {
            var conversation = await _conversationRepository.GetOne(receiverId, senderId);

            if (conversation != null)
            {
                await _messageService.MarkAsRead(conversation.Id, senderId);
            }
            else
            {
                var newConversation = new Conversation
                {
                    User1Id = senderId,
                    User2Id = receiverId,
                };
                conversation = await _conversationRepository.Create(newConversation);
            }

            var receiver = await _userService.GetOne(receiverId);
            var conversationResponse = _mapper.Map<ConversationResponseDto>(conversation);
            return new ConversationResultDto
            {
                ReceiverName = receiver.Username,
                Conversation = conversationResponse
            };
        }
    }
}
