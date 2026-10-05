import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { OrganizationService } from '../../services/organization-service';
import { AuthCard } from '../../components/auth-card/auth-card';
import { OrganizationList } from '../../models/organization-list';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink, AuthCard],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login implements OnInit {
  private fb = inject(FormBuilder);
  private auth = inject(Auth);
  private router = inject(Router);
  private org = inject(OrganizationService);

  organizationList: OrganizationList[] = [];

  ngOnInit(): void {
    this.org.organizations.subscribe((organizations) => {
      this.organizationList = organizations;
    });
  }

  error = signal<string | null>(null);
  loading = false;

  form = this.fb.nonNullable.group({
    organizationName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error.set(null);

    const { organizationName, email, password } = this.form.getRawValue();

    this.auth.login(organizationName, email, password).subscribe({
      next: () => this.router.navigate(['/']),
      error: (e) => {
        this.loading = false;
        this.error.set(e.status === 401 ? 'Invalid email or password' : 'Something went wrong');
      },
    });
  }
}
