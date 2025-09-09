import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { Router } from '@angular/router';
import { concatMap, Subject, tap } from 'rxjs';
import { ErrorModalComponent } from '../../../shared/components/error-modal/error-modal.component';
import { AuthService } from '../../../shared/services/auth.service';
import { CacheService } from '../../../shared/services/cache.service';
import { matchPasswordsValidator } from '../../../shared/validators/match-passwords.validator';

@Component({
  selector: 'app-register-form',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatDialogModule,
  ],
  templateUrl: './register-form.component.html',
  styleUrl: './register-form.component.scss',
})
export class RegisterFormComponent implements OnInit, OnDestroy {
  roles = [
    { id: 1, label: 'Admin' },
    { id: 2, label: 'User' },
  ];
  readonly dialog = inject(MatDialog);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private auth = inject(AuthService);
  private cache = inject(CacheService);

  private destroy$ = new Subject<void>();
  formGroup!: FormGroup;

  ngOnInit(): void {
    this.formGroup = this.fb.group(
      {
        email: new FormControl('', [Validators.required, Validators.email]),
        username: new FormControl('', [Validators.required]),
        password: new FormControl('', [Validators.required]),
        confirmPassword: new FormControl('', [Validators.required]),
        roleId: new FormControl(2, [Validators.required]), // default User = 2
      },
      { validators: matchPasswordsValidator },
    );
  }

  submit(form: FormGroup): void {
    if (!form.valid) return;
    if (form.hasError('passwordsNotMatch')) {
      console.error('Passwords do not match');
      return;
    }

    const { email, username, password } = form.getRawValue() as {
      email: string;
      username: string;
      password: string;
      // confirmPassword y roleId existen en el form, pero NO se envían al Auth/register
    };
    // Flujo: register → login → getMe → navegar
    this.auth
      .register({ email, username, password })
      .pipe(
        concatMap(() => this.auth.login({ email, password })),
        tap((loginRes) => localStorage.setItem('token', loginRes.token)),
      )
      .subscribe({
        next: () => this.router.navigate(['/app']),
        error: (err) => {
          console.error('Register/Login error:', err?.error || err);
          const ref = this.dialog.open(ErrorModalComponent, { width: '320px' });
          ref.componentInstance.label = 'No se pudo completar el registro';
        },
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
