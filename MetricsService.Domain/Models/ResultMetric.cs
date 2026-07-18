namespace MetricsApi.Domain.Models;

/// <summary>
///     Модель - интегральные результаты.
/// </summary>
public class ResultMetric
{
    /// <summary>
    ///     Наименование и PK.
    /// </summary>
    public required string FileName { get; set; }
    
    /// <summary>
    ///     Дельта времени Date.
    /// </summary>
    public required TimeSpan DeltaTime { get; set; }
    
    /// <summary>
    ///     Минимальное дата и время, как момент запуска первой операции.
    /// </summary>
    public required DateTime FirstStartTime { get; set; }
    
    /// <summary>
    ///     Среднее время выполнения.
    /// </summary>
    public required TimeSpan AvgDuration { get; set; }
    
    /// <summary>
    ///     Среднее значение по показателям.
    /// </summary>
    public required double ValueMean { get; set; }
    
    /// <summary>
    ///     Медиана по показателям.
    /// </summary>
    public required double ValueMedian { get; set; }
    
    /// <summary>
    ///     Минимальное значение показателя.
    /// </summary>
    public required double ValueMin { get; set; }
    
    /// <summary>
    ///     Максимальное значение показателя.
    /// </summary>
    public required double ValueMax { get; set; }
}