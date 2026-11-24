import { AsyncPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EMPTY, Subject, catchError, startWith, switchMap } from 'rxjs';
import { GameSessionApi } from '../core/api';

@Component({
  selector: 'ceq-lobby-page',
  imports: [AsyncPipe, DatePipe, RouterLink],
  template: `
    <section class="page-head compact">
      <div class="eyebrow"><span></span>Csapatlobby</div>
      <h1>A csapat<br><em>összeáll.</em></h1>
    </section>
    <section class="lobby-shell">
      @if (lobby$ | async; as lobby) {
        <article class="lobby-card">
          <div class="lobby-card-head">
            <div><span>Állapot</span><strong>{{ lobby.status }}</strong></div>
            <button class="button ghost" type="button" (click)="refresh()">Frissítés</button>
          </div>
          @if (joinCode()) {
            <div class="join-code"><span>Csapatkód</span><strong>{{ joinCode() }}</strong><small>Érvényes: {{ lobby.joinCodeExpiresAtUtc | date:'short' }}</small></div>
          }
          <h2>Résztvevők</h2>
          <ul class="participant-list">
            @for (participant of lobby.participants; track participant.id) {
              <li><span class="avatar small">{{ participant.displayName.slice(0, 2).toUpperCase() }}</span><div><strong>{{ participant.displayName }}</strong><small>{{ participant.isOwner ? 'Csapatvezető' : (participant.isGuest ? 'Vendég' : 'Játékos') }}</small></div></li>
            }
          </ul>
          <div class="location-actions">
            <button class="button ghost" type="button" [disabled]="locating()" (click)="reportMyLocation()">
              {{ locating() ? 'Helyzet mérése…' : (currentLocationReady(lobby) ? 'Helyzet frissítése ✓' : 'Helyzetem ellenőrzése') }}
            </button>
            @if (isOwner(lobby)) {
              <button class="button primary" type="button" [disabled]="starting() || !allLocationsReady(lobby)" (click)="startGame()">
                {{ starting() ? 'Indítás…' : 'Játék indítása' }}
              </button>
            }
          </div>
          <p class="lobby-hint">Minden játékosnak a Deák Ferenc tér 120 méteres körzetében kell friss, legfeljebb 50 méteres pontosságú helyzetet küldenie.</p>
        </article>
      }
      @if (errorMessage()) { <p class="form-message" role="alert">{{ errorMessage() }}</p> }
      <a class="back-link" routerLink="/missions">← Másik küldetés választása</a>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LobbyPage {
  private readonly route = inject(ActivatedRoute);
  private readonly sessions = inject(GameSessionApi);
  private readonly router = inject(Router);
  private readonly reload = new Subject<void>();
  private readonly sessionId = this.route.snapshot.paramMap.get('id') ?? '';
  protected readonly joinCode = signal(this.route.snapshot.queryParamMap.get('joinCode'));
  protected readonly errorMessage = signal('');
  protected readonly locating = signal(false);
  protected readonly starting = signal(false);
  protected readonly lobby$ = this.reload.pipe(
    startWith(undefined),
    switchMap(() => this.sessions.getLobby(this.sessionId)),
    catchError(() => {
      this.errorMessage.set('A lobby nem tölthető be, vagy nincs hozzá jogosultságod.');
      return EMPTY;
    })
  );

  protected refresh(): void {
    this.errorMessage.set('');
    this.reload.next();
  }

  protected currentLocationReady(lobby: import('../core/api').GameSessionLobby): boolean {
    return lobby.participants.find(participant => participant.isCurrent)?.isLocationReady ?? false;
  }

  protected isOwner(lobby: import('../core/api').GameSessionLobby): boolean {
    return lobby.participants.some(participant => participant.isCurrent && participant.isOwner);
  }

  protected allLocationsReady(lobby: import('../core/api').GameSessionLobby): boolean {
    return lobby.participants.length > 0 && lobby.participants.every(participant => participant.isLocationReady);
  }

  protected reportMyLocation(): void {
    if (!navigator.geolocation) {
      this.errorMessage.set('A böngésző nem támogatja a helymeghatározást.');
      return;
    }

    this.locating.set(true);
    this.errorMessage.set('');
    navigator.geolocation.getCurrentPosition(
      position => {
        this.sessions.reportLocation(
          this.sessionId,
          position.coords.latitude,
          position.coords.longitude,
          position.coords.accuracy,
          new Date(position.timestamp).toISOString()
        ).subscribe({
          next: () => { this.locating.set(false); this.refresh(); },
          error: () => { this.locating.set(false); this.errorMessage.set('A helyzet mentése nem sikerült.'); }
        });
      },
      () => { this.locating.set(false); this.errorMessage.set('A pontos helyzet engedélyezése szükséges a játékhoz.'); },
      { enableHighAccuracy: true, timeout: 15_000, maximumAge: 0 }
    );
  }

  protected startGame(): void {
    this.starting.set(true);
    this.errorMessage.set('');
    this.sessions.start(this.sessionId).subscribe({
      next: () => void this.router.navigate(['/game', this.sessionId]),
      error: () => { this.starting.set(false); this.errorMessage.set('A játék nem indítható. Ellenőrizd minden csapattag helyzetét.'); }
    });
  }
}
