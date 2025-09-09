// src/app/shared/validators/match-passwords.validator.ts
import { AbstractControl, ValidationErrors } from '@angular/forms';

export function matchPasswordsValidator(
  group: AbstractControl,
): ValidationErrors | null {
  const password = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  if (!password || !confirm) return null; // aún no completados

  return password === confirm ? null : { passwordsNotMatch: true };
}
