import { Component, inject, signal } from '@angular/core';
import { Auth } from '../../services/auth';
import { OrganizationService } from '../../services/organization-service';
import { Organization } from '../../models/organization';
import { User } from '../../models/user';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-profile',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile {
  private auth = inject(Auth);
  private orgService = inject(OrganizationService);

  private fb = inject(FormBuilder);
  private router = inject(Router);

  user = signal<User | null>(null);
  org = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: [''],
  });

  isEdit = false;
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
    console.log(this.form);
  }

  logoutConfirm() {
    this.logoutModal = !this.logoutModal;
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
