using MetricsApi.Domain.DTOs;

namespace MetricsApi.Domain.Abstractions;

/// <summary>
///     Сервис валидации строк csv.
/// </summary>
public interface ICsvValidator
{
    /// <summary>
    ///     Проверяет строки на соответствие бизнес-правилам ТЗ.
    /// </summary>
    /// <param name="rows"> Строки, полученные после парсинга csv. </param>
    /// <returns> Список ошибок валидации, пустой если все строки корректны. </returns>
    IReadOnlyList<string> Validate(IReadOnlyList<CsvRow> rows);
}