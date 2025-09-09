import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { User } from '../interfaces/user.interface';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  endpoint = '/Users';
  private httpClient = inject(HttpClient);

  getUsers() {
    return this.httpClient.get<Array<User>>(
      `${environment.baseUrl}${this.endpoint}`,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
        },
      },
    );
  }
}
