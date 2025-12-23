import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { CurrentUser } from '../interfaces/current-user.interface';
import { LoginRequest } from '../interfaces/login-request.interface';
import { LoginResponse } from '../interfaces/login-response.interface';
import { RefreshRequest } from '../interfaces/refresh-request.interface';
import { RefreshResponse } from '../interfaces/refresh-response.interfaces';
import { RegisterRequestAuth } from '../interfaces/RegisterRequestUser';
import { RegisterResponseAuth } from '../interfaces/RegisterResponseAuth';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private httpClient: HttpClient = inject(HttpClient);
  private baseUrl = environment.baseUrl;
  private tokenService = inject(TokenService);
  login(body: LoginRequest): Observable<LoginResponse> {
    return this.httpClient.post<LoginResponse>(
      `${this.baseUrl}/auth/login`,
      body,
    );
  }
  getMe(): Observable<CurrentUser> {
    return this.httpClient.get<CurrentUser>(`${this.baseUrl}/Auth/me`);
  }
  register(body: RegisterRequestAuth): Observable<RegisterResponseAuth> {
    return this.httpClient.post<RegisterResponseAuth>(
      `${this.baseUrl}/auth/register`,
      body,
    );
  }
  refresh(): Observable<RefreshResponse> {
    const body: RefreshRequest = {
      refreshToken: this.tokenService.refreshToken!, 
    };
    return this.httpClient.post<RefreshResponse>(
      `${this.baseUrl}/Auth/refresh`,
      body,
    );
  }
  logout() {
    return this.httpClient.post<void>(`${this.baseUrl}/Auth/logout`, {
      refreshToken: this.tokenService.refreshToken,
    });
  }
  logoutAll() {
    return this.httpClient.post<void>(`${this.baseUrl}/Auth/logout-all`, {
      refreshToken: this.tokenService.refreshToken,
    });
  }
}
