import { Component, inject, signal } from '@angular/core';
import { Auth } from '../../services/auth';
import { OrganizationService } from '../../services/organization-service';
import { Organization } from '../../models/organization';
import { User } from '../../models/user';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../services/user-service';

@Component({
  selector: 'app-profile',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile {
  private auth = inject(Auth);
  private userService = inject(UserService);
  private orgService = inject(OrganizationService);

  private fb = inject(FormBuilder);
  private router = inject(Router);

  user = signal<User | null>(null);
  org = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: [''],
  });

  formPassword = this.fb.nonNullable.group({
    password: ['', [Validators.required]],
  });

  isEdit = false;
  editModal = false;
  logoutModal = false;

  constructor() {
    const _user = this.auth.user;
    if (!_user) return;

    this.user.set(_user);
    this.orgService.getOrganization(_user.organizationId).subscribe({
      next: (org) => this.org.set(org.name),
      error: (e) => console.log(e),
    });

    this.form.patchValue({ email: _user.email });
  }

  toggleEdit() {
    this.isEdit = !this.isEdit;
  }

  edit() {
    this.editModal = !this.editModal;
  }

  editConfirm() {
    console.log(this.form.getRawValue());
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const user = this.user();
    if (!user) return;

    const { email, password } = this.form.getRawValue();
    this.userService.update(user.id, email, password).subscribe({
      next: () => {
        if (password) {
          this.auth.logout();
          this.router.navigate(['/login']);
          return;
        }
        this.user.set(this.auth.user);
        this.form.patchValue({ password: '' });
        this.isEdit = false;
        this.editModal = false;
      },
      error: (e) => console.log(e),
    });
  }

  logoutConfirm() {
    this.logoutModal = !this.logoutModal;
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
