import { HttpErrorResponse } from '@angular/common/http';

/**
 * Превращает ответ об ошибке API (ValidationProblemDetails / ProblemDetails)
 * в плоский список сообщений для вывода пользователю.
 */
export function extractErrors(err: unknown): string[] {
  if (err instanceof HttpErrorResponse) {
    const body = err.error;

    if (body && typeof body === 'object') {
      if ('errors' in body && body.errors && typeof body.errors === 'object') {
        const messages = Object.values(body.errors as Record<string, string[]>).flat();
        if (messages.length > 0) return messages;
      }
      if (typeof body.detail === 'string') return [body.detail];
      if (typeof body.title === 'string') return [body.title];
    }
    if (typeof body === 'string' && body.length > 0) return [body];

    if (err.status === 0) return ['Сервер недоступен. Проверьте, что API запущено.'];
    return [`Ошибка ${err.status}: ${err.statusText}`];
  }
  return ['Неизвестная ошибка.'];
}
