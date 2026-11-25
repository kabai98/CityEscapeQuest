import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./pages/home.page').then(m => m.HomePage), title: 'City Escape Quest' },
  { path: 'missions', loadComponent: () => import('./pages/missions.page').then(m => m.MissionsPage), title: 'Küldetések · City Escape Quest' },
  { path: 'missions/:id', loadComponent: () => import('./pages/mission-details.page').then(m => m.MissionDetailsPage), title: 'Küldetés · City Escape Quest' },
  { path: 'lobby/:id', loadComponent: () => import('./pages/lobby.page').then(m => m.LobbyPage), title: 'Csapatlobby · City Escape Quest' },
  { path: 'game/:id', loadComponent: () => import('./pages/game.page').then(m => m.GamePage), title: 'Futó küldetés · City Escape Quest' },
  { path: 'login', loadComponent: () => import('./pages/login.page').then(m => m.LoginPage), title: 'Belépés · City Escape Quest' },
  { path: 'profile', loadComponent: () => import('./pages/profile.page').then(m => m.ProfilePage), title: 'Profil · City Escape Quest' },
  { path: '**', redirectTo: '' }
];
