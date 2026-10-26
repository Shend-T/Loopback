import { afterNextRender, Component, inject, signal } from '@angular/core';
import { User } from '../../models/user';
import { Auth } from '../../services/auth';
import { UserService } from '../../services/user-service';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-user-dashboard',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './user-dashboard.html',
  styleUrl: './user-dashboard.css',
})
export class UserDashboard {
  private userService = inject(UserService);
  private auth = inject(Auth);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  userList: User[] = [];
  loading = signal(true);
  error = signal<string | null>(null);
  currentUserId = this.auth.user?.id;
  selectedUser = signal<User | null>(null);

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

  editForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: [''],
    role: ['', [Validators.required]],
  });

  editModal = false;
  openEdit(user: User) {
    this.editModal = !this.editModal;
    this.selectedUser.set(user);
    this.editForm.patchValue({
      email: user.email,
      role: user.role,
    });
  }
  closeEdit() {
    this.editModal = false;
    this.selectedUser.set(null);
    this.editForm.reset();
  }
  confirmEdit() {
    const { email, password, role } = this.editForm.getRawValue();
    const user = this.selectedUser();
    if (!user) return;

    this.userService.update(user.id, email, password, role).subscribe({
      next: () => {
        this.editModal = false;
        this.selectedUser.set(null);
        this.editForm.reset();
      },
      error: (e) => console.log(e),
    });
  }

  deleteForm = this.fb.nonNullable.group({
    text: ['', [Validators.required, Validators.pattern(/^DELETE$/)]],
  });

  deleteModal = false;
  openDelete(user: User) {
    this.deleteModal = !this.deleteModal;
    this.selectedUser.set(user);
  }
  closeDelete() {
    this.deleteForm.reset();
    this.deleteModal = false;
    this.selectedUser.set(null);
  }
  confirmDelete() {
    if (this.deleteForm.invalid) return;

    const user = this.selectedUser();
    if (!user) return;

    this.userService.delete(user.id).subscribe({
      next: () => {
        this.userList = this.userList.filter((u) => u.id !== user.id);
        this.closeDelete();
      },
      error: (e) => console.log(e.error?.message),
    });
  }
}
