import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { User } from '../../interfaces/user.interface';
import { UsersService } from '../../services/users.service';
@Component({
  selector: 'app-contacts',
  standalone: true,
  imports: [MatListModule, MatIconModule],
  templateUrl: './contacts.component.html',
  styleUrl: './contacts.component.scss',
})
export class ContactsComponent implements OnInit, OnDestroy {
  private router = inject(Router);
  private usersService = inject(UsersService);
  private subscriptions = new Subscription();
  users: User[] = [];
  ngOnInit(): void {
    this.loadUsers();
  }
  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
  loadUsers() {
    this.subscriptions.add(
      this.usersService.getUsers().subscribe({
        next: (users) => {
          this.users = users;
        },
        error: (error) => {
          console.error('Error loading users:', error);
        },
        complete: () => {
          console.log('Users loaded successfully');
        },
      }),
    );
  }
  openChat(id: string) {
    this.router.navigate([`/app/chat/${id}`]);
  }
}
