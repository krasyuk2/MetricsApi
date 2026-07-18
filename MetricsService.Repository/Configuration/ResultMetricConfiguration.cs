using MetricsApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetricsApi.Repository.Configuration;

/// <summary>
///     Конфигурация для модели ResultMetric.
/// </summary>
public class ResultMetricConfiguration : IEntityTypeConfiguration<ResultMetric>
{
    public void Configure(EntityTypeBuilder<ResultMetric> builder)
    {
        builder.HasKey(x => x.FileName);
        builder.Property(x => x.FileName).HasMaxLength(255);
        builder.HasIndex(x => x.FirstStartTime);
        builder.HasIndex(x => x.ValueMean);
        builder.HasIndex(x => x.AvgDuration);
    }
}