using GoodHamburger.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, "Not Found", ex.Message);
        }
        catch (DomainValidationException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "Validation Error", ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error processing request.");
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError,
                "Internal Server Error", "An unexpected error occurred.");
        }
    }

    private static Task WriteProblemAsync(HttpContext context, int status, string title, string detail)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = status,
            Title  = title,
            Detail = detail
        };

        return context.Response.WriteAsJsonAsync(problem);
    }
}
