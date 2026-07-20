using MetricsApi.Domain.Abstractions.Applications;
using MetricsApi.Domain.Abstractions.Repositories;
using MetricsApi.Domain.Models;
using MetricsApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MetricsApi.Controllers;


[ApiController]
[Route("api/metrics")]
public class MetricsController : ControllerBase
{
    private readonly IFileProcessingService _processingService;
    private readonly IMetricsRepository _repository;

    public MetricsController(IFileProcessingService processingService, IMetricsRepository repository)
    {
        _processingService = processingService;
        _repository = repository;
    }
    
    
    [HttpPost("files")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadCsvAsync(IFormFile csvFile, CancellationToken ct)
    {
        if (csvFile is null || csvFile.Length == 0)
            return BadRequest("Файл не передан или пуст.");

        await using var stream = csvFile.OpenReadStream();
        var result = await _processingService.ProcessAsync(csvFile.FileName, stream, ct);

        if (result.IsSuccess)
            return Ok();

        return ValidationProblem(new ValidationProblemDetails
        {
            Title = "Файл не прошёл валидацию.",
            Errors = { ["csvFile"] = result.Errors.ToArray() }
        });
    }
    
    [HttpGet("results")]
    [ProducesResponseType(typeof(IReadOnlyList<ResultMetricResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ResultMetricResponse>>> GetResultsAsync(
        [FromQuery] ResultsFilterRequest request, CancellationToken ct)
    {
        var filter = new ResultFilter(
            request.FileName,
            ToUtc(request.FirstStartTimeFrom),
            ToUtc(request.FirstStartTimeTo),
            request.ValueMeanFrom, request.ValueMeanTo,
            request.AvgDurationSecondsFrom, request.AvgDurationSecondsTo);

        var results = await _repository.GetResultsAsync(filter, ct);

        return Ok(results.Select(r => new ResultMetricResponse(
            r.FileName,
            r.DeltaTime.TotalSeconds,
            r.FirstStartTime,
            r.AvgDuration.TotalSeconds,
            r.ValueMean, r.ValueMedian, r.ValueMin, r.ValueMax)));
    }
    
    [HttpGet("values/{fileName}/latest")]
    [ProducesResponseType(typeof(IReadOnlyList<ValueRecordResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ValueRecordResponse>>> GetLatestValuesAsync(
        string fileName, CancellationToken ct)
    {
        var values = await _repository.GetLatestValuesAsync(fileName, 10, ct);

        if (values.Count == 0)
            return NotFound($"Файл '{fileName}' не найден.");

        return Ok(values.Select(v => new ValueRecordResponse(
            v.StartTime, v.Duration.TotalSeconds, v.Value)));
    }
    
    private static DateTime? ToUtc(DateTime? dt) => dt?.Kind switch
    {
        null => null,
        DateTimeKind.Utc => dt,
        DateTimeKind.Unspecified => DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc),
        _ => dt.Value.ToUniversalTime()
    };
}