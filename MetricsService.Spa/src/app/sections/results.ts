import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MetricsApi, ResultMetric, ResultsFilter } from '../api/metrics-api';
import { extractErrors } from '../api/problem-details';

@Component({
  selector: 'app-results',
  imports: [FormsModule, DatePipe, DecimalPipe],
  templateUrl: './results.html',
})
export class Results {
  private readonly api = inject(MetricsApi);

  readonly filter: ResultsFilter = {
    fileName: '',
    firstStartTimeFrom: '',
    firstStartTimeTo: '',
    valueMeanFrom: null,
    valueMeanTo: null,
    avgDurationSecondsFrom: null,
    avgDurationSecondsTo: null,
  };

  readonly results = signal<ResultMetric[]>([]);
  readonly loading = signal(false);
  readonly loaded = signal(false);
  readonly errors = signal<string[]>([]);

  search(): void {
    this.loading.set(true);
    this.errors.set([]);

    this.api.getResults(this.filter).subscribe({
      next: results => {
        this.results.set(results);
        this.loaded.set(true);
        this.loading.set(false);
      },
      error: err => {
        this.errors.set(extractErrors(err));
        this.loading.set(false);
      },
    });
  }

  reset(): void {
    this.filter.fileName = '';
    this.filter.firstStartTimeFrom = '';
    this.filter.firstStartTimeTo = '';
    this.filter.valueMeanFrom = null;
    this.filter.valueMeanTo = null;
    this.filter.avgDurationSecondsFrom = null;
    this.filter.avgDurationSecondsTo = null;
    this.search();
  }
}
