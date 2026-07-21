using System.Text;
using MetricsApi.Application.Implementation;
using NUnit.Framework;

namespace MetricsService.Test;

/// <summary>
///     Тесты парсера csv.
/// </summary>
[TestFixture]
public class CsvParserTests
{
    private readonly CsvParser _parser = new();

    private const string Header = "Date;ExecutionTime;Value";

    [Test]
    public void Parse_ValidFile_ReturnsRowsWithoutErrors()
    {
        var result = _parser.Parse(ToStream(
            Header,
            "2024-03-15T10-00-00.0000Z;1.5;100.5",
            "2024-03-15T11-00-00.0000Z;2;200"));

        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.Rows, Has.Count.EqualTo(2));
        Assert.That(result.Rows[0].StartTime,
            Is.EqualTo(new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc)));
        Assert.That(result.Rows[0].Duration, Is.EqualTo(TimeSpan.FromSeconds(1.5)));
        Assert.That(result.Rows[0].Value, Is.EqualTo(100.5));
    }

    [Test]
    public void Parse_CommaAsDecimalSeparator_ParsesNumbers()
    {
        var result = _parser.Parse(ToStream(Header, "2024-03-15T10-00-00.0000Z;1,5;100,25"));

        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.Rows[0].Duration, Is.EqualTo(TimeSpan.FromSeconds(1.5)));
        Assert.That(result.Rows[0].Value, Is.EqualTo(100.25));
    }

    [Test]
    public void Parse_InvalidDateFormat_ReturnsError()
    {
        var result = _parser.Parse(ToStream(Header, "2024-03-15 10:00:00;1;1"));

        Assert.That(result.Rows, Is.Empty);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0], Does.Contain("не соответствует формату даты"));
    }

    [Test]
    public void Parse_NotANumber_ReturnsError()
    {
        var result = _parser.Parse(ToStream(Header, "2024-03-15T10-00-00.0000Z;abc;xyz"));

        Assert.That(result.Rows, Is.Empty);
        Assert.That(result.Errors, Has.Count.EqualTo(2));
    }

    [Test]
    public void Parse_MissingValue_ReturnsError()
    {
        var result = _parser.Parse(ToStream(Header, "2024-03-15T10-00-00.0000Z;1.5"));

        Assert.That(result.Rows, Is.Empty);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0], Does.Contain("ожидается 3 значения"));
    }

    [Test]
    public void Parse_MoreThanMaxRows_ReturnsError()
    {
        var lines = new List<string> { Header };
        lines.AddRange(Enumerable.Range(0, 10_001)
            .Select(_ => "2024-03-15T10-00-00.0000Z;1;1"));

        var result = _parser.Parse(ToStream(lines.ToArray()));

        Assert.That(result.Errors, Has.Some.Contains("больше 10000 строк"));
    }

    [Test]
    public void Parse_ExactlyMaxRows_NoErrors()
    {
        var lines = new List<string> { Header };
        lines.AddRange(Enumerable.Range(0, 10_000)
            .Select(_ => "2024-03-15T10-00-00.0000Z;1;1"));

        var result = _parser.Parse(ToStream(lines.ToArray()));

        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.Rows, Has.Count.EqualTo(10_000));
    }

    [Test]
    public void Parse_MultipleBadLines_CollectsAllErrors()
    {
        var result = _parser.Parse(ToStream(
            Header,
            "bad-date;1;1",
            "2024-03-15T10-00-00.0000Z;bad;1",
            "2024-03-15T10-00-00.0000Z;1;1"));

        Assert.That(result.Rows, Has.Count.EqualTo(1));
        Assert.That(result.Errors, Has.Count.EqualTo(2));
    }

    private static MemoryStream ToStream(params string[] lines) =>
        new(Encoding.UTF8.GetBytes(string.Join('\n', lines)));
}