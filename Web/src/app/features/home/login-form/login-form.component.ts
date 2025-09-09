import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { ErrorModalComponent } from '../../../shared/components/error-modal/error-modal.component';
import { LoginRequest } from '../../../shared/interfaces/login-request.interface';
import { AuthService } from '../../../shared/services/auth.service';

@Component({
  selector: 'app-login-form',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.scss',
})
export class LoginFormComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  readonly dialog = inject(MatDialog);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private authService = inject(AuthService);

  formGroup!: FormGroup;
  ngOnInit(): void {
    this.formGroup = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  submit(form: FormGroup): void {
    if (form.valid) {
      const payload: LoginRequest = {
        email: form.get('email')?.value,
        password: form.get('password')?.value,
      };
      // TODO: add loading spinner
      this.authService
        .login(payload)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (response) => {
            console.log('JWT:', response.token);
            localStorage.setItem('token', response.token);
            this.router.navigate(['/app']);
          },
          error: (errorResponse) => {
            const errorCode = errorResponse.error.ErrorCode;
            let errorMessage = 'Something went wrong';
            if (errorCode === '001') {
              errorMessage = 'Invalid credentials';
            }
            const dialog = this.dialog.open(ErrorModalComponent, {
              width: '250px',
            });
            dialog.componentInstance.label = errorMessage;
          },
          complete: () => {
            // TODO: remove loading spinner
          },
        });
    }
  }
}
