import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OrganizationList } from '../models/organization-list';

@Injectable({
  providedIn: 'root',
})
export class OrganizationService {
  private http = inject(HttpClient);

  get organizations(): Observable<OrganizationList[]> {
    return this.http.get<OrganizationList[]>(`${environment.apiUrl}/organizations`);
  }
}
