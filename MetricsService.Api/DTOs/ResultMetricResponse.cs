namespace MetricsApi.DTOs;

public record ResultMetricResponse(
    string FileName,
    double DeltaTimeSeconds,
    DateTime FirstStartTime,
    double AvgDurationSeconds,
    double ValueMean,
    double ValueMedian,
    double ValueMin,
    double ValueMax);