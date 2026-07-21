namespace MetricsApi.DTOs;

/// <summary>
///     Модель ответа со значением из csv файла.
/// </summary>
/// <param name="StartTime"> Время начала операции. </param>
/// <param name="DurationSeconds"> Время выполнения в секундах. </param>
/// <param name="Value"> Показатель в виде числа с плавающей запятой. </param>
public record ValueRecordResponse(
    DateTime StartTime,
    double DurationSeconds,
    double Value);