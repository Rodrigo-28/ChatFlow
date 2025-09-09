import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { CurrentUser } from '../interfaces/current-user.interface';
import { LoginRequest } from '../interfaces/login-request.interface';
import { LoginResponse } from '../interfaces/login-response.interface';
import { RegisterRequestAuth } from '../interfaces/RegisterRequestUser';
import { RegisterResponseAuth } from '../interfaces/RegisterResponseAuth';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private httpClient: HttpClient = inject(HttpClient);
  private baseUrl = environment.baseUrl;
  login(body: LoginRequest): Observable<LoginResponse> {
    return this.httpClient.post<LoginResponse>(
      `${this.baseUrl}/auth/login`,
      body,
    );
  }
  getMe(): Observable<CurrentUser> {
    return this.httpClient.get<CurrentUser>(`${this.baseUrl}/auth/me`, {
      headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
    });
  }
  register(body: RegisterRequestAuth): Observable<RegisterResponseAuth> {
    return this.httpClient.post<RegisterResponseAuth>(
      `${this.baseUrl}/auth/register`,
      body,
    );
  }
}
