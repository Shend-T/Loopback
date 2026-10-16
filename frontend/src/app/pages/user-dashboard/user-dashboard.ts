import { afterNextRender, Component, inject, signal } from '@angular/core';
import { User } from '../../models/user';
import { Auth } from '../../services/auth';
import { UserService } from '../../services/user-service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-dashboard',
  imports: [RouterLink],
  templateUrl: './user-dashboard.html',
  styleUrl: './user-dashboard.css',
})
export class UserDashboard {
  private userService = inject(UserService);
  private auth = inject(Auth);
  private router = inject(Router);

  userList: User[] = [];
  loading = signal(true);
  error = signal<string | null>(null);
  currentUserId = this.auth.user?.id;

  constructor() {
    this.userService.users().subscribe({
      next: (res) => {
        this.userList = res;
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load this page');
        this.loading.set(false);
      },
    });
  }

  editModal = false;
  openEdit(user: User) {
    this.editModal = !this.editModal;
  }
  deleteModal = false;
  openDelete(user: User) {
    this.deleteModal = !this.deleteModal;
  }
}
