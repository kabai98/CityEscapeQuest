import { AsyncPipe, DecimalPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, of } from 'rxjs';
import { MissionApi, MissionSummary } from '../../core/api';

const demoMissions: readonly MissionSummary[] = [
  { id: '00000000-0000-0000-0000-000000000001', title: 'A budai kód', shortDescription: 'Egy eltűnt térképész nyomában a Várnegyed sikátoraiban.', recommendedPlayers: 4, estimatedDurationMinutes: 90, price: null, isFree: true, previewImageUrl: '' },
  { id: '00000000-0000-0000-0000-000000000002', title: 'Éjféli frekvencia', shortDescription: 'Fejtsetek meg egy évtizedek óta hallgató rádióadás üzenetét.', recommendedPlayers: 3, estimatedDurationMinutes: 120, price: 6990, isFree: false, previewImageUrl: '' },
  { id: '00000000-0000-0000-0000-000000000003', title: 'A Duna árnyéka', shortDescription: 'Fényképek, hangok és rejtett jelek vezetnek át a rakparton.', recommendedPlayers: 5, estimatedDurationMinutes: 105, price: 8990, isFree: false, previewImageUrl: '' }
];

@Component({
  selector: 'ceq-missions-page',
  imports: [AsyncPipe, DecimalPipe, RouterLink],
  templateUrl: './mission-list.html'
})
export class MissionList {
  private readonly api = inject(MissionApi);
  protected readonly missions$ = this.api.browse().pipe(catchError(() => of(demoMissions)));
}
