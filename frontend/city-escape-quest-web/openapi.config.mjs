import { defineConfig } from 'ng-openapi';

export default defineConfig({
  input: process.env['CEQ_OPENAPI_URL'] ?? 'http://localhost:5245/swagger/v1/swagger.json',
  output: 'src/app/generated/api',
  options: {
    dateType: 'string',
    enumStyle: 'union'
  },
  validateInput: spec => {
    const hasRequiredPaths = [
      '/api/auth/login',
      '/api/auth/register',
      '/api/auth/refresh',
      '/api/auth/revoke',
      '/api/missions',
      '/api/missions/{id}',
      '/api/game-sessions',
      '/api/game-sessions/join',
      '/api/game-sessions/{id}',
      '/api/game-sessions/{id}/location',
      '/api/game-sessions/{id}/start',
      '/api/game-sessions/{id}/state',
      '/api/game-sessions/{id}/answer'
    ].every(path => path in spec.paths);
    const hasTypedAuth = Boolean(spec.components?.schemas?.['AuthTokens']?.required?.includes('accessToken'));
    const hasOperationNames =
      spec.paths['/api/auth/login']?.post?.operationId === 'AuthLogin' &&
      spec.paths['/api/game-sessions/{id}/state']?.get?.operationId === 'GameSessionsGetState';

    if (!hasRequiredPaths || !hasTypedAuth || !hasOperationNames) {
      throw new Error('A Swagger séma elavult. Indítsd újra a jelenlegi backendet, majd futtasd újra az api:generate parancsot.');
    }

    return true;
  }
});
