using MetricsApi.Domain.DTOs;

namespace MetricsApi.Domain.Abstractions;

/// <summary>
///     Сервис парсинга файла csv.
/// </summary>
public interface ICsvParser
{
    ParseResult Parse(Stream csvStream);
}