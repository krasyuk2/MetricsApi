import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MetricsApi, ValueRecord } from '../api/metrics-api';
import { extractErrors } from '../api/problem-details';

@Component({
  selector: 'app-latest',
  imports: [FormsModule, DatePipe, DecimalPipe],
  templateUrl: './latest.html',
})
export class Latest {
  private readonly api = inject(MetricsApi);

  fileName = '';

  readonly values = signal<ValueRecord[]>([]);
  readonly loading = signal(false);
  readonly loaded = signal(false);
  readonly errors = signal<string[]>([]);

  load(): void {
    const name = this.fileName.trim();
    if (!name) return;

    this.loading.set(true);
    this.errors.set([]);
    this.values.set([]);

    this.api.getLatestValues(name).subscribe({
      next: values => {
        this.values.set(values);
        this.loaded.set(true);
        this.loading.set(false);
      },
      error: err => {
        this.loaded.set(true);
        this.loading.set(false);
        this.errors.set(extractErrors(err));
      },
    });
  }
}
