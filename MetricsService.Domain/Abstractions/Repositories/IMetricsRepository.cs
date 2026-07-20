using MetricsApi.Domain.Models;

namespace MetricsApi.Domain.Abstractions.Repositories;

/// <summary>
///     Репозиторий работы с метрирой.
/// </summary>
public interface IMetricsRepository
{
    Task ReplaceFileDataAsync(string fileName, IReadOnlyList<ValueRecord> values, ResultMetric result,
        CancellationToken ct);
    
    Task<IReadOnlyList<ResultMetric>> GetResultsAsync(ResultFilter filter, CancellationToken ct);
    
    Task<IReadOnlyList<ValueRecord>> GetLatestValuesAsync(string fileName, int count, CancellationToken ct);
}