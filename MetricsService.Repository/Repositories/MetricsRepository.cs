using MetricsApi.Domain.Abstractions.Repositories;
using MetricsApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MetricsApi.Repository.Repositories;

/// <inheritdoc/>
public class MetricsRepository : IMetricsRepository
{
    private readonly ApplicationContext _context;

    /// <summary>
    ///     Конструктор.
    /// </summary>
    public MetricsRepository(ApplicationContext context) => _context = context;

    /// <inheritdoc/>
    public async Task ReplaceFileDataAsync(string fileName, IReadOnlyList<ValueRecord> values,
        ResultMetric result, CancellationToken ct)
    {
        _context.ChangeTracker.Clear();
        
        await using var tx = await _context.Database.BeginTransactionAsync(ct);

        await _context.Values.Where(x => x.FileName == fileName).ExecuteDeleteAsync(ct);
        await _context.Results.Where(x => x.FileName == fileName).ExecuteDeleteAsync(ct);

        _context.Values.AddRange(values);
        _context.Results.Add(result);
        await _context.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ResultMetric>> GetResultsAsync(ResultFilter filter, CancellationToken ct)
    {
        var query = _context.Results.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.FileName))
            query = query.Where(x => x.FileName == filter.FileName);

        if (filter.FirstStartTimeFrom is { } from)
            query = query.Where(x => x.FirstStartTime >= from);

        if (filter.FirstStartTimeTo is { } to)
            query = query.Where(x => x.FirstStartTime <= to);

        if (filter.ValueMeanFrom is { } meanFrom)
            query = query.Where(x => x.ValueMean >= meanFrom);

        if (filter.ValueMeanTo is { } meanTo)
            query = query.Where(x => x.ValueMean <= meanTo);

        if (filter.AvgDurationSecondsFrom is { } durFrom)
            query = query.Where(x => x.AvgDuration >= TimeSpan.FromSeconds(durFrom));

        if (filter.AvgDurationSecondsTo is { } durTo)
            query = query.Where(x => x.AvgDuration <= TimeSpan.FromSeconds(durTo));

        return await query.ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ValueRecord>> GetLatestValuesAsync(string fileName, int count,
        CancellationToken ct)
    {
        return await _context.Values.AsNoTracking()
            .Where(x => x.FileName == fileName)
            .OrderByDescending(x => x.StartTime)
            .Take(count)
            .ToListAsync(ct);
    }
}