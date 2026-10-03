import { afterNextRender, Component, inject, signal } from '@angular/core';
import { User } from '../../models/user';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-user-dashboard',
  imports: [],
  templateUrl: './user-dashboard.html',
  styleUrl: './user-dashboard.css',
})
export class UserDashboard {
  private auth = inject(Auth);
  user = signal<User | null>(null);

  constructor() {
    afterNextRender(() => this.user.set(this.auth.user));
  }
}
