import { Injectable, computed, signal } from '@angular/core';

export interface AuthTokens {
  accessToken: string;
  refreshToken: string | null;
  accessTokenExpiresAtUtc: string;
}

@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly storageKey = 'ceq.auth';
  readonly tokens = signal<AuthTokens | null>(this.read());
  readonly isAuthenticated = computed(() => {
    const current = this.tokens();
    return current !== null && new Date(current.accessTokenExpiresAtUtc).getTime() > Date.now();
  });

  set(tokens: AuthTokens): void {
    sessionStorage.setItem(this.storageKey, JSON.stringify(tokens));
    this.tokens.set(tokens);
  }

  clear(): void {
    sessionStorage.removeItem(this.storageKey);
    this.tokens.set(null);
  }

  private read(): AuthTokens | null {
    try {
      const value = sessionStorage.getItem(this.storageKey);
      return value ? JSON.parse(value) as AuthTokens : null;
    } catch {
      return null;
    }
  }
}
