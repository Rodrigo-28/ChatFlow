import { Component, inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { Router } from '@angular/router';
import { CacheService } from '../../services/cache.service';

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
  logout(): void {
    localStorage.removeItem('token');
    this.router.navigate(['/']);
  }
  ngOnInit(): void {
    this.cacheService.getItem<string>('senderName').subscribe({
      next: (name) => {
        this.name = name;
      },
    });
  }

  name: string | null = null;
}
