import { HttpClient } from '@angular/common/http';
import { inject, Injectable, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../environments/environment.development';
import { finalize, Observable, shareReplay, tap } from 'rxjs';

import { User } from '../models/user';

interface LoginResponse {
  accessToken: string;
  refreshToken: string;
}

const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

function decodeJwt(token: string): Record<string, any> | null {
  try {
    const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    const bytes = Uint8Array.from(atob(base64), (c) => c.charCodeAt(0));
    return JSON.parse(new TextDecoder().decode(bytes));
  } catch {
    return null;
  }
}

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private http = inject(HttpClient);
  private isBrowser = isPlatformBrowser(inject(PLATFORM_ID));

  private refresh$: Observable<LoginResponse> | null = null;

  private saveTokens(res: LoginResponse) {
    localStorage.setItem('access_token', res.accessToken);
    localStorage.setItem('refresh_token', res.refreshToken);
  }

  login(email: string, password: string) {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, { email, password })
      .pipe(tap((res) => this.saveTokens(res)));
  }

  refresh() {
    if (!this.refresh$) {
      this.refresh$ = this.http
        .post<LoginResponse>(`${environment.apiUrl}/auth/refresh`, {
          refreshToken: this.refreshToken,
        })
        .pipe(
          tap((res) => this.saveTokens(res)),
          finalize(() => (this.refresh$ = null)),
          shareReplay(1),
        );
    }
    return this.refresh$;
  }

  get token() {
    return this.isBrowser ? localStorage.getItem('access_token') : null;
  }
  get refreshToken() {
    return this.isBrowser ? localStorage.getItem('refresh_token') : null;
  }
  isLoggedIn() {
    return !!this.refreshToken;
  }

  logout() {
    const refreshToken = this.refreshToken;

    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');

    if (refreshToken) {
      this.http
        .post(`${environment.apiUrl}/auth/logout`, { refreshToken })
        .subscribe({ error: () => {} });
    }
  }

  get user(): User | null {
    const token = this.token;
    const claims = token ? decodeJwt(token) : null;
    if (!claims) return null;

    return {
      id: Number(claims['sub']),
      email: claims['email'],
      organizationId: Number(claims['organizationId']),
      role: claims['role'] ?? claims[ROLE_CLAIM],
    };
  }
}
