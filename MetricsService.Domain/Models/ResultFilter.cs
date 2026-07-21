namespace MetricsApi.Domain.Models;

/// <summary>
///     Фильтр выборки интегральных результатов.
/// </summary>
/// <param name="FileName"> Имя файла, null — без фильтра. </param>
/// <param name="FirstStartTimeFrom"> Нижняя граница времени запуска первой операции. </param>
/// <param name="FirstStartTimeTo"> Верхняя граница времени запуска первой операции. </param>
/// <param name="ValueMeanFrom"> Нижняя граница среднего показателя. </param>
/// <param name="ValueMeanTo"> Верхняя граница среднего показателя. </param>
/// <param name="AvgDurationSecondsFrom"> Нижняя граница среднего времени выполнения в секундах. </param>
/// <param name="AvgDurationSecondsTo"> Верхняя граница среднего времени выполнения в секундах. </param>
public record ResultFilter(
    string? FileName,
    DateTime? FirstStartTimeFrom,
    DateTime? FirstStartTimeTo,
    double? ValueMeanFrom,
    double? ValueMeanTo,
    double? AvgDurationSecondsFrom,
    double? AvgDurationSecondsTo);