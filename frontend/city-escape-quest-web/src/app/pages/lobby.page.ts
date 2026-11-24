import { AsyncPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
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
          <p class="lobby-hint">Az indítás akkor válik elérhetővé, amikor minden játékos csatlakozott és a kezdőhelyszínen tartózkodik.</p>
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
  private readonly reload = new Subject<void>();
  private readonly sessionId = this.route.snapshot.paramMap.get('id') ?? '';
  protected readonly joinCode = signal(this.route.snapshot.queryParamMap.get('joinCode'));
  protected readonly errorMessage = signal('');
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
}
