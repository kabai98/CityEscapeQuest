using CityEscapeQuest.Domain.Common;
using CityEscapeQuest.Infrastructure.Identity;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CityEscapeQuest.WebApi;

public sealed class ApiExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            DomainException => (StatusCodes.Status409Conflict, "Business rule was violated"),
            IdentityOperationException => (StatusCodes.Status400BadRequest, "Identity operation failed"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource was not found"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Authentication or authorization failed"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        httpContext.Response.StatusCode = statusCode;
        var detail = statusCode == StatusCodes.Status500InternalServerError && !environment.IsEnvironment("Testing")
            ? "The server could not complete the request."
            : exception.Message;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            },
            Exception = exception
        });
    }
}