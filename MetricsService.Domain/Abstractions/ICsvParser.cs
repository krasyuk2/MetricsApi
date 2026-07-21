using MetricsApi.Domain.DTOs;

namespace MetricsApi.Domain.Abstractions;

/// <summary>
///     Сервис парсинга файла csv.
/// </summary>
public interface ICsvParser
{
    /// <summary>
    ///     Читает поток csv и превращает его в набор строк и ошибок парсинга.
    /// </summary>
    /// <param name="csvStream"> Поток с содержимым csv файла. </param>
    /// <returns> Результат парсинга: корректные строки и накопленные ошибки. </returns>
    ParseResult Parse(Stream csvStream);
}