import { Message } from './message.interface';

export interface Conversation {
  id: string;
  messages: Message[];
}
