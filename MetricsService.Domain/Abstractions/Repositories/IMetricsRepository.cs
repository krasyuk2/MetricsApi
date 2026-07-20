using MetricsApi.Domain.Models;

namespace MetricsApi.Domain.Abstractions.Repositories;

/// <summary>
///     Репозиторий работы с метрирой.
/// </summary>
public interface IMetricsRepository
{
    /// <summary>
    ///     
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="values"></param>
    /// <param name="result"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task ReplaceFileDataAsync(string fileName, IReadOnlyList<ValueRecord> values, ResultMetric result, 
        CancellationToken ct);
}