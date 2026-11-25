import { Injectable, computed, signal } from '@angular/core';
import { AuthTokens } from '../generated/api/models';

type SessionTokens = Omit<AuthTokens, 'refreshToken'> & { refreshToken: string | null };

@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly storageKey = 'ceq.auth';
  readonly tokens = signal<SessionTokens | null>(this.read());
  readonly isAuthenticated = computed(() => {
    const current = this.tokens();
    return current !== null && new Date(current.accessTokenExpiresAtUtc).getTime() > Date.now();
  });

  set(tokens: SessionTokens): void {
    sessionStorage.setItem(this.storageKey, JSON.stringify(tokens));
    this.tokens.set(tokens);
  }

  clear(): void {
    sessionStorage.removeItem(this.storageKey);
    this.tokens.set(null);
  }

  private read(): SessionTokens | null {
    try {
      const value = sessionStorage.getItem(this.storageKey);
      return value ? JSON.parse(value) as SessionTokens : null;
    } catch {
      return null;
    }
  }
}
