using AutoMapper;
using ChatFlow.Application.DTOs.Requests;
using ChatFlow.Application.DTOs.Responses;
using ChatFlow.Application.Exceptions;
using ChatFlow.Application.Interfaces;
using ChatFlow.Domain.Interfaces;
using ChatFlow.Domain.Models;

namespace ChatFlow.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly WebSocketHandler _webSocketHandler;

        public MessageService(IMessageRepository messageRepository, IUserService userService, IMapper mapper, WebSocketHandler webSocketHandler)
        {
            _messageRepository = messageRepository;
            _userService = userService;
            _mapper = mapper;
            _webSocketHandler = webSocketHandler;
        }


        public async Task<SendMessageResponseDto> SendMessage(SendMessageDto body, Guid senderId)
        {
            // 1. Verificar que exista el usuario receptor
            var receiver = await _userService.GetOne(body.ReceiverId);

            var sender = await _userService.GetOne(senderId);

            if (receiver == null)
            {
                throw new BadRequestException("User not found")
                {
                    ErrorCode = "003"
                };
            }



            // 3. Crear el mensaje
            Message newMessage = new Message
            {
                ConversationId = body.ConversationId,
                SenderId = senderId,
                Content = body.Content,
            };
            var message = await _messageRepository.Create(newMessage);

            // 4. Emitir mensaje
            await _webSocketHandler.SendMessageToUser(body.ReceiverId, body.ConversationId, sender.Username, body.Content);


            return _mapper.Map<SendMessageResponseDto>(message);
        }

        public async Task MarkAsRead(Guid conversationId, Guid senderId)
        {
            await _messageRepository.MarkAsRead(conversationId, senderId);
        }
    }
}
