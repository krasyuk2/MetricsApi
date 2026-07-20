using MetricsApi.Domain.Abstractions;
using MetricsApi.Domain.DTOs;

namespace MetricsApi.Application.Implementation;

/// <summary>Бизнес-валидация строк по правилам ТЗ.</summary>
public class CsvValidator : ICsvValidator
{
    private static readonly DateTime MinDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public IReadOnlyList<string> Validate(IReadOnlyList<CsvRow> rows)
    {
        var errors = new List<string>();

        if (rows.Count == 0)
            errors.Add("Файл не содержит ни одной корректной записи.");

        var now = DateTime.UtcNow;

        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var line = i + 2; // +1 заголовок, +1 нумерация с единицы

            if (row.StartTime < MinDate || row.StartTime > now)
                errors.Add($"Строка {line}: дата должна быть в диапазоне от 01.01.2000 до текущего момента.");

            if (row.Duration < TimeSpan.Zero)
                errors.Add($"Строка {line}: время выполнения не может быть меньше 0.");

            if (row.Value < 0)
                errors.Add($"Строка {line}: значение показателя не может быть меньше 0.");
        }

        return errors;
    }
}