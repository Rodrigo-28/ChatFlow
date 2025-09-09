import { Component } from '@angular/core';
import {
  MatButtonToggleChange,
  MatButtonToggleModule,
} from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { LoginFormComponent } from '../../features/home/login-form/login-form.component';
import { RegisterFormComponent } from '../../features/home/register-form/register-form.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    MatCardModule,
    LoginFormComponent,
    MatButtonToggleModule,
    RegisterFormComponent,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  tab: 'login' | 'register' = 'login';

  changeTab(event: MatButtonToggleChange) {
    this.tab = event.value;
  }
}
