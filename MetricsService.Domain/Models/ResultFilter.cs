namespace MetricsApi.Domain.Models;

public record ResultFilter(
    string? FileName,
    DateTime? FirstStartTimeFrom,
    DateTime? FirstStartTimeTo,
    double? ValueMeanFrom,
    double? ValueMeanTo,
    double? AvgDurationSecondsFrom,
    double? AvgDurationSecondsTo);