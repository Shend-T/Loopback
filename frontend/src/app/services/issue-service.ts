import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Issue } from '../models/issue';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class IssueService {
  private http = inject(HttpClient);

  get issues(): Observable<Issue[]> {
    return this.http.get<Issue[]>(`${environment.apiUrl}/issue`);
  }
}
