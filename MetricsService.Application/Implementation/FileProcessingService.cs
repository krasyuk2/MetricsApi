using MetricsApi.Domain.Abstractions;
using MetricsApi.Domain.Abstractions.Applications;
using MetricsApi.Domain.Abstractions.Repositories;
using MetricsApi.Domain.DTOs;
using MetricsApi.Domain.Models;

namespace MetricsApi.Application.Services;

public class FileProcessingService : IFileProcessingService
{
    private readonly ICsvParser _parser;
    private readonly ICsvValidator _validator;
    private readonly IMetricsRepository _repository;

    public FileProcessingService(ICsvParser parser, ICsvValidator validator, IMetricsRepository repository)
    {
        _parser = parser;
        _validator = validator;
        _repository = repository;
    }

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
    
    private static double CalculateMedian(IEnumerable<double> values)
    {
        var sorted = values.Order().ToArray();
        var mid = sorted.Length / 2;

        return sorted.Length % 2 == 0
            ? (sorted[mid - 1] + sorted[mid]) / 2.0
            : sorted[mid];
    }
}