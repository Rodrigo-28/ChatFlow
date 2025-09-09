import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class WebSocketService {
  private ws: WebSocket | null = null;

  public connect(senderId: string): Observable<any> {
    return new Observable((observer) => {
      if (!senderId) {
        throw new Error('No senderId found in cache');
      }
      // Build ws url
      // HTTPS → WSS
      const wsUrl = `wss://localhost:7105/ws?userId=${senderId}`;

      this.ws = new WebSocket(wsUrl);
      this.ws.onopen = () => {};
      this.ws.onerror = (err) => {
        observer.error(event);
      };
      this.ws.onclose = () => {
        observer.complete();
      };
      this.ws.onmessage = (event) => {
        observer.next(event.data);
      };
    });
  }

  disconnect(): void {
    if (this.ws) {
      this.ws.close();
      this.ws = null;
    }
  }
}
