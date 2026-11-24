import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthTokens } from './auth.store';

export interface MissionSummary {
  id: string;
  title: string;
  shortDescription: string;
  recommendedPlayers: number;
  estimatedDurationMinutes: number;
  price: number | null;
  isFree: boolean;
  previewImageUrl: string;
}

export interface MissionDetails extends MissionSummary {
  minimumPlayers: number;
  startLatitude: number;
  startLongitude: number;
  startRadiusMeters: number;
  isLocationCheckEnabled: boolean;
}

export interface CreatedGameSession {
  sessionId: string;
  joinCode: string;
  joinCodeExpiresAtUtc: string;
}

export interface JoinedGameSession {
  sessionId: string;
  participantId: string;
  accessToken: string | null;
  accessTokenExpiresAtUtc: string | null;
}

export interface LobbyParticipant {
  id: string;
  displayName: string;
  isGuest: boolean;
  isOwner: boolean;
  isCurrent: boolean;
  isLocationReady: boolean;
}

export interface GamePlayState {
  sessionId: string;
  status: string;
  missionTitle: string;
  storyIntro: string;
  taskPrompt: string | null;
  completionText: string | null;
  startedAtUtc: string | null;
  completedAtUtc: string | null;
}

export interface GameSessionLobby {
  sessionId: string;
  missionId: string;
  status: string;
  joinCodeExpiresAtUtc: string;
  isLocationCheckEnabled: boolean;
  participants: readonly LobbyParticipant[];
}

@Injectable({ providedIn: 'root' })
export class MissionApi {
  private readonly http = inject(HttpClient);

  browse(): Observable<readonly MissionSummary[]> {
    return this.http.get<readonly MissionSummary[]>('/api/missions?page=1&pageSize=20');
  }

  get(id: string): Observable<MissionDetails> {
    return this.http.get<MissionDetails>(`/api/missions/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class GameSessionApi {
  private readonly http = inject(HttpClient);

  create(missionId: string): Observable<CreatedGameSession> {
    return this.http.post<CreatedGameSession>('/api/game-sessions', { missionId });
  }

  join(joinCode: string, displayName: string): Observable<JoinedGameSession> {
    return this.http.post<JoinedGameSession>('/api/game-sessions/join', { joinCode, displayName });
  }

  getLobby(sessionId: string): Observable<GameSessionLobby> {
    return this.http.get<GameSessionLobby>(`/api/game-sessions/${sessionId}`);
  }

  reportLocation(
    sessionId: string,
    latitude: number,
    longitude: number,
    accuracyMeters: number,
    measuredAtUtc: string
  ): Observable<void> {
    return this.http.post<void>(`/api/game-sessions/${sessionId}/location`, {
      latitude,
      longitude,
      accuracyMeters,
      measuredAtUtc
    });
  }

  start(sessionId: string): Observable<GamePlayState> {
    return this.http.post<GamePlayState>(`/api/game-sessions/${sessionId}/start`, {});
  }

  getState(sessionId: string): Observable<GamePlayState> {
    return this.http.get<GamePlayState>(`/api/game-sessions/${sessionId}/state`);
  }

  submitAnswer(sessionId: string, answer: string): Observable<GamePlayState> {
    return this.http.post<GamePlayState>(`/api/game-sessions/${sessionId}/answer`, { answer });
  }
}

@Injectable({ providedIn: 'root' })
export class AuthApi {
  private readonly http = inject(HttpClient);

  login(email: string, password: string): Observable<AuthTokens> {
    return this.http.post<AuthTokens>('/api/auth/login', { email, password });
  }

  register(email: string, password: string, displayName: string): Observable<AuthTokens> {
    return this.http.post<AuthTokens>('/api/auth/register', { email, password, displayName });
  }

  refresh(refreshToken: string): Observable<AuthTokens> {
    return this.http.post<AuthTokens>('/api/auth/refresh', { refreshToken });
  }

  revoke(refreshToken: string): Observable<void> {
    return this.http.post<void>('/api/auth/revoke', { refreshToken });
  }
}
