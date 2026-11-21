
using EWallet.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Api.Handlers;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                exception.Message
            ),

            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                exception.Message
            ),

            InsufficientBalanceException => (
                StatusCodes.Status409Conflict,
                "Insufficient Balance",
                exception.Message
            ),

            InvalidOperationException => (
                StatusCodes.Status409Conflict,
                "Conflict",
                exception.Message
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred."
            )
        };

        if (statusCode >= 500)
        {
            logger.LogError(
                exception,
                "An unhandled exception occurred."
            );
        }
        else
        {
            logger.LogWarning(
                exception,
                "A request failed with status code {StatusCode}.",
                statusCode
            );
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            },
            cancellationToken
        );

        return true;
    }
}
