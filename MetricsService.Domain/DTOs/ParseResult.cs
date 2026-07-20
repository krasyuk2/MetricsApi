namespace MetricsApi.Domain.DTOs;

/// <summary>
///     Модель после парсинга файла.
/// </summary>
/// <param name="Rows"> Строки который он содержит. </param>
/// <param name="Errors"> Ошибки при парсинге. (копим ошибки и кидаем все разом, а не по одной) </param>
public record ParseResult(List<CsvRow> Rows, List<string> Errors);