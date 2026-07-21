using MetricsApi.Domain.Models;

namespace MetricsApi.Domain.Abstractions.Repositories;

/// <summary>
///     Репозиторий работы с метриками.
/// </summary>
public interface IMetricsRepository
{
    /// <summary>
    ///     В одной транзакции удаляет старые данные файла и сохраняет новые значения и результаты.
    /// </summary>
    /// <param name="fileName"> Имя файла, данные которого перезаписываются. </param>
    /// <param name="values"> Значения из csv файла. </param>
    /// <param name="result"> Интегральные результаты, посчитанные по файлу. </param>
    /// <param name="ct"> Токен отмены операции. </param>
    Task ReplaceFileDataAsync(string fileName, IReadOnlyList<ValueRecord> values, ResultMetric result,
        CancellationToken ct);
    
    /// <summary>
    ///     Возвращает интегральные результаты, подходящие под фильтры.
    /// </summary>
    /// <param name="filter"> Фильтры по имени файла и диапазонам значений. </param>
    /// <param name="ct"> Токен отмены операции. </param>
    /// <returns> Список результатов из таблицы Results. </returns>
    Task<IReadOnlyList<ResultMetric>> GetResultsAsync(ResultFilter filter, CancellationToken ct);
    
    /// <summary>
    ///     Возвращает последние значения файла, отсортированные по времени запуска по убыванию.
    /// </summary>
    /// <param name="fileName"> Имя файла. </param>
    /// <param name="count"> Максимальное количество возвращаемых записей. </param>
    /// <param name="ct"> Токен отмены операции. </param>
    /// <returns> Список последних значений из таблицы Values. </returns>
    Task<IReadOnlyList<ValueRecord>> GetLatestValuesAsync(string fileName, int count, CancellationToken ct);
}