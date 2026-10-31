import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { Auth } from '../../services/auth';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { IssueService } from '../../services/issue-service';
import { Issue } from '../../models/issue';
import { User } from '../../models/user';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  private auth = inject(Auth);
  private issueService = inject(IssueService);
  private router = inject(Router);
  private http = inject(HttpClient);

  token = this.auth.token;
  loggedIn = this.auth.isLoggedIn();

  user = signal<User | null>(null);
  issueList = signal<Issue[]>([]);
  loading = signal(true);

  constructor() {
    const _user = this.auth.user;
    if (!_user) return;

    this.user.set(_user);

    this.issueService.issues.subscribe({
      next: (res) => {
        this.issueList.set(res);
        this.loading.set(false);
        console.log('Good');
      },
      error: () => {
        this.loading.set(false);
        console.log('Error');
      },
    });
  }
}
