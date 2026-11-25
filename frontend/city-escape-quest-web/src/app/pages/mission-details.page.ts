import { AsyncPipe, DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EMPTY, catchError, finalize, shareReplay, switchMap } from 'rxjs';
import { GameSessionApi, MissionApi } from '../core/api';
import { AuthStore } from '../core/auth.store';

@Component({
  selector: 'ceq-mission-details-page',
  imports: [AsyncPipe, DecimalPipe, RouterLink],
  templateUrl: './mission-details.page.html'
})
export class MissionDetailsPage {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly missions = inject(MissionApi);
  private readonly sessions = inject(GameSessionApi);
  private readonly auth = inject(AuthStore);

  protected readonly loading = signal(true);
  protected readonly creating = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly mission$ = this.route.paramMap.pipe(
    switchMap(parameters => this.missions.get(parameters.get('id') ?? '')),
    catchError(() => {
      this.errorMessage.set('A küldetés nem tölthető be. Ellenőrizd az API-kapcsolatot.');
      return EMPTY;
    }),
    finalize(() => this.loading.set(false)),
    shareReplay({ bufferSize: 1, refCount: true })
  );

  protected createSession(missionId: string): void {
    if (!this.auth.isAuthenticated()) {
      void this.router.navigate(['/login'], { queryParams: { returnUrl: this.router.url } });
      return;
    }

    this.creating.set(true);
    this.errorMessage.set('');
    this.sessions.create(missionId)
      .pipe(finalize(() => this.creating.set(false)))
      .subscribe({
        next: session => void this.router.navigate(['/lobby', session.sessionId], {
          queryParams: { joinCode: session.joinCode }
        }),
        error: () => this.errorMessage.set('A csapat létrehozása nem sikerült. Próbáld újra.')
      });
  }
}
