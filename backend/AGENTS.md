# Backend fejlesztĂ©si irĂˇnyelvek

KĂ¶vesd a gyĂ¶kĂ©r `AGENTS.md` Git-munkafolyamatĂˇt. A solution a projektgyĂ¶kĂ©rben lĂ©vĹ‘ `CityEscapeQuest.slnx`; a backend projektek a `backend` alatt vannak. A meglĂ©vĹ‘ controlleres API-t tartsd meg.

## ArchitektĂşra

- `Domain`: entitĂˇsok, Ă©rtĂ©kobjektumok Ă©s ĂĽzleti invariĂˇnsok. Nincs ASP.NET Core- vagy EF Core-fĂĽggĂ©s.
- `Application`: jĂˇtĂ©k- Ă©s kĂĽldetĂ©sfolyamatok, alkalmazĂˇsi szerzĹ‘dĂ©sek Ă©s az `IApplicationDbContext` hatĂˇr. A felhasznĂˇlĂł dĂ¶ntĂ©se alapjĂˇn itt az EF Core `DbSet<T>` Ă©s lekĂ©rdezĹ‘ API-k hasznĂˇlhatĂłk; ez tudatos kompromisszum a szigorĂş EF-fĂĽggetlen Application rĂ©teghez kĂ©pest. HTTP- Ă©s Infrastructure-tĂ­pusok itt nem hasznĂˇlhatĂłk.
- `Infrastructure`: SQL Serveres `AppDbContext`, Identity, EF-konfigurĂˇciĂłk, migrĂˇciĂłk Ă©s kĂĽlsĹ‘ integrĂˇciĂłk. A `WebApi` rĂ©tegtĹ‘l fĂĽggetlen.
- `WebApi`: request/response kezelĂ©s, bemeneti validĂˇciĂł, autentikĂˇciĂł, OpenAPI Ă©s middleware. A controllerekben nincs ĂĽzleti szabĂˇly vagy kĂ¶zvetlen adatbĂˇzis-lekĂ©rdezĂ©s.

Ne vezess be CQRS-t, MediatR-t, Generic Repositoryt (`IRepository<T>`) vagy kĂĽlĂ¶n `IUnitOfWork` absztrakciĂłt. Ăšj entitĂˇshoz ne kĂ©szĂ­ts automatikusan repositoryt. Az EF Core `DbContext` kezeli a mentĂ©si egysĂ©get.

## Domain mappaszerkezet

A fĹ‘ mappĂˇk ĂĽzleti kĂ©pessĂ©geket jelĂ¶lnek (`GameSessions`, `Missions`, `Achievements`). A nagyobb kĂ©pessĂ©gen belĂĽl az entitĂˇsokat `Entities`, az Ă©rtĂ©kjellegĹ± tĂ­pusokat `ValueObjects`, az Ă¶sszetettebb ĂĽzleti szabĂˇlyokat `Rules` alĂˇ csoportosĂ­tsd. Ă–nĂˇllĂł szĂˇmĂ­tĂˇs kerĂĽlhet `Calculations` alĂˇ. Kis kĂ©pessĂ©gnĂ©l ne kĂ©szĂ­ts ĂĽres almappĂˇkat csak az egysĂ©gessĂ©g kedvĂ©Ă©rt.

Az entitĂˇs tovĂˇbbra is vĂ©dje sajĂˇt ĂˇllapotĂˇt; rĂ¶vid invariĂˇnsokat Ă©s elnevezett hatĂˇrĂ©rtĂ©keket nem kell kĂĽlĂ¶n szabĂˇlyosztĂˇlyba emelni. A mappĂˇk navigĂˇciĂłt szolgĂˇlnak: a meglĂ©vĹ‘ kĂ©pessĂ©gszintĹ± namespace-eket tartsd meg, ha az ĂˇtrendezĂ©s nem vĂˇltoztat felelĹ‘ssĂ©gen.

## SzerzĹ‘dĂ©sek Ă©s validĂˇciĂł

