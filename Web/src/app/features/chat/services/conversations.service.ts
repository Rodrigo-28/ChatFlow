import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { ConversationResponse } from '../interfaces/conversation-response.interface';

@Injectable({
  providedIn: 'root',
})
export class ConversationsService {
  endpoint = '/conversations';
  private httpClient = inject(HttpClient);

  getConversation(userId: string) {
    return this.httpClient.get<ConversationResponse>(
      `${environment.baseUrl}${this.endpoint}/${userId}`,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
        },
      },
    );
  }
}
