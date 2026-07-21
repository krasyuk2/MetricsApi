using MetricsApi.Application.Implementation;
using MetricsApi.Domain.DTOs;
using NUnit.Framework;

namespace MetricsService.Test;

/// <summary>
///     Тесты бизнес-валидации строк csv.
/// </summary>
[TestFixture]
public class CsvValidatorTests
{
    private readonly CsvValidator _validator = new();

    [Test]
    public void Validate_EmptyRows_ReturnsError()
    {
        var errors = _validator.Validate([]);

        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors[0], Does.Contain("ни одной корректной записи"));
    }

    [Test]
    public void Validate_ValidRows_ReturnsNoErrors()
    {
        var errors = _validator.Validate([
            Row(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), 1, 1),
            Row(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), 0, 0)
        ]);

        Assert.That(errors, Is.Empty);
    }

    [Test]
    public void Validate_DateBefore2000_ReturnsError()
    {
        var errors = _validator.Validate([
            Row(new DateTime(1999, 12, 31, 23, 59, 59, DateTimeKind.Utc), 1, 1)
        ]);

        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors[0], Does.Contain("дата должна быть в диапазоне"));
    }

    [Test]
    public void Validate_DateInFuture_ReturnsError()
    {
        var errors = _validator.Validate([
            Row(DateTime.UtcNow.AddDays(1), 1, 1)
        ]);

        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors[0], Does.Contain("дата должна быть в диапазоне"));
    }

    [Test]
    public void Validate_NegativeDuration_ReturnsError()
    {
        var errors = _validator.Validate([
            Row(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), -1, 1)
        ]);

        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors[0], Does.Contain("время выполнения"));
    }

    [Test]
    public void Validate_NegativeValue_ReturnsError()
    {
        var errors = _validator.Validate([
            Row(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), 1, -0.5)
        ]);

        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors[0], Does.Contain("значение показателя"));
    }

    [Test]
    public void Validate_ErrorMessage_ContainsLineNumberWithHeaderOffset()
    {
        var errors = _validator.Validate([
            Row(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), 1, 1),
            Row(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), -1, 1)
        ]);

        Assert.That(errors, Has.Count.EqualTo(1));
        Assert.That(errors[0], Does.StartWith("Строка 3"));
    }

    private static CsvRow Row(DateTime start, double durationSeconds, double value) =>
        new(start, TimeSpan.FromSeconds(durationSeconds), value);
}