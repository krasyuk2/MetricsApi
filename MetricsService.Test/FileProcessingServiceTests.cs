using MetricsApi.Application.Implementation;
using MetricsApi.Domain.Abstractions;
using MetricsApi.Domain.Abstractions.Repositories;
using MetricsApi.Domain.DTOs;
using MetricsApi.Domain.Models;
using NSubstitute;
using NUnit.Framework;

namespace MetricsService.Test;

/// <summary>
///     Тесты сервиса обработки csv файла.
/// </summary>
[TestFixture]
public class FileProcessingServiceTests
{
    private ICsvParser _parser = null!;
    private ICsvValidator _validator = null!;
    private IMetricsRepository _repository = null!;
    private FileProcessingService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _parser = Substitute.For<ICsvParser>();
        _validator = Substitute.For<ICsvValidator>();
        _repository = Substitute.For<IMetricsRepository>();
        _service = new FileProcessingService(_parser, _validator, _repository);
    }

    [Test]
    public async Task ProcessAsync_ParserErrors_ReturnsFailureAndDoesNotSave()
    {
        SetupParser([], ["ошибка парсинга"]);
        _validator.Validate(Arg.Any<IReadOnlyList<CsvRow>>()).Returns([]);

        var result = await _service.ProcessAsync("a.csv", Stream.Null, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors, Does.Contain("ошибка парсинга"));
        await _repository.DidNotReceiveWithAnyArgs()
            .ReplaceFileDataAsync(default!, default!, default!, default);
    }

    [Test]
    public async Task ProcessAsync_ValidationErrors_ReturnsFailureAndDoesNotSave()
    {
        SetupParser([Row(Date(10), 1, 5)], []);
        _validator.Validate(Arg.Any<IReadOnlyList<CsvRow>>()).Returns(["ошибка валидации"]);

        var result = await _service.ProcessAsync("a.csv", Stream.Null, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors, Does.Contain("ошибка валидации"));
        await _repository.DidNotReceiveWithAnyArgs()
            .ReplaceFileDataAsync(default!, default!, default!, default);
    }

    [Test]
    public async Task ProcessAsync_CombinesParserAndValidatorErrors()
    {
        SetupParser([Row(Date(10), 1, 5)], ["ошибка парсинга"]);
        _validator.Validate(Arg.Any<IReadOnlyList<CsvRow>>()).Returns(["ошибка валидации"]);

        var result = await _service.ProcessAsync("a.csv", Stream.Null, CancellationToken.None);

        Assert.That(result.Errors, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task ProcessAsync_ValidRows_SavesValuesAndMetrics()
    {
        SetupParser([
            Row(Date(10), 2, 10),
            Row(Date(12), 4, 30),
            Row(Date(11), 6, 20)
        ], []);
        _validator.Validate(Arg.Any<IReadOnlyList<CsvRow>>()).Returns([]);

        IReadOnlyList<ValueRecord>? savedValues = null;
        ResultMetric? savedResult = null;
        await _repository.ReplaceFileDataAsync(
            "a.csv",
            Arg.Do<IReadOnlyList<ValueRecord>>(v => savedValues = v),
            Arg.Do<ResultMetric>(r => savedResult = r),
            Arg.Any<CancellationToken>());

        var result = await _service.ProcessAsync("a.csv", Stream.Null, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(savedValues, Is.Not.Null);
        Assert.That(savedValues, Has.Count.EqualTo(3));
        Assert.That(savedValues!.Select(v => v.FileName), Is.All.EqualTo("a.csv"));

        Assert.That(savedResult, Is.Not.Null);
        Assert.That(savedResult!.FileName, Is.EqualTo("a.csv"));
        Assert.That(savedResult.FirstStartTime, Is.EqualTo(Date(10)));
        Assert.That(savedResult.DeltaTime, Is.EqualTo(TimeSpan.FromHours(2)));
        Assert.That(savedResult.AvgDuration, Is.EqualTo(TimeSpan.FromSeconds(4)));
        Assert.That(savedResult.ValueMean, Is.EqualTo(20));
        Assert.That(savedResult.ValueMedian, Is.EqualTo(20));
        Assert.That(savedResult.ValueMin, Is.EqualTo(10));
        Assert.That(savedResult.ValueMax, Is.EqualTo(30));
    }

    [Test]
    public async Task ProcessAsync_EvenRowCount_MedianIsAverageOfMiddleValues()
    {
        SetupParser([
            Row(Date(10), 1, 10),
            Row(Date(11), 1, 20),
            Row(Date(12), 1, 30),
            Row(Date(13), 1, 40)
        ], []);
        _validator.Validate(Arg.Any<IReadOnlyList<CsvRow>>()).Returns([]);

        ResultMetric? savedResult = null;
        await _repository.ReplaceFileDataAsync(
            Arg.Any<string>(),
            Arg.Any<IReadOnlyList<ValueRecord>>(),
            Arg.Do<ResultMetric>(r => savedResult = r),
            Arg.Any<CancellationToken>());

        await _service.ProcessAsync("a.csv", Stream.Null, CancellationToken.None);

        Assert.That(savedResult, Is.Not.Null);
        Assert.That(savedResult!.ValueMedian, Is.EqualTo(25));
    }

    private void SetupParser(List<CsvRow> rows, List<string> errors) =>
        _parser.Parse(Arg.Any<Stream>()).Returns(new ParseResult(rows, errors));

    private static CsvRow Row(DateTime start, double durationSeconds, double value) =>
        new(start, TimeSpan.FromSeconds(durationSeconds), value);

    private static DateTime Date(int hour) =>
        new(2024, 3, 15, hour, 0, 0, DateTimeKind.Utc);
}