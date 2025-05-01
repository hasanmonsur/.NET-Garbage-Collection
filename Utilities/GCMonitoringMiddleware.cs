using System.Diagnostics;

namespace GCoptimizedWebAPI.Utilities
{
    public class GCMonitoringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GCMonitoringMiddleware> _logger;

        public GCMonitoringMiddleware(RequestDelegate next, ILogger<GCMonitoringMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var initialGen0 = GC.CollectionCount(0);
            var initialGen1 = GC.CollectionCount(1);
            var initialGen2 = GC.CollectionCount(2);
            var initialMemory = GC.GetTotalMemory(false);

            await _next(context);

            stopwatch.Stop();
            
            var gen0 = GC.CollectionCount(0) - initialGen0;
            var gen1 = GC.CollectionCount(1) - initialGen1;
            var gen2 = GC.CollectionCount(2) - initialGen2;
            var memoryUsed = (GC.GetTotalMemory(false) - initialMemory) / 1024;

            _logger.LogInformation(
                "Request: {Method} {Path} completed in {ElapsedMs}ms. " +
                "GC: Gen0={Gen0}, Gen1={Gen1}, Gen2={Gen2}. " +
                "Memory: {MemoryUsed}KB",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                gen0,
                gen1,
                gen2,
                memoryUsed);
        }
    }
}