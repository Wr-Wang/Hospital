using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hospital.Api.Filters;

/// <summary>
/// 全局 API 请求/响应日志记录器。
/// 记录每个请求的接口名称、参数、响应内容和处理耗时。
/// </summary>
public class ApiLogFilter : IAsyncActionFilter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    // 不记录的敏感请求头
    private static readonly HashSet<string> SensitiveHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Cookie", "Set-Cookie", "X-Api-Key", "Api-Key"
    };

    private readonly ILogger<ApiLogFilter> _logger;
    private readonly IConfiguration _configuration;

    public ApiLogFilter(ILogger<ApiLogFilter> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!_configuration.GetValue<bool>("ApiLogging:Enabled"))
        {
            await next();
            return;
        }

        var controllerName = context.ActionDescriptor.RouteValues["controller"] ?? context.Controller.GetType().Name;
        var actionName = context.ActionDescriptor.RouteValues["action"] ?? "Unknown";
        var httpMethod = context.HttpContext.Request.Method;
        var requestPath = context.HttpContext.Request.Path;

        var stopwatch = Stopwatch.StartNew();

        // 请求头日志（过滤敏感头）
        var headers = context.HttpContext.Request.Headers
            .Where(h => !SensitiveHeaders.Contains(h.Key))
            .ToDictionary(h => h.Key, h => h.Value.ToString());

        var argsJson = SerializeSafe(context.ActionArguments);
        var headersJson = SerializeSafe(headers);

        // ── 请求日志 ──
        _logger.LogInformation(
            "[API] >>> {Method} {Path}  {Controller}.{Action}",
            httpMethod, requestPath, controllerName, actionName);
        _logger.LogInformation(
            "[API] >>> Headers: {Headers}",
            headersJson);
        if (context.ActionArguments.Count > 0)
        {
            _logger.LogInformation(
                "[API] >>> 参数: {Arguments}",
                argsJson);
        }

        var resultContext = await next();

        stopwatch.Stop();

        // ── 响应日志 ──
        object? responseValue = null;
        var statusCode = context.HttpContext.Response.StatusCode;

        if (resultContext.Result is ObjectResult objectResult)
        {
            responseValue = objectResult.Value;
        }
        else if (resultContext.Result is JsonResult jsonResult)
        {
            responseValue = jsonResult.Value;
        }

        _logger.LogInformation(
            "[API] <<< {StatusCode} ({Elapsed}ms)  {Controller}.{Action}",
            statusCode, stopwatch.Elapsed.TotalMilliseconds.ToString("F0"), controllerName, actionName);
        if (responseValue != null)
        {
            var responseJson = SerializeSafe(responseValue);
            _logger.LogInformation(
                "[API] <<< 响应: {Response} \n\r",
                responseJson);
        }
    }

    private static string SerializeSafe(object obj)
    {
        if (obj is string s) return s;
        if (obj == null) return "(null)";
        try
        {
            return JsonSerializer.Serialize(obj, JsonOptions);
        }
        catch
        {
            return $"(serialization failed: {obj.GetType().Name})";
        }
    }
}
