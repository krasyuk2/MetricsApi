using MetricsApi.Domain.Abstractions;
using MetricsApi.Domain.Abstractions.Applications;
using MetricsApi.Domain.Abstractions.Repositories;
using MetricsApi.Domain.DTOs;
using MetricsApi.Domain.Models;

namespace MetricsApi.Application.Implementation;

/// <inheritdoc/>
public class FileProcessingService : IFileProcessingService
{
    /// <summary>
    ///     Парсер.
    /// </summary>
    private readonly ICsvParser _parser;
    
    /// <summary>
    ///     Валидатор.
    /// </summary>
    private readonly ICsvValidator _validator;
    
    /// <summary>
    ///     Точка взаимодействия с бд.
    /// </summary>
    private readonly IMetricsRepository _repository;

    /// <summary>
    ///     Конструктор.
    /// </summary>
    public FileProcessingService(ICsvParser parser, ICsvValidator validator, IMetricsRepository repository)
    {
        _parser = parser;
        _validator = validator;
        _repository = repository;
    }
    
    /// <inheritdoc/>
    public async Task<ProcessingResult> ProcessAsync(string fileName, Stream csvStream, CancellationToken ct)
    {
        var parsed = _parser.Parse(csvStream);

        var errors = parsed.Errors
            .Concat(_validator.Validate(parsed.Rows))
            .ToList();

        if (errors.Count > 0)
            return ProcessingResult.Failure(errors);

        var values = parsed.Rows
            .Select(r => new ValueRecord
            {
                FileName = fileName,
                StartTime = r.StartTime,
                Duration = r.Duration,
                Value = r.Value
            })
            .ToList();

        var result = CalculateMetrics(fileName, parsed.Rows);

        await _repository.ReplaceFileDataAsync(fileName, values, result, ct);
        return ProcessingResult.Success();
    }

    
    /// <summary>
    ///     Подсчитывает интегральные результаты по строкам файла.
    /// </summary>
    /// <param name="fileName"> Имя файла. </param>
    /// <param name="rows"> Корректные строки csv файла. </param>
    /// <returns> Интегральные результаты для записи в таблицу Results. </returns>
    private static ResultMetric CalculateMetrics(string fileName, IReadOnlyList<CsvRow> rows)
    {
        var minDate = rows.Min(r => r.StartTime);
        var maxDate = rows.Max(r => r.StartTime);

        return new ResultMetric
        {
            FileName = fileName,
            DeltaTime = maxDate - minDate,
            FirstStartTime = minDate,
            AvgDuration = TimeSpan.FromSeconds(rows.Average(r => r.Duration.TotalSeconds)),
            ValueMean = rows.Average(r => r.Value),
            ValueMedian = CalculateMedian(rows.Select(r => r.Value)),
            ValueMin = rows.Min(r => r.Value),
            ValueMax = rows.Max(r => r.Value)
        };
    }
    
    /// <summary>
    ///     Считает медиану: середина отсортированного набора либо среднее двух центральных элементов.
    /// </summary>
    /// <param name="values"> Значения показателя. </param>
    private static double CalculateMedian(IEnumerable<double> values)
    {
        var sorted = values.Order().ToArray();
        var mid = sorted.Length / 2;

        return sorted.Length % 2 == 0
            ? (sorted[mid - 1] + sorted[mid]) / 2.0
            : sorted[mid];
    }
}