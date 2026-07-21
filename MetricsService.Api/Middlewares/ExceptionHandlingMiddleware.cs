using Microsoft.AspNetCore.Mvc;

namespace MetricsApi.Middlewares;

/// <summary>
///     Middleware глобальной обработки исключений: логирует ошибку и возвращает ProblemDetails.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    ///     Конструктор.
    /// </summary>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    ///     Выполняет следующий шаг конвейера и перехватывает необработанные исключения.
    /// </summary>
    /// <param name="context"> Контекст текущего запроса. </param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogInformation("Запрос {Path} отменён клиентом.", context.Request.Path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Необработанное исключение при обработке запроса {Path}.", context.Request.Path);
            await WriteProblemAsync(context);
        }
    }

    /// <summary>
    ///     Записывает в ответ ProblemDetails с кодом 500, если ответ ещё не начат.
    /// </summary>
    /// <param name="context"> Контекст текущего запроса. </param>
    private static async Task WriteProblemAsync(HttpContext context)
    {
        if (context.Response.HasStarted)
            return;

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Внутренняя ошибка сервера.",
            Detail = "Произошла непредвиденная ошибка. Попробуйте повторить запрос позже.",
            Instance = context.Request.Path
        };

        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem, context.RequestAborted);
    }
}