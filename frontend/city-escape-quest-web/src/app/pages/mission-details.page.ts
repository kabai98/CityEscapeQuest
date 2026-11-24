import { AsyncPipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EMPTY, catchError, finalize, shareReplay, switchMap } from 'rxjs';
import { GameSessionApi, MissionApi } from '../core/api';
import { AuthStore } from '../core/auth.store';

@Component({
  selector: 'ceq-mission-details-page',
  imports: [AsyncPipe, DecimalPipe, RouterLink],
  template: `
    <section class="detail">
      <a class="back-link" routerLink="/missions">← Vissza a küldetésekhez</a>
      @if (mission$ | async; as mission) {
        <div class="detail-grid">
          <div>
            <div class="eyebrow"><span></span>{{ mission.isFree ? 'Ingyenes küldetés' : 'Prémium küldetés' }}</div>
            <h1>{{ mission.title }}</h1>
            <p>{{ mission.shortDescription }}</p>
            <div class="tags">
              <span>{{ mission.minimumPlayers }}–{{ mission.recommendedPlayers }} játékos</span>
              <span>{{ mission.estimatedDurationMinutes }} perc</span>
              <span>{{ mission.isFree ? 'Ingyenes' : ((mission.price ?? 0) | number:'1.0-0') + ' Ft' }}</span>
            </div>
            <div class="actions">
              <button class="button primary" type="button" [disabled]="creating()" (click)="createSession(mission.id)">
                {{ creating() ? 'Csapat létrehozása…' : 'Csapat létrehozása' }}
              </button>
            </div>
          </div>
          <aside>
            <span>Indulási információk</span>
            <ul>
              <li>A kezdőpont {{ mission.startRadiusMeters }} méteres körzetében kell lennetek.</li>
              <li>Minden csapattagnak engedélyeznie kell a pontos helymeghatározást.</li>
              <li>A küldetést a csapatvezető indítja.</li>
            </ul>
          </aside>
        </div>
      } @else if (loading()) {
        <p class="form-message">A küldetés betöltése…</p>
      }
      @if (errorMessage()) {
        <p class="form-message" role="alert">{{ errorMessage() }}</p>
      }
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
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
