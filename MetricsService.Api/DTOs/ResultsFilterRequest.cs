namespace MetricsApi.DTOs;

public record ResultsFilterRequest(
    string? FileName,
    DateTime? FirstStartTimeFrom,
    DateTime? FirstStartTimeTo,
    double? ValueMeanFrom,
    double? ValueMeanTo,
    double? AvgDurationSecondsFrom,
    double? AvgDurationSecondsTo);