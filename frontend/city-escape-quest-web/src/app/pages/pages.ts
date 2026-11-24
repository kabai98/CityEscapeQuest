import { AsyncPipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { catchError, of } from 'rxjs';
import { AuthApi, MissionApi, MissionSummary } from '../core/api';
import { AuthStore } from '../core/auth.store';

const demoMissions: readonly MissionSummary[] = [
  { id: '00000000-0000-0000-0000-000000000001', title: 'A budai kód', shortDescription: 'Egy eltűnt térképész nyomában a Várnegyed sikátoraiban.', recommendedPlayers: 4, estimatedDurationMinutes: 90, price: null, isFree: true, previewImageUrl: '' },
  { id: '00000000-0000-0000-0000-000000000002', title: 'Éjféli frekvencia', shortDescription: 'Fejtsetek meg egy évtizedek óta hallgató rádióadás üzenetét.', recommendedPlayers: 3, estimatedDurationMinutes: 120, price: 6990, isFree: false, previewImageUrl: '' },
  { id: '00000000-0000-0000-0000-000000000003', title: 'A Duna árnyéka', shortDescription: 'Fényképek, hangok és rejtett jelek vezetnek át a rakparton.', recommendedPlayers: 5, estimatedDurationMinutes: 105, price: 8990, isFree: false, previewImageUrl: '' }
];

@Component({
  selector: 'ceq-home-page',
  imports: [RouterLink],
  template: `
    <section class="hero">
      <div class="eyebrow"><span></span>Városi kaland · valódi helyszínek</div>
      <h1>A város tele van<br><em>megoldatlan</em> történetekkel.</h1>
      <p>Gyűjtsd össze a csapatod, kövesd a nyomokat, és változtasd Budapest utcáit egy élő szabadulószobává.</p>
      <div class="actions"><a class="button primary" routerLink="/missions">Küldetések felfedezése</a><a class="button ghost" href="#how">Hogyan működik?</a></div>
      <div class="hero-stamp"><strong>01</strong><span>Válassz<br>küldetést</span></div>
    </section>
    <section class="manifesto" id="how">
      <div><span class="section-number">01</span><h2>Nem egy újabb<br>képernyős játék.</h2></div>
      <div><p>A City Escape Quest kimozdít. A történet a telefonodon indul, de a megoldás az utcákon vár.</p><ul><li><b>01</b> Hívd össze a csapatot</li><li><b>02</b> Érkezzetek a kezdőpontra</li><li><b>03</b> Oldjátok meg együtt</li></ul></div>
    </section>
    <section class="features">
      <article><span>◎</span><h3>Valódi helyszínek</h3><p>GPS-alapú indulás, városi nyomok és felfedezés.</p></article>
      <article><span>◉</span><h3>Csapatra tervezve</h3><p>Minden játékos számít, minden nézőpont új lehetőség.</p></article>
      <article><span>⌁</span><h3>Többféle kihívás</h3><p>Fotózás, hangfelismerés, logika és nyomkeresés.</p></article>
    </section>
    <section class="join-band"><p>Már van csapatkódod?</p><a routerLink="/login" [queryParams]="{ mode: 'guest' }">Csatlakozás vendégként →</a></section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomePage {}

@Component({
  selector: 'ceq-missions-page',
  imports: [AsyncPipe, DecimalPipe, RouterLink],
  template: `
    <section class="page-head"><div class="eyebrow"><span></span>Küldetéskatalógus</div><h1>Válasszatok<br><em>történetet.</em></h1><p>A nehézség változik. A csapatszellem mindig kell.</p></section>
    <section class="catalog">
      @if (missions$ | async; as missions) {
        @for (mission of missions; track mission.id; let index = $index) {
          <article class="mission-card">
            <div class="mission-art"><span>0{{ index + 1 }}</span><div class="map-lines"></div></div>
            <div class="mission-copy">
              <div class="tags"><span>{{ mission.recommendedPlayers }} játékos</span><span>{{ mission.estimatedDurationMinutes }} perc</span></div>
              <h2>{{ mission.title }}</h2><p>{{ mission.shortDescription }}</p>
              <div class="card-foot"><strong>{{ mission.isFree ? 'Ingyenes' : ((mission.price ?? 0) | number:'1.0-0') + ' Ft' }}</strong><a [routerLink]="['/missions', mission.id]">Részletek →</a></div>
            </div>
          </article>
        }
      }
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MissionsPage {
  private readonly api = inject(MissionApi);
  protected readonly missions$ = this.api.browse().pipe(catchError(() => of(demoMissions)));
}

@Component({
  selector: 'ceq-mission-details-page',
  imports: [RouterLink],
  template: `
    <section class="detail">
      <a class="back-link" routerLink="/missions">← Vissza a küldetésekhez</a>
      <div class="detail-grid"><div><div class="eyebrow"><span></span>Kiemelt küldetés</div><h1>A város következő<br><em>titka vár.</em></h1><p>Gyűjtsd össze a csapatod. A pontos történet, kezdőpont és részletes játékszabályok az API-adatok betöltése után jelennek meg.</p><div class="actions"><button class="button primary" type="button">Csapat létrehozása</button></div></div><aside><span>Indulás előtt</span><ul><li>Minden csapattagnak a kezdőzónában kell lennie.</li><li>Engedélyezd a pontos helymeghatározást.</li><li>A küldetést a csapatvezető indítja.</li></ul></aside></div>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MissionDetailsPage { readonly id = input<string>(); }

@Component({
  selector: 'ceq-login-page',
  imports: [ReactiveFormsModule],
  template: `
    <section class="auth-shell">
      <div><div class="eyebrow"><span></span>Belépés a kalandba</div><h1>{{ guestMode() ? 'Csatlakozz a csapathoz.' : (registerMode() ? 'Kezdődjön a történeted.' : 'Folytasd a történeted.') }}</h1><p>{{ guestMode() ? 'Nem kell regisztrálnod. Add meg a csapatkódot és a neved.' : (registerMode() ? 'Hozd létre a játékosprofilodat néhány lépésben.' : 'Lépj be a küldetéseidhez, achievementjeidhez és csapataidhoz.') }}</p></div>
      <form [formGroup]="activeForm" (ngSubmit)="submit()">
        <div class="auth-tabs"><button type="button" [class.active]="!guestMode() && !registerMode()" (click)="selectMode('login')">Belépés</button><button type="button" [class.active]="registerMode()" (click)="selectMode('register')">Regisztráció</button><button type="button" [class.active]="guestMode()" (click)="selectMode('guest')">Vendég</button></div>
        @if (guestMode()) {
          <label>Csapatkód<input formControlName="joinCode" inputmode="numeric" placeholder="123456"></label>
          <label>Megjelenő név<input formControlName="displayName" autocomplete="nickname" placeholder="Hogy szólíthatunk?"></label>
        } @else {
          @if (registerMode()) { <label>Megjelenő név<input formControlName="displayName" autocomplete="nickname" placeholder="Hogy szólíthatunk?"></label> }
          <label>E-mail<input formControlName="email" type="email" autocomplete="email" placeholder="nev@pelda.hu"></label>
          <label>Jelszó<input formControlName="password" type="password" [attr.autocomplete]="registerMode() ? 'new-password' : 'current-password'" placeholder="Legalább 10 karakter"></label>
        }
        <button class="button primary full" type="submit" [disabled]="busy()">{{ busy() ? 'Kapcsolódás…' : (guestMode() ? 'Csatlakozás' : (registerMode() ? 'Fiók létrehozása' : 'Belépés')) }}</button>
        @if (message()) { <p class="form-message">{{ message() }}</p> }
      </form>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginPage {
  private readonly api = inject(AuthApi);
  private readonly store = inject(AuthStore);
  private readonly router = inject(Router);
  protected readonly guestMode = signal(false);
  protected readonly registerMode = signal(false);
  protected readonly busy = signal(false);
  protected readonly message = signal('');
  protected readonly loginForm = new FormGroup({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(10)] })
  });
  protected readonly registerForm = new FormGroup({
    displayName: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(80)] }),
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(10)] })
  });
  protected readonly guestForm = new FormGroup({
    joinCode: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.pattern(/^\\d{6}$/)] }),
    displayName: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(80)] })
  });
  protected get activeForm(): FormGroup {
    return this.guestMode() ? this.guestForm : (this.registerMode() ? this.registerForm : this.loginForm);
  }

  protected selectMode(mode: 'login' | 'register' | 'guest'): void {
    this.guestMode.set(mode === 'guest');
    this.registerMode.set(mode === 'register');
    this.message.set('');
  }

  protected submit(): void {
    const form = this.activeForm;
    if (form.invalid) {
      form.markAllAsTouched();
      this.message.set('Ellenőrizd a megadott adatokat.');
      return;
    }
    if (this.guestMode()) {
      this.message.set('A vendég csatlakozási végpont a következő backend iterációban készül el.');
      return;
    }
    this.busy.set(true);
    const credentials = this.registerMode() ? this.registerForm.getRawValue() : this.loginForm.getRawValue();
    const request = this.registerMode()
      ? this.api.register(credentials.email, credentials.password, this.registerForm.controls.displayName.value)
      : this.api.login(credentials.email, credentials.password);
    request.subscribe({
      next: tokens => { this.store.set(tokens); void this.router.navigateByUrl('/profile'); },
      error: () => { this.busy.set(false); this.message.set('A művelet nem sikerült. Ellenőrizd az adatokat és az API kapcsolatot.'); }
    });
  }
}

@Component({
  selector: 'ceq-profile-page',
  imports: [RouterLink],
  template: `
    <section class="page-head compact"><div class="eyebrow"><span></span>Játékosprofil</div><h1>A te<br><em>kalandnaplód.</em></h1></section>
    <section class="profile-grid">
      <article class="profile-card"><div class="avatar">CQ</div><h2>{{ auth.isAuthenticated() ? 'Kalandor' : 'Vendég játékos' }}</h2><p>{{ auth.isAuthenticated() ? 'A profilod készen áll a következő küldetésre.' : 'Jelentkezz be az eredményeid és achievementjeid mentéséhez.' }}</p>@if (!auth.isAuthenticated()) { <a class="button primary" routerLink="/login">Belépés</a> } @else { <button class="button ghost" type="button" (click)="auth.clear()">Kijelentkezés</button> }</article>
      <article class="stats"><div><strong>0</strong><span>Teljesített küldetés</span></div><div><strong>0</strong><span>Achievement</span></div><div><strong>—</strong><span>Következő rang</span></div></article>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfilePage { protected readonly auth = inject(AuthStore); }
