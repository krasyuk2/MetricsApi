namespace MetricsApi.DTOs;

/// <summary>
///     Модель ответа с интегральными результатами файла.
/// </summary>
/// <param name="FileName"> Имя файла. </param>
/// <param name="DeltaTimeSeconds"> Дельта времени Date в секундах (максимальное минус минимальное). </param>
/// <param name="FirstStartTime"> Момент запуска первой операции. </param>
/// <param name="AvgDurationSeconds"> Среднее время выполнения в секундах. </param>
/// <param name="ValueMean"> Среднее значение показателя. </param>
/// <param name="ValueMedian"> Медиана показателя. </param>
/// <param name="ValueMin"> Минимальное значение показателя. </param>
/// <param name="ValueMax"> Максимальное значение показателя. </param>
public record ResultMetricResponse(
    string FileName,
    double DeltaTimeSeconds,
    DateTime FirstStartTime,
    double AvgDurationSeconds,
    double ValueMean,
    double ValueMedian,
    double ValueMin,
    double ValueMax);