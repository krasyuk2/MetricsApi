using MetricsApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MetricsApi.Repository;

/// <summary>
///     Контекст приложения.
/// </summary>
public class ApplicationContext : DbContext
{
    /// <summary>
    ///     Конструктор.
    /// </summary>
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) {}

    /// <summary>
    ///     Значения из файлов.
    /// </summary>
    public DbSet<ValueRecord> Values { get; set; }
    
    /// <summary>
    ///     Результаты подсчета из файлов метрик.
    /// </summary>
    public DbSet<ResultMetric> Results  { get; set; }
    
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder mb) =>
        mb.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
}