import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

/** Интегральные результаты файла (таблица Results). */
export interface ResultMetric {
  fileName: string;
  deltaTimeSeconds: number;
  firstStartTime: string;
  avgDurationSeconds: number;
  valueMean: number;
  valueMedian: number;
  valueMin: number;
  valueMax: number;
}

/** Строка значений файла (таблица Values). */
export interface ValueRecord {
  startTime: string;
  durationSeconds: number;
  value: number;
}

/** Фильтры выборки результатов. Все поля опциональны. */
export interface ResultsFilter {
  fileName?: string;
  firstStartTimeFrom?: string;
  firstStartTimeTo?: string;
  valueMeanFrom?: number | null;
  valueMeanTo?: number | null;
  avgDurationSecondsFrom?: number | null;
  avgDurationSecondsTo?: number | null;
}

@Injectable({ providedIn: 'root' })
export class MetricsApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/metrics';

  uploadCsv(file: File): Observable<void> {
    const form = new FormData();
    form.append('csvFile', file, file.name);
    return this.http.post<void>(`${this.baseUrl}/files`, form);
  }

  getResults(filter: ResultsFilter): Observable<ResultMetric[]> {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(filter)) {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(capitalize(key), String(value));
      }
    }
    return this.http.get<ResultMetric[]>(`${this.baseUrl}/results`, { params });
  }

  getLatestValues(fileName: string): Observable<ValueRecord[]> {
    return this.http.get<ValueRecord[]>(
      `${this.baseUrl}/values/${encodeURIComponent(fileName)}/latest`);
  }
}

/** Имена query-параметров на бэкенде — в PascalCase. */
function capitalize(key: string): string {
  return key.charAt(0).toUpperCase() + key.slice(1);
}
