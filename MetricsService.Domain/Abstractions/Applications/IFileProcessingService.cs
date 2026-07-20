using MetricsApi.Domain.DTOs;

namespace MetricsApi.Domain.Abstractions.Applications;

public interface IFileProcessingService
{
    Task<ProcessingResult> ProcessAsync(string fileName, Stream csvStream, CancellationToken ct);
}