import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Home } from './pages/home/home';

import { guestGuard } from './guards/guest-guard';
import { UserDashboard } from './pages/user-dashboard/user-dashboard';
import { authGuard } from './guards/auth-guard';
import { MainLayout } from './layouts/main-layout/main-layout';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],
    children: [{ path: '', component: Home }],
  },
  { path: 'login', component: Login, canActivate: [guestGuard] },
  { path: 'dashboard', component: UserDashboard, canActivate: [authGuard] },
];
