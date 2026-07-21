using MetricsApi.Domain.Abstractions.Applications;
using MetricsApi.Domain.Abstractions.Repositories;
using MetricsApi.Domain.Models;
using MetricsApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MetricsApi.Controllers;

/// <summary>
///     Контроллер работы с метриками: загрузка csv, выборка результатов и последних значений.
/// </summary>
[ApiController]
[Route("api/metrics")]
public class MetricsController : ControllerBase
{
    /// <summary>
    ///     Сервис обработки csv файла: парсинг, валидация, расчёт метрик и сохранение.
    /// </summary>
    private readonly IFileProcessingService _processingService;
    
    /// <summary>
    ///     Репозиторий работы с метриками.
    /// </summary>
    private readonly IMetricsRepository _repository;

    /// <summary>
    ///     Конструктор.
    /// </summary>
    public MetricsController(IFileProcessingService processingService, IMetricsRepository repository)
    {
        _processingService = processingService;
        _repository = repository;
    }
    
    /// <summary>
    ///     Принимает csv файл, валидирует его и сохраняет значения и интегральные результаты в БД.
    /// </summary>
    /// <param name="csvFile"> Файл формата Date;ExecutionTime;Value. </param>
    /// <param name="ct"> Токен отмены операции. </param>
    /// <returns> 200 при успехе, 400 со списком ошибок если файл невалиден. </returns>
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
    
    /// <summary>
    ///     Возвращает список интегральных результатов, подходящих под фильтры.
    /// </summary>
    /// <param name="request"> Фильтры по имени файла и диапазонам значений. </param>
    /// <param name="ct"> Токен отмены операции. </param>
    /// <returns> Список результатов из таблицы Results. </returns>
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
    
    /// <summary>
    ///     Возвращает последние 10 значений файла, отсортированных по времени запуска по убыванию.
    /// </summary>
    /// <param name="fileName"> Имя файла. </param>
    /// <param name="ct"> Токен отмены операции. </param>
    /// <returns> Список значений, 404 если файл не найден. </returns>
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
    
    /// <summary>
    ///     Приводит дату к UTC: Unspecified трактуется как UTC, Local конвертируется.
    /// </summary>
    /// <param name="dt"> Исходная дата либо null. </param>
    private static DateTime? ToUtc(DateTime? dt) => dt?.Kind switch
    {
        null => null,
        DateTimeKind.Utc => dt,
        DateTimeKind.Unspecified => DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc),
        _ => dt.Value.ToUniversalTime()
    };
}