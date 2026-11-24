import { Routes } from '@angular/router';
import { HomePage, LoginPage, MissionDetailsPage, MissionsPage, ProfilePage } from './pages/pages';

export const routes: Routes = [
  { path: '', component: HomePage, title: 'City Escape Quest' },
  { path: 'missions', component: MissionsPage, title: 'Küldetések · City Escape Quest' },
  { path: 'missions/:id', component: MissionDetailsPage, title: 'Küldetés · City Escape Quest' },
  { path: 'login', component: LoginPage, title: 'Belépés · City Escape Quest' },
  { path: 'profile', component: ProfilePage, title: 'Profil · City Escape Quest' },
  { path: '**', redirectTo: '' }
];
