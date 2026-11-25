import {
  HttpContext,
  HttpContextToken,
  HttpErrorResponse,
  HttpInterceptorFn
} from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, tap, throwError } from 'rxjs';
import { AuthStore } from './auth.store';
import { AuthService } from '../generated/api/services';

const skipRefresh = new HttpContextToken<boolean>(() => false);

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const store = inject(AuthStore);
  const auth = inject(AuthService);
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

      return auth.authRefresh(
        { refreshToken },
        'body',
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
