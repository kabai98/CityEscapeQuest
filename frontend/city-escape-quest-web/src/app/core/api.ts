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
export class AuthApi {
  private readonly http = inject(HttpClient);

  login(email: string, password: string): Observable<AuthTokens> {
    return this.http.post<AuthTokens>('/api/auth/login', { email, password });
  }

  register(email: string, password: string, displayName: string): Observable<AuthTokens> {
    return this.http.post<AuthTokens>('/api/auth/register', { email, password, displayName });
  }
}
