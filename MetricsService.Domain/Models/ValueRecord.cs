namespace MetricsApi.Domain.Models;

/// <summary>
///     Модель значений из файла CSV.
/// </summary>
public class ValueRecord
{
    /// <summary>
    ///     Идентификатор.
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    ///     Имя файла.
    /// </summary>
    public required string FileName { get; set; }
    
    /// <summary>
    ///   Время начала (ГГГГ-ММ-ДДTчч-мм-сс.ммммZ).
    /// </summary>
    public required DateTime StartTime {get; set;}
    
    /// <summary>
    ///     Время выполнения в секундах.
    /// </summary>
    public required TimeSpan Duration {get; set;}
    
    /// <summary>
    ///     Показатель в виде числа с плавающей запятой
    /// </summary>
    public required double Value { get; set; }
}