import { AsyncPipe, DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EMPTY, catchError, finalize, shareReplay, switchMap } from 'rxjs';
import { AuthStore } from '../../core/auth.store';
import { GameSessionsService, MissionsService } from '../../generated/api/services';

@Component({
  selector: 'ceq-mission-details-page',
  imports: [AsyncPipe, DecimalPipe, RouterLink],
  templateUrl: './mission-details.html',
  styleUrl: './mission-details.css'
})
export class MissionDetails {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly missions = inject(MissionsService);
  private readonly sessions = inject(GameSessionsService);
  private readonly auth = inject(AuthStore);

  protected readonly loading = signal(true);
  protected readonly creating = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly mission$ = this.route.paramMap.pipe(
    switchMap(parameters => this.missions.missionsGet(parameters.get('id') ?? '')),
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
    this.sessions.gameSessionsCreate({ missionId })
      .pipe(finalize(() => this.creating.set(false)))
      .subscribe({
        next: session => void this.router.navigate(['/lobby', session.sessionId], {
          queryParams: { joinCode: session.joinCode }
        }),
        error: () => this.errorMessage.set('A csapat létrehozása nem sikerült. Próbáld újra.')
      });
  }
}
