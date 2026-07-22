import { Component, inject, signal } from '@angular/core';
import { MetricsApi } from '../api/metrics-api';
import { extractErrors } from '../api/problem-details';

type UploadState = 'idle' | 'loading' | 'success' | 'error';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.html',
})
export class Upload {
  private readonly api = inject(MetricsApi);

  readonly file = signal<File | null>(null);
  readonly state = signal<UploadState>('idle');
  readonly errors = signal<string[]>([]);
  readonly uploadedName = signal('');

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.file.set(input.files?.[0] ?? null);
    this.state.set('idle');
    this.errors.set([]);
  }

  upload(): void {
    const file = this.file();
    if (!file) return;

    this.state.set('loading');
    this.errors.set([]);

    this.api.uploadCsv(file).subscribe({
      next: () => {
        this.state.set('success');
        this.uploadedName.set(file.name);
      },
      error: err => {
        this.state.set('error');
        this.errors.set(extractErrors(err));
      },
    });
  }
}
