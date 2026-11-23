# City Escape Quest

ASP.NET Core backend for a location-based team adventure game with escape-room-style missions.

## Backend structure

- `CityEscapeQuest.Domain`: aggregates, value objects, and business invariants.
- `CityEscapeQuest.Application`: use cases, DTOs, and infrastructure ports.
- `CityEscapeQuest.Infrastructure`: SQL Server EF Core persistence, ASP.NET Core Identity, JWT access tokens, and refresh-token rotation.
- `CityEscapeQuest.WebApi`: controller-based REST API, OpenAPI, ProblemDetails, rate limiting, and health checks.

## Local setup

1. Install the .NET 10 SDK and Microsoft SQL Server or LocalDB.
2. Configure a JWT signing key with at least 32 characters using user secrets or an environment variable. Do not commit production secrets.
3. Apply EF Core migrations once generated for the selected SQL Server instance.
4. Run `dotnet test CityEscapeQuest.slnx`.
5. Run `dotnet run --project src/CityEscapeQuest.WebApi`.

The local `docs` directory is intentionally excluded from Git for the current phase.
