using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MetricsApi.Domain.Abstractions;
using MetricsApi.Domain.DTOs;

namespace MetricsApi.Application.Implementation;

/// <summary>
///     Парсер CSV формата Date;ExecutionTime;Value.
/// </summary>
public class CsvParser : ICsvParser
{
    private const string DateFormat = "yyyy-MM-ddTHH-mm-ss.ffffZ";
    private const int MaxRows = 10_000;

    private static readonly CsvConfiguration Config = new(CultureInfo.InvariantCulture)
    {
        Delimiter = ";",
        HasHeaderRecord = true,
        TrimOptions = TrimOptions.Trim,
        BadDataFound = null // кривые строки обрабатываем сами
    };

    public ParseResult Parse(Stream csvStream)
    {
        var rows = new List<CsvRow>();
        var errors = new List<string>();

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, Config);

        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            var line = csv.Parser.Row;
            
            if (rows.Count >= MaxRows)
            {
                errors.Add($"Файл содержит больше {MaxRows} строк.");
                break;
            }

            if (csv.Parser.Count != 3)
            {
                errors.Add($"Строка {line}: ожидается 3 значения, получено {csv.Parser.Count}.");
                continue;
            }

            var rawDate = csv.GetField(0);
            var rawDuration = csv.GetField(1);
            var rawValue = csv.GetField(2);

            var hasError = false;

            if (!DateTime.TryParseExact(rawDate, DateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var startTime))
            {
                errors.Add($"Строка {line}: '{rawDate}' не соответствует формату даты {DateFormat}.");
                hasError = true;
            }

            if (!TryParseDouble(rawDuration, out var durationSeconds))
            {
                errors.Add($"Строка {line}: '{rawDuration}' не является числом (ExecutionTime).");
                hasError = true;
            }

            if (!TryParseDouble(rawValue, out var value))
            {
                errors.Add($"Строка {line}: '{rawValue}' не является числом (Value).");
                hasError = true;
            }

            if (!hasError)
                rows.Add(new CsvRow(startTime, TimeSpan.FromSeconds(durationSeconds), value));
        }

        return new ParseResult(rows, errors);
    }
    
    private static bool TryParseDouble(string? raw, out double result) =>
        double.TryParse(raw?.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out result);
}