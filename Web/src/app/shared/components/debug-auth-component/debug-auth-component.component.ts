import { JsonPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { TokenService } from '../../services/token.service';

@Component({
  selector: 'app-debug-auth-component',
  standalone: true,
  imports: [JsonPipe],
  templateUrl: './debug-auth-component.component.html',
  styleUrl: './debug-auth-component.component.scss',
})
export class DebugAuthComponentComponent {
  private auth = inject(AuthService);
  private tokens = inject(TokenService);

  lastMe: unknown = null;
  onGetMe() {
    console.log(
      '➡️ GET /Auth/me con token:',
      (this.tokens.accessToken ?? '').slice(0, 24),
      '...',
    );
    this.auth.getMe().subscribe({
      next: (me) => {
        this.lastMe = me;
        console.log('✅ /Auth/me OK', me);
        console.log(
          'Token actual (inicio):',
          (this.tokens.accessToken ?? '').slice(0, 24),
          '...',
        );
      },
      error: (e) => {
        console.error('❌ /Auth/me error', e);
      },
    });
  }

  onBreakToken() {
    // simula token inválido → el back responderá 401 y el interceptor intentará refresh
    this.tokens.accessToken =
      'invalid.' + Math.random().toString(36).slice(2) + '.token';
    console.log('⚠️ Token roto. Ahora llamá a GetMe para disparar el refresh.');
  }

  onRemoveRefresh() {
    // simula que no hay refresh disponible → el refresh va a fallar y te debe sacar a '/'
    this.tokens.refreshToken = null;
    console.log('⚠️ refreshToken quitado. El próximo 401 no podrá refrescar.');
  }
}
