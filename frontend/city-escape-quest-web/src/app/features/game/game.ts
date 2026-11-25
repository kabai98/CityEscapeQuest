import { AsyncPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EMPTY, Subject, catchError, startWith, switchMap } from 'rxjs';
import { GameSessionApi } from '../../core/api';

@Component({
  selector: 'ceq-game-page',
  imports: [AsyncPipe, ReactiveFormsModule, RouterLink],
  templateUrl: './game.html'
})
export class Game {
  private readonly route = inject(ActivatedRoute);
  private readonly sessions = inject(GameSessionApi);
  private readonly reload = new Subject<void>();
  private readonly sessionId = this.route.snapshot.paramMap.get('id') ?? '';
  protected readonly message = signal('');
  protected readonly submitting = signal(false);
  protected readonly answerForm = new FormGroup({
    answer: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] })
  });
  protected readonly state$ = this.reload.pipe(
    startWith(undefined),
    switchMap(() => this.sessions.getState(this.sessionId)),
    catchError(() => { this.message.set('A játékállapot nem tölthető be.'); return EMPTY; })
  );

  protected submitAnswer(): void {
    if (this.answerForm.invalid) {
      this.answerForm.markAllAsTouched();
      return;
    }
    this.submitting.set(true);
    this.message.set('');
    this.sessions.submitAnswer(this.sessionId, this.answerForm.controls.answer.value).subscribe({
      next: () => { this.submitting.set(false); this.reload.next(); },
      error: () => { this.submitting.set(false); this.message.set('Ez még nem a helyes megfejtés. Nézzetek körül a téren!'); }
    });
  }
}
