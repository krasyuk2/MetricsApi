namespace MetricsApi.Domain.DTOs;

/// <summary>
///     Модель DTO для csv строки.
/// </summary>
/// <param name="StartTime"> Время начала ГГГГ-ММ-ДДTчч-мм-сс.ммммZ. </param>
/// <param name="Duration"> Время выполнения в секундах. </param>
/// <param name="Value"> Показатель в виде числа с плавающей запятой. </param>
public record CsvRow(DateTime StartTime, TimeSpan Duration, double Value);
