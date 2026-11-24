import {
  HttpClient,
  HttpContext,
  HttpContextToken,
  HttpErrorResponse,
  HttpInterceptorFn
} from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, tap, throwError } from 'rxjs';
import { AuthTokens, AuthStore } from './auth.store';

const skipRefresh = new HttpContextToken<boolean>(() => false);

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const store = inject(AuthStore);
  const http = inject(HttpClient);
  const accessToken = store.tokens()?.accessToken;
  const authenticatedRequest = accessToken
    ? request.clone({ setHeaders: { Authorization: `Bearer ${accessToken}` } })
    : request;

  return next(authenticatedRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      const refreshToken = store.tokens()?.refreshToken;
      if (error.status !== 401 || !refreshToken || request.context.get(skipRefresh)) {
        return throwError(() => error);
      }

      return http.post<AuthTokens>(
        '/api/auth/refresh',
        { refreshToken },
        { context: new HttpContext().set(skipRefresh, true) }
      ).pipe(
        tap(tokens => store.set(tokens)),
        switchMap(tokens => next(request.clone({
          setHeaders: { Authorization: `Bearer ${tokens.accessToken}` }
        }))),
        catchError(refreshError => {
          store.clear();
          return throwError(() => refreshError);
        })
      );
    })
  );
};
