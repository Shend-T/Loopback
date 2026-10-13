import { Component, inject, signal } from '@angular/core';
import { Auth } from '../../services/auth';
import { Invite } from '../../services/invite';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthCard } from '../../components/auth-card/auth-card';

@Component({
  selector: 'app-create-invite',
  imports: [ReactiveFormsModule, AuthCard],
  templateUrl: './create-invite.html',
  styleUrl: './create-invite.css',
})
export class CreateInvite {
  private auth = inject(Auth);
  private invite = inject(Invite);
  private fb = inject(FormBuilder);

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    role: ['', [Validators.required]],
  });
  link = signal<string | null>(null);

  copied = signal(false);

  async copy(url: string) {
    try {
      await navigator.clipboard.writeText(url);
      this.copied.set(true);
      setTimeout(() => this.copied.set(false), 2000);
    } catch {
      // clipboard can fail (permissions, non-HTTPS); the input is still selectable
    }
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, role } = this.form.getRawValue();
    this.invite.sendInvite(email, role).subscribe({
      next: (l) => this.link.set(l),
      error: (e) => console.log(e),
    });
  }
}
