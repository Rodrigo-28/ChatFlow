import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EMPTY, switchMap } from 'rxjs';
import { ConversationComponent } from '../../features/chat/components/conversation/conversation.component';
import { ConversationResponse } from '../../features/chat/interfaces/conversation-response.interface';
import { MessageWebsocket } from '../../features/chat/interfaces/message-websocket.interface';
import { Message } from '../../features/chat/interfaces/message.interface';
import { SendMessage } from '../../features/chat/interfaces/send-message.interface';
import { ConversationsService } from '../../features/chat/services/conversations.service';
import { MessagesService } from '../../features/chat/services/messages.service';
import { CacheService } from '../../shared/services/cache.service';
import { WebSocketService } from '../../shared/services/websocket.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [ConversationComponent],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss',
})
export class ChatComponent implements OnInit {
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly conversationService = inject(ConversationsService);
  private readonly messagesService = inject(MessagesService);
  private readonly cacheService = inject(CacheService);
  private readonly websocketService = inject(WebSocketService);
  body: ConversationResponse | null = null;
  userId: string | null = null;
  ngOnInit(): void {
    this.userId = this.activatedRoute.snapshot.paramMap.get('id');
    this.getConversation(this.userId || '');
    this.getCacheService();
  }
  getCacheService() {
    this.cacheService
      .getItem<string>('senderId')
      .pipe(
        switchMap((senderId) => {
          if (!senderId) return EMPTY;
          return this.websocketService.connect(senderId);
        }),
      )
      .subscribe({
        next: (payload: string) => {
          const message: MessageWebsocket = JSON.parse(payload);
          if (
            this.body &&
            message.ConversationId === this.body?.conversation.id
          ) {
            const newMessage: Message = {
              id: new Date().getTime().toString(), // Assign appropriate value
              senderId: new Date().getTime().toString(),
              content: message.Message,
              sentAt: new Date(message.SendAt).toString(),
              isRead: true,
            };

            this.body = {
              ...this.body,
              conversation: {
                ...this.body.conversation,
                messages: [
                  ...(this.body?.conversation.messages || []),
                  newMessage,
                ],
              },
            };
          }
        },
      });
  }
  getConversation(userId: string) {
    this.conversationService.getConversation(userId).subscribe({
      next: (conversation) => {
        this.body = conversation;
      },
      error: (error) => {
        console.error('Error fetching conversation:', error);
      },
      complete: () => {},
    });
  }
  createMessage(payload: SendMessage) {
    this.messagesService.sendMessage(payload).subscribe({
      next: (message) => {
        this.getConversation(this.userId || '');
      },
      error: (error) => {
        console.error('Error sending message:', error);
      },
      complete: () => {},
    });
  }
}
