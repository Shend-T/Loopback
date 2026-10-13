import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Organization } from '../models/organization';

@Injectable({
  providedIn: 'root',
})
export class OrganizationService {
  private http = inject(HttpClient);

  get organizations(): Observable<Organization[]> {
    return this.http.get<Organization[]>(`${environment.apiUrl}/organizations`);
  }
}
