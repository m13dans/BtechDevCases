using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using SimpleAuthAPI.Model;

namespace SimpleAuthAPI.Service;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private const string UserMessage = "An unexpected error occurred. Please contact support.";

    public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
    {
        string traceId = httpContext.TraceIdentifier;

        try
        {
            string exceptionDetails = BuildExceptionDetails(exception);

            StackFrame? frame = new StackTrace(exception, true)
                .GetFrames()
                .FirstOrDefault(x =>
                    x.GetMethod()?.DeclaringType?.Namespace?
                        .StartsWith("SimpleAuthAPI", StringComparison.Ordinal) == true);

            MethodBase? method = frame?.GetMethod();

            string information = JsonSerializer.Serialize(new
            {
                category = "Unhandled Exception",
                trace_id = traceId,
                exception_type = exception.GetType().FullName,
                message = exceptionDetails,
                source_class = method?.DeclaringType?.FullName,
                source_method = method?.Name,
                // source_file = frame?.GetFileName(),
                // source_line = frame?.GetFileLineNumber(),
                // stack_trace = exception.ToString(),
                http_method = httpContext.Request.Method,
                path = httpContext.Request.Path.Value
            });


            // log to db
        }
        catch
        {
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            responseType = ResponseType.ServerError,
            message = UserMessage,
            data = new { trace_id = traceId }
        }, cancellationToken);

        return true;
    }

    public static string BuildExceptionDetails(Exception exception)
    {
        var details = new List<string>();
        Exception? currentException = exception;
        int level = 0;

        while (currentException is not null)
        {
            string label = level == 0 ? "Exception" : $"Inner Exception {level}";
            details.Add($"{label} : {currentException.Message}");
            currentException = currentException.InnerException;
            level++;
        }

        return string.Join(" -> ", details);
    }
}



