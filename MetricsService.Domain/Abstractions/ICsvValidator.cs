using MetricsApi.Domain.DTOs;

namespace MetricsApi.Domain.Abstractions;

/// <summary>
///     Сервис валидации строк csv.
/// </summary>
public interface ICsvValidator
{
    IReadOnlyList<string> Validate(IReadOnlyList<CsvRow> rows);
}