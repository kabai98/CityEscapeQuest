# City Escape Quest – Git munkafolyamat

Ez a projekt a `https://github.com/kabai98/CityEscapeQuest` repositoryhoz tartozik. A munka alapja a GitHubon lévő `main`; új feladatot ne egy korábbi feature branchből indíts.

## Új munka indítása

1. Ellenőrizd a repository gyökerét, az aktuális branchet, a remote-ot és a munkakönyvtár állapotát. A felhasználó meglévő módosításait őrizd meg; ne töröld vagy írd felül őket.
2. Frissítsd a remote állapotát, majd a helyi `main` branchet kizárólag fast-forward módban húzd le az `origin/main` ágról. Ha a `main` eltért vagy a munkakönyvtár akadályozza a váltást, állj meg és ismertesd a konkrét eltérést.
3. A friss `main` commitból hozz létre új, rövid életű branchet a feladat előtt. A nevét a munka alapján válaszd meg; ne kérj külön branchnév-jóváhagyást, ha a feladatból egyértelmű név képezhető.

## Branchnév

Használj egyértelmű típust és rövid, kötőjeles angol leírást, például `feature/mission-progress`, `fix/guest-join`, `hotfix/token-refresh`, `bughunt/location-start` vagy `chore/git-workflow-guidance`. A típus a munka jellegét kövesse; nem zárt felsorolás.

Ha a feladathoz létező GitHub issue tartozik, a branchnév az issue címének tömör, felismerhető változatán alapuljon, és tartalmazza az issue számát is, például `fix/42-guest-join`. Ha nincs kapcsolódó issue, ne találj ki ticketet vagy számot. Ha a kért név már foglalt vagy érvénytelen, válassz másik egyértelmű nevet; csak valódi névütközés vagy értelmezési bizonytalanság esetén kérdezz.

## Változtatás és közzététel

- Csak a feladathoz tartozó fájlokat stage-eld. Ne vegyél fel titkokat, helyi adatbázist, buildkimenetet vagy a Git által ignorált `docs` mappát.
- Commit előtt ellenőrizd a staged diffet és futtasd a változáshoz illő ellenőrzéseket. A commitüzenet legyen rövid, cselekvő és illeszkedjen a repository magyar nyelvű előzményeihez.
- Push és pull request akkor történjen, amikor a felhasználó kéri a közzétételt vagy a jelenlegi munkafolyamatban erre már felhatalmazást adott. A PR célága `main`; a PR mutassa be a termékhatást, az ellenőrzéseket és az esetleges migrációs kockázatot.
- Force push, PR-merge és branch-törlés külön felhatalmazást igényel.

A projektgyökér a kanonikus checkout. A `.runtime` alatti régi checkoutok és archívumok helyi segédmappák; új feladatot alapértelmezésben innen, a projektgyökérből kezdj.
