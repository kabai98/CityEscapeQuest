import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthApi } from '../../core/api';
import { AuthStore } from '../../core/auth.store';

@Component({
  selector: 'ceq-profile-page',
  imports: [RouterLink],
  templateUrl: './profile.html'
})
export class Profile {
  protected readonly auth = inject(AuthStore);
  private readonly api = inject(AuthApi);

  protected logout(): void {
    const refreshToken = this.auth.tokens()?.refreshToken;
    if (!refreshToken) {
      this.auth.clear();
      return;
    }

    this.api.revoke(refreshToken)
      .pipe(finalize(() => this.auth.clear()))
      .subscribe({ error: () => undefined });
  }
}
