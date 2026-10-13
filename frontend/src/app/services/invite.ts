import { inject, Injectable } from '@angular/core';
import { Auth } from './auth';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { map, tap } from 'rxjs';
import { InviteUser } from '../models/inviteUser';

@Injectable({
  providedIn: 'root',
})
export class Invite {
  private auth = inject(Auth);
  private http = inject(HttpClient);

  sendInvite(email: string, role: string) {
    return this.http
      .post<{ token: string }>(`${environment.apiUrl}/invite`, {
        email,
        role,
      })
      .pipe(map((res) => res.token));
  }

  check(token: string) {
    return this.http.post<InviteUser>(`${environment.apiUrl}/invite/check`, { token });
  }
}
