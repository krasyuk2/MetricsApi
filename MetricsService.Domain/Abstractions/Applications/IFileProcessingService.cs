using MetricsApi.Domain.DTOs;

namespace MetricsApi.Domain.Abstractions.Applications;

/// <summary>
///     Сервис обработки csv файла: парсинг, валидация, расчёт метрик и сохранение.
/// </summary>
public interface IFileProcessingService
{
    /// <summary>
    ///     Обрабатывает csv файл и при успешной валидации сохраняет значения и результаты в БД.
    /// </summary>
    /// <param name="fileName"> Имя загруженного файла. </param>
    /// <param name="csvStream"> Поток с содержимым csv файла. </param>
    /// <param name="ct"> Токен отмены операции. </param>
    /// <returns> Результат обработки: успех либо список ошибок. </returns>
    Task<ProcessingResult> ProcessAsync(string fileName, Stream csvStream, CancellationToken ct);
}