import { AsyncPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EMPTY, Subject, catchError, startWith, switchMap } from 'rxjs';
import { GameSessionApi } from '../core/api';

@Component({
  selector: 'ceq-game-page',
  imports: [AsyncPipe, ReactiveFormsModule, RouterLink],
  template: `
    <section class="game-shell">
      @if (state$ | async; as state) {
        <div class="eyebrow"><span></span>{{ state.status }}</div>
        <h1>{{ state.missionTitle }}</h1>
        <article class="story-panel">
          <h2>23:47 — A megállított idő</h2>
          <p>{{ state.storyIntro }}</p>
        </article>
        @if (state.taskPrompt) {
          <article class="task-panel">
            <span>Első feladat</span>
            <h2>A három vonal kódja</h2>
            <p>{{ state.taskPrompt }}</p>
            <form [formGroup]="answerForm" (ngSubmit)="submitAnswer()">
              <label>Megfejtés<input formControlName="answer" autocomplete="off" placeholder="Írd be a választ"></label>
              <button class="button primary" type="submit" [disabled]="submitting()">{{ submitting() ? 'Ellenőrzés…' : 'Válasz elküldése' }}</button>
            </form>
          </article>
        }
        @if (state.completionText) {
          <article class="completion-panel"><span>✓ Küldetés teljesítve</span><h2>Az idő újra elindult</h2><p>{{ state.completionText }}</p><a class="button primary" routerLink="/missions">Új küldetés választása</a></article>
        }
      }
      @if (message()) { <p class="form-message" role="alert">{{ message() }}</p> }
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GamePage {
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
