import { afterNextRender, Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { AuthCard } from '../../components/auth-card/auth-card';

import { InviteUser } from '../../models/inviteUser';

import { Auth } from '../../services/auth';
import { Invite } from '../../services/invite';

const passwordsMatch: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
  const password = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  return password === confirm ? null : { passwordsMismatch: true };
};

@Component({
  selector: 'app-accept-invite',
  imports: [ReactiveFormsModule, AuthCard],
  templateUrl: './accept-invite.html',
  styleUrl: './accept-invite.css',
})
export class AcceptInvite {
  private auth = inject(Auth);
  private invite = inject(Invite);
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  private token = this.route.snapshot.queryParamMap.get('token');

  info = signal<InviteUser | null>(null);

  form = this.fb.nonNullable.group(
    {
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(72)]],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: passwordsMatch },
  );

  constructor() {
    afterNextRender(() => {
      if (!this.token) {
        return;
      }
      this.invite.check(this.token).subscribe({
        next: (i) => {
          this.info.set(i);
          console.log(this.token);
        },
        error: () => console.log('error'),
      });
    });
  }

  submit() {
    if (this.form.invalid || !this.token) {
      this.form.markAllAsTouched();
      return;
    }

    this.auth.acceptInvite(this.token, this.form.getRawValue().password).subscribe({
      next: () => this.router.navigate(['/']),
      error: (e) => {
        console.log('error');
      },
    });
  }
}