- Az alkalmazĂˇsi bemeneti Ă©s kimeneti tĂ­pusok az adott kĂ©pessĂ©g `Application` mappĂˇjĂˇban legyenek; tĂ¶bb Ă¶sszetartozĂł egyszerĹ± recordot lehet `*Contracts.cs` fĂˇjlban tartani. A domain entitĂˇsokat ne add vissza kĂ¶zvetlenĂĽl az API-bĂłl.
- A HTTP-kĂ©rĂ©sek Ă©s a hozzĂˇjuk tartozĂł FluentValidation validatorok a `WebApi` rĂ©tegbe tartoznak. Ă–sszetettebb bemeneti szabĂˇlyokhoz hĂ­vd a `ValidateAsync` metĂłdust, Ă©s adj vissza konzisztens 400-as validĂˇciĂłs vĂˇlaszt. Ne hasznĂˇld a rĂ©gi automatikus MVC-validĂˇciĂłs integrĂˇciĂłt.
- A FluentValidation a bemeneti adatokra valĂł; az ĂĽzleti invariĂˇnsokat a Domain akkor is ellenĹ‘rzi. Ne kĂ©szĂ­ts validatort minden egyszerĹ± recordhoz csak a minta kedvĂ©Ă©rt.
- Ha a nyilvĂˇnos HTTP-vĂˇlasz eltĂ©r az alkalmazĂˇsi eredmĂ©nytĹ‘l, hozz lĂ©tre kĂĽlĂ¶n WebApi-vĂˇlaszmodellt Ă©s explicit lekĂ©pezĂ©st.

## Adat Ă©s alkalmazĂˇsindĂ­tĂˇs

- OlvasĂˇskor hasznĂˇlj `AsNoTracking()`-ot, ha az entitĂˇst nem mĂłdosĂ­tod. ListĂˇkat lapozz, Ă©s egyszerĹ± megjelenĂ­tĂ©shez csak a szĂĽksĂ©ges mezĹ‘ket kĂ©rd le. MĂłdosĂ­tĂˇskor a kĂ¶vetett aggregĂˇtumon Ă©rvĂ©nyesĂ­tsd a domain szabĂˇlyokat, majd hĂ­vd a `SaveChangesAsync(cancellationToken)` metĂłdust.
- Az EF-konfigurĂˇciĂłk az `Infrastructure/Persistence/Configurations` mappĂˇban vannak; Ăşj sĂ©ma-vĂˇltozĂˇshoz Ăşj migrĂˇciĂłt generĂˇlj, meglĂ©vĹ‘t ne Ă­rj Ăˇt kĂ©zzel.
- A hosszabb CORS-, rate limit-, JWT- Ă©s OpenAPI-beĂˇllĂ­tĂˇsok a `WebApi/Configuration` mappĂˇban legyenek. A `Program.cs` mutassa a rĂ©tegek bekĂ¶tĂ©sĂ©t, a middleware-ek sorrendjĂ©t, a vĂ©gpontokat Ă©s az indĂ­tĂˇs feltĂ©teleit. Ne Ă­rj elĹ‘ mestersĂ©ges sorszĂˇmkorlĂˇtot.
- Endpoint alapĂş rate limit esetĂ©n `UseRouting()` fusson a `UseRateLimiter()` elĹ‘tt; a CORS, authentication Ă©s authorization sorrendjĂ©t is Ĺ‘rizd meg.

## EllenĹ‘rzĂ©s

Az Ă©rintett projektet Ă©pĂ­tsd Ă©s teszteld elĹ‘szĂ¶r, majd szĂĽksĂ©g esetĂ©n futtasd a teljes `dotnet build CityEscapeQuest.slnx` Ă©s `dotnet test CityEscapeQuest.slnx` parancsokat a projektgyĂ¶kĂ©rbĹ‘l. ViselkedĂ©svĂˇltozĂˇsnĂˇl a relevĂˇns tesztet frissĂ­tsd; adatbĂˇzis-mĹ±kĂ¶dĂ©shez valĂłdi SQL Server-kompatibilis integrĂˇciĂłs teszt szĂĽksĂ©ges.

HĂˇttĂ©r: [Microsoft Learn â€“ Clean Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures), [EF Core DbContext](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/) Ă©s [ASP.NET Core middleware-sorrend](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0).
