import { HttpClient } from '@angular/common/http';
import { inject, Injectable, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../environments/environment.development';
import { finalize, Observable, shareReplay, tap } from 'rxjs';

interface LoginResponse {
  accessToken: string;
  refreshToken: string;
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
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
  }
}
