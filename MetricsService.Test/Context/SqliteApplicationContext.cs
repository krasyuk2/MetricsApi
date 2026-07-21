using MetricsApi.Domain.Models;
using MetricsApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace MetricsService.Test.Context;

/// <summary>
///     Контекст для тестов: хранит TimeSpan как ticks, т.к. SQLite не умеет сравнивать интервалы.
/// </summary>
public class SqliteApplicationContext : ApplicationContext
{
    /// <summary>
    ///     Конструктор.
    /// </summary>
    public SqliteApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) {}

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<ResultMetric>().Property(x => x.DeltaTime)
            .HasConversion(ts => ts.Ticks, t => TimeSpan.FromTicks(t));
        mb.Entity<ResultMetric>().Property(x => x.AvgDuration)
            .HasConversion(ts => ts.Ticks, t => TimeSpan.FromTicks(t));
        mb.Entity<ValueRecord>().Property(x => x.Duration)
            .HasConversion(ts => ts.Ticks, t => TimeSpan.FromTicks(t));
    }
}