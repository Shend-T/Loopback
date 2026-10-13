import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Home } from './pages/home/home';

import { guestGuard } from './guards/guest-guard';
import { UserDashboard } from './pages/user-dashboard/user-dashboard';
import { authGuard } from './guards/auth-guard';
import { MainLayout } from './layouts/main-layout/main-layout';
import { Register } from './pages/register/register';
import { AcceptInvite } from './pages/accept-invite/accept-invite';
import { CreateInvite } from './pages/create-invite/create-invite';
import { adminGuard } from './guards/admin-guard';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],
    children: [
      { path: '', component: Home },
      { path: 'dashboard', component: UserDashboard },
      { path: 'invite', component: CreateInvite, canActivate: [adminGuard] },
    ],
  },
  { path: 'accept-invite', component: AcceptInvite, canActivate: [guestGuard] },
  { path: 'register', component: Register, canActivate: [guestGuard] },
  { path: 'login', component: Login, canActivate: [guestGuard] },
];
