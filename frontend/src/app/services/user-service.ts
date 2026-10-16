import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Auth } from './auth';
import { environment } from '../../environments/environment.development';
import { map, Observable, of, switchMap } from 'rxjs';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);
  private auth = inject(Auth);

  users() {
    return this.http.get<User[]>(`${environment.apiUrl}/users`);
  }

  update(id: number, email: string, password?: string, role?: string) {
    const isSelf = this.auth.user?.id === id;
    const refreshAfter = isSelf && !password;

    return this.http
      .put(`${environment.apiUrl}/users/${id}`, {
        email,
        ...(password && { password: password }),
        ...(role && { role: role }),
      })
      .pipe(
        switchMap((res) => (refreshAfter ? this.auth.refresh().pipe(map(() => res)) : of(res))),
      );
  }
}
