import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EMPTY, Subject, catchError, startWith, switchMap } from 'rxjs';
import { GameSessionApi, GameSessionLobby } from '../../core/api';

@Component({
  selector: 'ceq-lobby-page',
  imports: [AsyncPipe, DatePipe, RouterLink],
  templateUrl: './lobby.html'
})
export class Lobby {
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

  protected currentLocationReady(lobby: GameSessionLobby): boolean {
    return lobby.participants.find(participant => participant.isCurrent)?.isLocationReady ?? false;
  }

  protected isOwner(lobby: GameSessionLobby): boolean {
    return lobby.participants.some(participant => participant.isCurrent && participant.isOwner);
  }

  protected allLocationsReady(lobby: GameSessionLobby): boolean {
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
