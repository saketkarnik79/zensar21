
using System.Diagnostics;

namespace ASP_DemoPageBasedWebApp.Middlewares
{
    internal class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();
            var method = context.Request.Method;
            var path = context.Request.Path;
            var elapsedTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"[{DateTime.Now}] {method} {path} - {elapsedTime} ms");
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
