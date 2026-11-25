import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthApi, GameSessionApi } from '../core/api';
import { AuthStore } from '../core/auth.store';

@Component({
  selector: 'ceq-login-page',
  imports: [ReactiveFormsModule],
  templateUrl: './login.page.html'
})
export class LoginPage {
  private readonly api = inject(AuthApi);
  private readonly store = inject(AuthStore);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly sessions = inject(GameSessionApi);
  protected readonly guestMode = signal(this.route.snapshot.queryParamMap.get('mode') === 'guest');
  protected readonly registerMode = signal(false);
  protected readonly busy = signal(false);
  protected readonly message = signal('');
  protected readonly loginForm = new FormGroup({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(10)] })
  });
  protected readonly registerForm = new FormGroup({
    displayName: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(80)] }),
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(10)] })
  });
  protected readonly guestForm = new FormGroup({
    joinCode: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.pattern(/^\\d{6}$/)] }),
    displayName: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(80)] })
  });
  protected get activeForm(): FormGroup {
    return this.guestMode() ? this.guestForm : (this.registerMode() ? this.registerForm : this.loginForm);
  }

  protected selectMode(mode: 'login' | 'register' | 'guest'): void {
    this.guestMode.set(mode === 'guest');
    this.registerMode.set(mode === 'register');
    this.message.set('');
  }

  protected submit(): void {
    const form = this.activeForm;
    if (form.invalid) {
      form.markAllAsTouched();
      this.message.set('Ellenőrizd a megadott adatokat.');
      return;
    }
    if (this.guestMode()) {
      this.busy.set(true);
      const { joinCode, displayName } = this.guestForm.getRawValue();
      this.sessions.join(joinCode, displayName).subscribe({
        next: result => {
          if (!result.accessToken || !result.accessTokenExpiresAtUtc) {
            this.busy.set(false);
            this.message.set('A vendég hozzáférés nem érkezett meg.');
            return;
          }
          this.store.set({
            accessToken: result.accessToken,
            refreshToken: null,
            accessTokenExpiresAtUtc: result.accessTokenExpiresAtUtc
          });
          void this.router.navigate(['/lobby', result.sessionId]);
        },
        error: () => {
          this.busy.set(false);
          this.message.set('A csatlakozás nem sikerült. Ellenőrizd a kódot és a nevet.');
        }
      });
      return;
    }
    this.busy.set(true);
    const credentials = this.registerMode() ? this.registerForm.getRawValue() : this.loginForm.getRawValue();
    const request = this.registerMode()
      ? this.api.register(credentials.email, credentials.password, this.registerForm.controls.displayName.value)
      : this.api.login(credentials.email, credentials.password);
    request.subscribe({
      next: tokens => { this.store.set(tokens); void this.router.navigateByUrl('/profile'); },
      error: () => { this.busy.set(false); this.message.set('A művelet nem sikerült. Ellenőrizd az adatokat és az API kapcsolatot.'); }
    });
  }
}
