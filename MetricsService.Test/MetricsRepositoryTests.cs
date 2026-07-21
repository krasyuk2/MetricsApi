using MetricsApi.Domain.Models;
using MetricsApi.Repository;
using MetricsApi.Repository.Repositories;
using MetricsService.Test.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MetricsService.Test;

/// <summary>
///     Тесты репозитория метрик на SQLite in-memory.
/// </summary>
[TestFixture]
public class MetricsRepositoryTests
{
    private SqliteConnection _connection = null!;
    private ApplicationContext _context = null!;
    private MetricsRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new SqliteApplicationContext(options);
        _context.Database.EnsureCreated();
        _repository = new MetricsRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Test]
    public async Task ReplaceFileDataAsync_NewFile_SavesValuesAndResult()
    {
        await _repository.ReplaceFileDataAsync("a.csv",
            [Value("a.csv", Date(10), 1, 5), Value("a.csv", Date(11), 2, 10)],
            Result("a.csv", valueMean: 7.5),
            CancellationToken.None);

        Assert.That(await _context.Values.CountAsync(), Is.EqualTo(2));
        var results = await _context.Results.ToListAsync();
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].FileName, Is.EqualTo("a.csv"));
        Assert.That(results[0].ValueMean, Is.EqualTo(7.5));
    }

    [Test]
    public async Task ReplaceFileDataAsync_ExistingFile_OverwritesData()
    {
        await _repository.ReplaceFileDataAsync("a.csv",
            [Value("a.csv", Date(10), 1, 5)],
            Result("a.csv", valueMean: 5),
            CancellationToken.None);

        await _repository.ReplaceFileDataAsync("a.csv",
            [Value("a.csv", Date(12), 3, 100), Value("a.csv", Date(13), 3, 200)],
            Result("a.csv", valueMean: 150),
            CancellationToken.None);

        Assert.That(await _context.Values.CountAsync(), Is.EqualTo(2));
        var results = await _context.Results.ToListAsync();
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].ValueMean, Is.EqualTo(150));
    }

    [Test]
    public async Task ReplaceFileDataAsync_OtherFileData_IsNotTouched()
    {
        await _repository.ReplaceFileDataAsync("a.csv",
            [Value("a.csv", Date(10), 1, 5)], Result("a.csv"), CancellationToken.None);
        await _repository.ReplaceFileDataAsync("b.csv",
            [Value("b.csv", Date(10), 1, 5)], Result("b.csv"), CancellationToken.None);

        await _repository.ReplaceFileDataAsync("a.csv",
            [Value("a.csv", Date(11), 2, 10)], Result("a.csv"), CancellationToken.None);

        Assert.That(await _context.Values.CountAsync(v => v.FileName == "b.csv"), Is.EqualTo(1));
        Assert.That(await _context.Results.CountAsync(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetResultsAsync_NoFilters_ReturnsAll()
    {
        await Seed(Result("a.csv"), Result("b.csv"));

        var results = await _repository.GetResultsAsync(
            new ResultFilter(null, null, null, null, null, null, null), CancellationToken.None);

        Assert.That(results, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetResultsAsync_FilterByFileName_ReturnsOnlyMatching()
    {
        await Seed(Result("a.csv"), Result("b.csv"));

        var results = await _repository.GetResultsAsync(
            new ResultFilter("a.csv", null, null, null, null, null, null), CancellationToken.None);

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].FileName, Is.EqualTo("a.csv"));
    }

    [Test]
    public async Task GetResultsAsync_FilterByFirstStartTimeRange_ReturnsOnlyMatching()
    {
        await Seed(
            Result("a.csv", firstStartTime: Date(10)),
            Result("b.csv", firstStartTime: Date(12)),
            Result("c.csv", firstStartTime: Date(14)));

        var results = await _repository.GetResultsAsync(
            new ResultFilter(null, Date(11), Date(13), null, null, null, null), CancellationToken.None);

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].FileName, Is.EqualTo("b.csv"));
    }

    [Test]
    public async Task GetResultsAsync_FilterByValueMeanRange_ReturnsOnlyMatching()
    {
        await Seed(
            Result("a.csv", valueMean: 5),
            Result("b.csv", valueMean: 15),
            Result("c.csv", valueMean: 25));

        var results = await _repository.GetResultsAsync(
            new ResultFilter(null, null, null, 10, 20, null, null), CancellationToken.None);

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].FileName, Is.EqualTo("b.csv"));
    }

    [Test]
    public async Task GetResultsAsync_FilterByAvgDurationRange_ReturnsOnlyMatching()
    {
        await Seed(
            Result("a.csv", avgDurationSeconds: 1),
            Result("b.csv", avgDurationSeconds: 5),
            Result("c.csv", avgDurationSeconds: 10));

        var results = await _repository.GetResultsAsync(
            new ResultFilter(null, null, null, null, null, 3, 7), CancellationToken.None);

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].FileName, Is.EqualTo("b.csv"));
    }

    [Test]
    public async Task GetLatestValuesAsync_ReturnsTop10SortedByStartTimeDescending()
    {
        var values = Enumerable.Range(0, 15)
            .Select(i => Value("a.csv", Date(0).AddHours(i), 1, i))
            .ToList();
        await _repository.ReplaceFileDataAsync("a.csv", values, Result("a.csv"), CancellationToken.None);

        var latest = await _repository.GetLatestValuesAsync("a.csv", 10, CancellationToken.None);

        Assert.That(latest, Has.Count.EqualTo(10));
        Assert.That(latest[0].StartTime, Is.EqualTo(Date(0).AddHours(14)));
        Assert.That(latest, Is.Ordered.Descending.By(nameof(ValueRecord.StartTime)));
    }

    [Test]
    public async Task GetLatestValuesAsync_UnknownFile_ReturnsEmpty()
    {
        var latest = await _repository.GetLatestValuesAsync("missing.csv", 10, CancellationToken.None);

        Assert.That(latest, Is.Empty);
    }

    private async Task Seed(params ResultMetric[] results)
    {
        _context.Results.AddRange(results);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    private static ValueRecord Value(string fileName, DateTime start, double durationSeconds, double value) =>
        new()
        {
            FileName = fileName,
            StartTime = start,
            Duration = TimeSpan.FromSeconds(durationSeconds),
            Value = value
        };

    private static ResultMetric Result(string fileName, DateTime? firstStartTime = null,
        double valueMean = 0, double avgDurationSeconds = 0) =>
        new()
        {
            FileName = fileName,
            DeltaTime = TimeSpan.Zero,
            FirstStartTime = firstStartTime ?? Date(10),
            AvgDuration = TimeSpan.FromSeconds(avgDurationSeconds),
            ValueMean = valueMean,
            ValueMedian = 0,
            ValueMin = 0,
            ValueMax = 0
        };

    private static DateTime Date(int hour) =>
        new(2024, 3, 15, hour, 0, 0, DateTimeKind.Utc);
}