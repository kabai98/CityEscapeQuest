# Frontend development guidance

This directory contains the City Escape Quest Angular web client. Follow the Angular team's official [angular-developer skill](https://github.com/angular/skills/tree/main/angular-developer) when it is available. For conflicting Angular patterns, follow that skill and the documentation for the version pinned in `package.json` (currently Angular 22).

- Use standalone components and functional providers. Do not introduce NgModules. In Angular 20+, standalone is the default; do not add redundant `standalone: true`.
- Use signals for local state, `computed()` for derived state, and built-in template control flow (`@if`, `@for`, `@switch`). Angular 22 uses OnPush by default; do not add redundant change-detection metadata to new components.
- Prefer Signal Forms for new Angular 22 forms. Keep existing typed reactive forms unless the task calls for migrating them or a focused change makes migration worthwhile.
- Use the built-in router, HttpClient and functional interceptors. Organize by user-facing capability; keep API contracts and authentication concerns in `core`.
- Keep each page's TypeScript and HTML together in a named feature folder under `src/app/features` (for example `features/home/home.ts` and `features/home/home.html`, or `features/game/game.ts` and `features/game/game.html`). Keep closely related pages together when they share one capability, such as the mission list and mission details in `features/missions`. Do not collect unrelated pages in one `pages` folder or one TypeScript file.
- Match the TypeScript, HTML and optional component CSS basenames. Use an external HTML template for a page; reserve inline templates for genuinely small components. Keep shared, app-wide styling in global CSS and page-specific styling beside its component when it is introduced.
- Keep components focused on presentation and interaction. Put HTTP and shared session state in injectable clients or stores.
- Generate transport models and Angular API services from the backend OpenAPI document with `npm run api:generate`. Treat `src/app/generated/api` as generated code: never edit it by hand or duplicate its request/response contracts in `core` or feature code. Keep UI state, token persistence and interceptors outside the generated directory.
- Design mobile-first and preserve keyboard navigation, semantic HTML, visible focus and sufficient contrast.
- Never embed API secrets or production credentials in the browser bundle.
- Check the project's Angular version before choosing version-specific APIs, and run the production build after Angular code changes.
