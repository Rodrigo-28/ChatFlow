import { Component, inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CacheService } from '../../services/cache.service';
import { TokenService } from '../../services/token.service';

@Component({
  selector: 'app-user-card',
  standalone: true,
  imports: [MatCardModule, MatButtonModule],
  templateUrl: './user-card.component.html',
  styleUrl: './user-card.component.scss',
})
export class UserCardComponent implements OnInit {
  private router = inject(Router);
  private readonly cacheService = inject(CacheService);
  private auth = inject(AuthService);
  private tokens = inject(TokenService);
  name: string | null = null;
  loading = false;

  ngOnInit(): void {
    this.cacheService.getItem<string>('senderName').subscribe({
      next: (name) => (this.name = name),
    });
  }
  logout(): void {
    if (this.loading) return;
    this.loading = true;

    this.auth.logout().subscribe({
      next: () => this.finishLogout(),
      error: () => this.finishLogout(), // aunque falle, limpiamos y navegamos
    });
  }

  // opcional: para “cerrar en todos los dispositivos”
  logoutAll(): void {
    if (this.loading) return;
    this.loading = true;

    this.auth.logoutAll().subscribe({
      next: () => this.finishLogout(),
      error: () => this.finishLogout(),
    });
  }
  private finishLogout() {
    // limpiar front SIEMPRE
    this.tokens.clear();
    this.cacheService.clear(); // borra 'senderName' y cualquier cosa cacheada
    this.loading = false;
    this.router.navigate(['/']);
  }
}
