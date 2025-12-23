import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, filter, finalize, switchMap, take } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { TokenService } from '../services/token.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private tokens = inject(TokenService);
  private auth = inject(AuthService);
  private router = inject(Router);

  //variables de control
  private isRefreshing = false;
  private refreshSubject = new BehaviorSubject<string | null>(null);

  //Punto de entrada
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    const withAuth = this.addAuthHeader(req, this.tokens.accessToken);

    return next.handle(withAuth).pipe(
      catchError((err: HttpErrorResponse) => {
        // Si es 401 y NO es un endpoint de auth, intentamos refrescar
        if (err.status === 401 && !this.isAuthUrl(req.url)) {
          return this.handle401(withAuth, next);
        }
        return throwError(() => err);
      }),
    );
  }

  //Agregar el token a cada request
  private addAuthHeader(req: HttpRequest<any>, token: string | null) {
    if (!token) return req;
    return req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }

  private isAuthUrl(url: string) {
    // evitamos fallas sobre /Auth/login|register|refresh
    return /\/Auth\/(login|register|refresh|logout|logout-all)/i.test(url);
  }

  private handle401(
    req: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    //caso a no hay refresh en curso
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshSubject.next(null);

      return this.auth.refresh().pipe(
        switchMap((res) => {
          // guardar SIEMPRE el nuevo access token
          this.tokens.accessToken = res.token;
          // si tu backend rota el refresh, guardarlo también
          if (res.refreshToken) this.tokens.refreshToken = res.refreshToken;

          this.refreshSubject.next(res.token);
          // reintentar la request original con el token nuevo
          const retried = this.addAuthHeader(req, res.token);
          return next.handle(retried);
        }),
        catchError((err) => {
          // si el refresh falla, limpiamos y llevamos a login
          this.tokens.clear();
          this.router.navigateByUrl('/');
          return throwError(() => err);
        }),
        finalize(() => (this.isRefreshing = false)),
      );
    } else {
      // ya hay un refresh en curso: esperamos a que termine
      return this.refreshSubject.pipe(
        filter((t) => t !== null),
        take(1),
        switchMap(() =>
          next.handle(this.addAuthHeader(req, this.tokens.accessToken)),
        ),
      );
    }
  }
}
