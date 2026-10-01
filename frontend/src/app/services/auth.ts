import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { tap } from 'rxjs';

interface LoginResposne {
  accessToken: string;
  refreshToken: string;
}

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private htpp = inject(HttpClient);

  login(email: string, password: string) {
    return this.htpp
      .post<LoginResposne>(`${environment.apiUrl}/auth/login`, { email, password })
      .pipe(
        tap((res) => {
          localStorage.setItem('access_token', res.accessToken);
          localStorage.setItem('refresh_token', res.refreshToken);
        }),
      );
  }

  get token() {
    return localStorage.getItem('access_token');
  }
  isLoggedIn() {
    return !!this.token;
  }

  logout() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
  }
}
