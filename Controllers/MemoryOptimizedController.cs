using System.Runtime;
using GCoptimizedWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GCoptimizedWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemoryOptimizedController : ControllerBase
    {
        private readonly IMemoryIntensiveService _memoryService;
        private readonly ILogger<MemoryOptimizedController> _logger;

        public MemoryOptimizedController(
            IMemoryIntensiveService memoryService,
            ILogger<MemoryOptimizedController> logger)
        {
            _memoryService = memoryService;
            _logger = logger;
        }

        [HttpGet("process-data/{size}")]
        public IActionResult ProcessData(int size)
        {
            try
            {
                // Use ArrayPool for temporary large buffers
                var result = _memoryService.ProcessLargeData(size);
                return Ok(new { ResultLength = result.Length });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing data");
                return StatusCode(500);
            }
        }

        [HttpGet("memory-info")]
        public IActionResult GetMemoryInfo()
        {
            var info = new
            {
                TotalMemory = GC.GetTotalMemory(false) / 1024 / 1024 + " MB",
                Gen0Collections = GC.CollectionCount(0),
                Gen1Collections = GC.CollectionCount(1),
                Gen2Collections = GC.CollectionCount(2),
                IsServerGC = GCSettings.IsServerGC,
                LatencyMode = GCSettings.LatencyMode.ToString()
            };

            return Ok(info);
        }

        [HttpPost("optimize-gc")]
        public IActionResult OptimizeGC([FromQuery] bool forceFullCollection = false)
        {
            if (forceFullCollection)
            {
                // Only for demonstration - not recommended in production
                GC.Collect(2, GCCollectionMode.Forced, true);
                GC.WaitForPendingFinalizers();
            }

            return Ok(new { 
                Status = "GC optimized",
                MemoryAfter = GC.GetTotalMemory(false) / 1024 / 1024 + " MB" 
            });
        }
    }
}