import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { AuthCard } from '../../components/auth-card/auth-card';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink, AuthCard],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private fb = inject(FormBuilder);
  private auth = inject(Auth);
  private router = inject(Router);

  error = signal<string | null>(null);
  loading = false;

  form = this.fb.nonNullable.group({
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

    const { email, password } = this.form.getRawValue();

    this.auth.login(email, password).subscribe({
      next: () => this.router.navigate(['/']),
      error: (e) => {
        this.loading = false;
        this.error.set(e.status === 401 ? 'Invalid email or password' : 'Something went wrong');
      },
    });
  }
}
