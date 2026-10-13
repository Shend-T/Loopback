import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { User } from '../../models/user';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar {
  private auth = inject(Auth);
  private router = inject(Router);

  user = signal<User | null>(this.auth.user);
  isAdmin = computed(() => this.user()?.role === 'Admin');

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
