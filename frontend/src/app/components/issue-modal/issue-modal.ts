import { Component, inject, output } from '@angular/core';
import { IssueService } from '../../services/issue-service';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-issue-modal',
  imports: [ReactiveFormsModule],
  templateUrl: './issue-modal.html',
  styleUrl: './issue-modal.css',
})
export class IssueModal {
  private issueService = inject(IssueService);
  private fb = inject(FormBuilder);

  form = this.fb.nonNullable.group({
    title: ['', [Validators.required]],
    description: ['', [Validators.required]],
    severity: ['', [Validators.required]],
  });

  closed = output<void>();
  created = output<void>();

  close() {
    this.closed.emit();
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { title, description, severity } = this.form.getRawValue();
    console.log(title);

    this.issueService.createIssue(title, description, severity).subscribe(() => {
      this.created.emit();
      this.close();
    });
  }
}
