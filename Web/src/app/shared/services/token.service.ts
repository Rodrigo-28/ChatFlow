import { Injectable } from '@angular/core';

const ACCESS_KEY  = 'token';          // tu JWT
const REFRESH_KEY = 'refreshToken';   // tu refresh

@Injectable({ providedIn: 'root' })
export class TokenService {
  // ACCESS TOKEN
  get accessToken(): string | null {
    return localStorage.getItem(ACCESS_KEY);
  }
  set accessToken(value: string | null) {
    if (value) localStorage.setItem(ACCESS_KEY, value);
    else localStorage.removeItem(ACCESS_KEY);
  }

  // REFRESH TOKEN
  get refreshToken(): string | null {
    return localStorage.getItem(REFRESH_KEY);
  }
  set refreshToken(value: string | null) {
    if (value) localStorage.setItem(REFRESH_KEY, value);
    else localStorage.removeItem(REFRESH_KEY);
  }

  clear() {
    localStorage.removeItem(ACCESS_KEY);
    localStorage.removeItem(REFRESH_KEY);
  }
}
