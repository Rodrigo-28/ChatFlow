import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { ContactsComponent } from '../../../features/chat/components/contacts/contacts.component';
import { AuthService } from '../../services/auth.service';
import { CacheService } from '../../services/cache.service';
import { UserCardComponent } from '../user-card/user-card.component';
@Component({
  selector: 'app-private-layout',
  standalone: true,
  imports: [
    RouterModule,
    MatSidenavModule,
    MatButtonModule,
    ContactsComponent,
    UserCardComponent,
  ],
  templateUrl: './private-layout.component.html',
  styleUrl: './private-layout.component.scss',
})
export class PrivateLayoutComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private readonly authService = inject(AuthService);
  private readonly cacheService = inject(CacheService);
  // private readonly websocketService = inject(WebSocketService);
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  ngOnInit(): void {
    this.authService
      .getMe()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (user) => {
          this.cacheService.setItem('senderId', user.senderId);
          this.cacheService.setItem('senderName', user.senderName);
        },
      });
  }
}
