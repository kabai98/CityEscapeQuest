# Backend fejlesztési irányelvek

Kövesd a gyökér `AGENTS.md` Git-munkafolyamatát. A solution a projektgyökérben lévő `CityEscapeQuest.slnx`; a backend projektek a `backend/src` alatt vannak. A meglévő controlleres API-t tartsd meg.

## Architektúra

- `Domain`: entitások, értékobjektumok és üzleti invariánsok. Nincs ASP.NET Core- vagy EF Core-függés.
- `Application`: játék- és küldetésfolyamatok, alkalmazási szerződések és az `IApplicationDbContext` határ. A felhasználó döntése alapján itt az EF Core `DbSet<T>` és lekérdező API-k használhatók; ez tudatos kompromisszum a szigorú EF-független Application réteghez képest. HTTP- és Infrastructure-típusok itt nem használhatók.
- `Infrastructure`: SQL Serveres `AppDbContext`, Identity, EF-konfigurációk, migrációk és külső integrációk. A `WebApi` rétegtől független.
- `WebApi`: request/response kezelés, bemeneti validáció, autentikáció, OpenAPI és middleware. A controllerekben nincs üzleti szabály vagy közvetlen adatbázis-lekérdezés.

Ne vezess be CQRS-t, MediatR-t, Generic Repositoryt (`IRepository<T>`) vagy külön `IUnitOfWork` absztrakciót. Új entitáshoz ne készíts automatikusan repositoryt. Az EF Core `DbContext` kezeli a mentési egységet.

## Szerződések és validáció

- Az alkalmazási bemeneti és kimeneti típusok az adott képesség `Application` mappájában legyenek; több összetartozó egyszerű recordot lehet `*Contracts.cs` fájlban tartani. A domain entitásokat ne add vissza közvetlenül az API-ból.
- A HTTP-kérések és a hozzájuk tartozó FluentValidation validatorok a `WebApi` rétegbe tartoznak. Összetettebb bemeneti szabályokhoz hívd a `ValidateAsync` metódust, és adj vissza konzisztens 400-as validációs választ. Ne használd a régi automatikus MVC-validációs integrációt.
- A FluentValidation a bemeneti adatokra való; az üzleti invariánsokat a Domain akkor is ellenőrzi. Ne készíts validatort minden egyszerű recordhoz csak a minta kedvéért.
- Ha a nyilvános HTTP-válasz eltér az alkalmazási eredménytől, hozz létre külön WebApi-válaszmodellt és explicit leképezést.

## Adat és alkalmazásindítás

- Olvasáskor használj `AsNoTracking()`-ot, ha az entitást nem módosítod. Listákat lapozz, és egyszerű megjelenítéshez csak a szükséges mezőket kérd le. Módosításkor a követett aggregátumon érvényesítsd a domain szabályokat, majd hívd a `SaveChangesAsync(cancellationToken)` metódust.
- Az EF-konfigurációk az `Infrastructure/Persistence/Configurations` mappában vannak; új séma-változáshoz új migrációt generálj, meglévőt ne írj át kézzel.
- A hosszabb CORS-, rate limit-, JWT- és OpenAPI-beállítások a `WebApi/Configuration` mappában legyenek. A `Program.cs` mutassa a rétegek bekötését, a middleware-ek sorrendjét, a végpontokat és az indítás feltételeit. Ne írj elő mesterséges sorszámkorlátot.
- Endpoint alapú rate limit esetén `UseRouting()` fusson a `UseRateLimiter()` előtt; a CORS, authentication és authorization sorrendjét is őrizd meg.

## Ellenőrzés

Az érintett projektet építsd és teszteld először, majd szükség esetén futtasd a teljes `dotnet build CityEscapeQuest.slnx` és `dotnet test CityEscapeQuest.slnx` parancsokat a projektgyökérből. Viselkedésváltozásnál a releváns tesztet frissítsd; adatbázis-működéshez valódi SQL Server-kompatibilis integrációs teszt szükséges.

Háttér: [Microsoft Learn – Clean Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures), [EF Core DbContext](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/) és [ASP.NET Core middleware-sorrend](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0).
