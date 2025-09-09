import { Conversation } from './conversation.interface';

export interface ConversationResponse {
  receiverName: string;
  conversation: Conversation;
}
