import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { Auth } from '../../services/auth';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  private auth = inject(Auth);
  private router = inject(Router);

  token = this.auth.token;
  loggedIn = this.auth.isLoggedIn();

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  private http = inject(HttpClient);

  testRefresh() {
    this.auth.refresh().subscribe({
      next: () => console.log('Refreshed OK'),
      error: (err) => console.log('Refresh FAILED', err.status),
    });
  }
}
