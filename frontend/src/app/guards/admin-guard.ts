import { inject, PLATFORM_ID } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Auth } from '../services/auth';
import { isPlatformBrowser } from '@angular/common';

export const adminGuard: CanActivateFn = (route, state) => {
  if (!isPlatformBrowser(inject(PLATFORM_ID))) return true;

  const auth = inject(Auth);
  const router = inject(Router);

  return auth.user?.role === 'Admin' ? true : router.createUrlTree(['/']);
};
