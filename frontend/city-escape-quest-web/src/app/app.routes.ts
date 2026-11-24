import { Routes } from '@angular/router';
import { HomePage, LoginPage, MissionsPage, ProfilePage } from './pages/pages';
import { MissionDetailsPage } from './pages/mission-details.page';
import { LobbyPage } from './pages/lobby.page';
import { GamePage } from './pages/game.page';

export const routes: Routes = [
  { path: '', component: HomePage, title: 'City Escape Quest' },
  { path: 'missions', component: MissionsPage, title: 'Küldetések · City Escape Quest' },
  { path: 'missions/:id', component: MissionDetailsPage, title: 'Küldetés · City Escape Quest' },
  { path: 'lobby/:id', component: LobbyPage, title: 'Csapatlobby · City Escape Quest' },
  { path: 'game/:id', component: GamePage, title: 'Futó küldetés · City Escape Quest' },
  { path: 'login', component: LoginPage, title: 'Belépés · City Escape Quest' },
  { path: 'profile', component: ProfilePage, title: 'Profil · City Escape Quest' },
  { path: '**', redirectTo: '' }
];
