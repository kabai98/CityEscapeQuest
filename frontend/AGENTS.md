# Frontend development guidance

This directory contains the City Escape Quest Angular web client. Follow the Angular team's official [angular-developer skill](https://github.com/angular/skills/tree/main/angular-developer) when it is available. For conflicting Angular patterns, follow that skill and the documentation for the version pinned in `package.json` (currently Angular 22).

- Use standalone components and functional providers. Do not introduce NgModules. In Angular 20+, standalone is the default; do not add redundant `standalone: true`.
- Use signals for local state, `computed()` for derived state, and built-in template control flow (`@if`, `@for`, `@switch`). Angular 22 uses OnPush by default; do not add redundant change-detection metadata to new components.
- Prefer Signal Forms for new Angular 22 forms. Keep existing typed reactive forms unless the task calls for migrating them or a focused change makes migration worthwhile.
- Use the built-in router, HttpClient and functional interceptors. Organize by user-facing capability; keep API contracts and authentication concerns in `core`.
- Keep components focused on presentation and interaction. Put HTTP and shared session state in injectable clients or stores.
- Design mobile-first and preserve keyboard navigation, semantic HTML, visible focus and sufficient contrast.
- Never embed API secrets or production credentials in the browser bundle.
- Check the project's Angular version before choosing version-specific APIs, and run the production build after Angular code changes.
