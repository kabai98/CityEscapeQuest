import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthStore } from '../../core/auth.store';
import { AuthService } from '../../generated/api/services';

@Component({
  selector: 'ceq-profile-page',
  imports: [RouterLink],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile {
  protected readonly auth = inject(AuthStore);
  private readonly api = inject(AuthService);

  protected logout(): void {
    const refreshToken = this.auth.tokens()?.refreshToken;
    if (!refreshToken) {
      this.auth.clear();
      return;
    }

    this.api.authRevoke({ refreshToken })
      .pipe(finalize(() => this.auth.clear()))
      .subscribe({ error: () => undefined });
  }
}
