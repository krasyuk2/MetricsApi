namespace MetricsApi.Domain.DTOs;

public record ProcessingResult(bool IsSuccess, IReadOnlyList<string> Errors)
{
    public static ProcessingResult Success() => new(true, []);
    public static ProcessingResult Failure(IReadOnlyList<string> errors) => new(false, errors);
}