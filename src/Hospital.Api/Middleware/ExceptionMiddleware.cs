using System.Net;
using System.Text.Json;

namespace Hospital.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "业务逻辑错误");
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Message, "business_error");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "资源不存在");
            await WriteErrorResponse(context, HttpStatusCode.NotFound, ex.Message, "not_found");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "无权限访问");
            await WriteErrorResponse(context, HttpStatusCode.Forbidden, ex.Message, "forbidden");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "服务器内部错误");
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError, "服务器内部错误", "server_error");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string message, string type)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var result = JsonSerializer.Serialize(new
        {
            error = message,
            type
        });

        await context.Response.WriteAsync(result);
    }
}
