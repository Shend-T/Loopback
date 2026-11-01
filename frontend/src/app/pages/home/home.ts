import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { Auth } from '../../services/auth';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { IssueService } from '../../services/issue-service';
import { Issue } from '../../models/issue';
import { User } from '../../models/user';
import { IssueModal } from '../../components/issue-modal/issue-modal';

@Component({
  selector: 'app-home',
  imports: [IssueModal],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  private auth = inject(Auth);
  private issueService = inject(IssueService);

  token = this.auth.token;
  loggedIn = this.auth.isLoggedIn();

  user = signal<User | null>(null);
  issueList = signal<Issue[]>([]);
  loading = signal(true);

  showModal = signal(false);

  constructor() {
    const _user = this.auth.user;
    if (_user) this.user.set(_user);
  }

  ngOnInit() {
    if (!this.user()) return;
    this.loadIssues();
  }

  loadIssues() {
    this.issueService.issues.subscribe({
      next: (res) => {
        this.issueList.set(res);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }
}
