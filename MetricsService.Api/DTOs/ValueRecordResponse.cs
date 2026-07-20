namespace MetricsApi.DTOs;

public record ValueRecordResponse(
    DateTime StartTime,
    double DurationSeconds,
    double Value);