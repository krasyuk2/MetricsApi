namespace MetricsApi.Domain.DTOs;

/// <summary>
///     Результат обработки csv файла.
/// </summary>
/// <param name="IsSuccess"> Признак успешной обработки. </param>
/// <param name="Errors"> Ошибки парсинга и валидации, пустой список при успехе. </param>
public record ProcessingResult(bool IsSuccess, IReadOnlyList<string> Errors)
{
    /// <summary>
    ///     Создает успешный результат без ошибок.
    /// </summary>
    public static ProcessingResult Success() => new(true, []);
    
    /// <summary>
    ///     Создает неуспешный результат со списком ошибок.
    /// </summary>
    /// <param name="errors"> Ошибки, из-за которых файл признан невалидным. </param>
    public static ProcessingResult Failure(IReadOnlyList<string> errors) => new(false, errors);
}