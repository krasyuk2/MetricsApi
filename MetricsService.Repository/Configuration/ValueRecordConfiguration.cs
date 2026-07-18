using MetricsApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetricsApi.Repository.Configuration;

/// <summary>
///     Конфигурация для модели ValueRecord.
/// </summary>
public class ValueRecordConfiguration : IEntityTypeConfiguration<ValueRecord>
{
    public void Configure(EntityTypeBuilder<ValueRecord> builder) 
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FileName).HasMaxLength(255);
        builder.HasIndex(x => new { x.FileName, x.StartTime });
    }
}