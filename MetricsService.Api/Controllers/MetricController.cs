using Microsoft.AspNetCore.Mvc;

namespace MetricsApi.Controllers;

/// <summary>
///     Контроллер для парсинга и получение результатов.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MetricController : ControllerBase
{
    /// <summary>
    ///     Загрузить файл, провалидировать, распарсить, записать результаты.
    /// </summary>
    /// <param name="csvFile"> Файл формата csv. </param>
    [HttpPost("files")]
    public async Task<ActionResult> UploadCsvAsync(IFormFile csvFile, CancellationToken ct)
    {
        return Ok(); 
    }

    [HttpGet("results")]
    public async Task GetResultsAsync(CancellationToken ct)
    {
        
    }
    
    [HttpGet("values/{fileName}/latest")]
    public async Task GetLastTenResultsAsync(string fileName, CancellationToken ct)
    {
    }
}