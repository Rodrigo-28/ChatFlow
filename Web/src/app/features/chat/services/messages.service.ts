import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { Message } from '../interfaces/message.interface';
import { SendMessage } from '../interfaces/send-message.interface';

@Injectable({
  providedIn: 'root',
})
export class MessagesService {
  private endpoint = '/messages';
  private httpClient = inject(HttpClient);
  constructor() {}

  sendMessage(payload: SendMessage) {
    return this.httpClient.post<Message>(
      `${environment.baseUrl}${this.endpoint}/send`,
      payload,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
        },
      },
    );
  }
}
