
using System.Runtime;
using GCoptimizedWebAPI.Services;
using GCoptimizedWebAPI.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure GC settings based on environment
if (builder.Environment.IsProduction())
{
    GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
    Console.WriteLine($"GC configured for production: {GCSettings.LatencyMode}");
}
else
{
    // Development settings
    GCSettings.LatencyMode = GCLatencyMode.Interactive;
    Console.WriteLine($"GC configured for development: {GCSettings.LatencyMode}");
}


// Add our memory-intensive service
builder.Services.AddSingleton<IMemoryIntensiveService, MemoryIntensiveService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMiddleware<GCMonitoringMiddleware>(); // Only in dev for diagnostics
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Configure GC for the application lifetime
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine($"GC Server Mode: {GCSettings.IsServerGC}");
    Console.WriteLine($"GC Latency Mode: {GCSettings.LatencyMode}");
});



app.Run();