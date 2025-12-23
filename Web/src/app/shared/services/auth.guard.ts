import { inject, Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { TokenService } from './token.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  private router = inject(Router);
  private tokens = inject(TokenService);

  canActivate(): boolean | UrlTree {
    return this.tokens.accessToken ? true : this.router.createUrlTree(['/']);
  }
}
