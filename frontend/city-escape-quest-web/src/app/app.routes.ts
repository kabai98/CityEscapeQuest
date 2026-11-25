import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./features/home/home').then(m => m.Home), title: 'City Escape Quest' },
  { path: 'missions', loadComponent: () => import('./features/missions/mission-list').then(m => m.MissionList), title: 'Küldetések · City Escape Quest' },
  { path: 'missions/:id', loadComponent: () => import('./features/missions/mission-details').then(m => m.MissionDetails), title: 'Küldetés · City Escape Quest' },
  { path: 'lobby/:id', loadComponent: () => import('./features/lobby/lobby').then(m => m.Lobby), title: 'Csapatlobby · City Escape Quest' },
  { path: 'game/:id', loadComponent: () => import('./features/game/game').then(m => m.Game), title: 'Futó küldetés · City Escape Quest' },
  { path: 'login', loadComponent: () => import('./features/login/login').then(m => m.Login), title: 'Belépés · City Escape Quest' },
  { path: 'profile', loadComponent: () => import('./features/profile/profile').then(m => m.Profile), title: 'Profil · City Escape Quest' },
  { path: '**', redirectTo: '' }
];
