using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SharedService.SharedKernel;

namespace SharedService.Framework.Middlewares;

/// <summary>
/// Обработчик исключений.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var envelope = Envelope.Error(GeneralErrors.Failure(ex.Message).ToErrors());

            await context.Response.WriteAsJsonAsync(envelope);

        }
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static void UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionMiddleware>();
    }
}