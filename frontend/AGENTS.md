# Frontend development guidance

This directory contains the City Escape Quest web client.

- Use current stable Angular with standalone components and functional providers. Do not introduce NgModules.
- Prefer signals for local UI state, typed reactive forms, the built-in router and HttpClient.
- Organize code by user-facing capability. Keep API contracts and authentication concerns in `core`.
- Components should remain focused on presentation and interaction; keep HTTP and session state in injectable clients/stores.
- Design mobile-first and preserve keyboard navigation, semantic HTML, visible focus and sufficient contrast.
- Never embed API secrets or production credentials in the browser bundle.
- Run the production build before committing frontend changes.
