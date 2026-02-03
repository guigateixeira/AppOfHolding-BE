using Microsoft.AspNetCore.Mvc;

namespace AppOfHolding.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public HealthCheckController(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var healthInfo = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Environment = _environment.EnvironmentName,
            Application = new
            {
                Name = _configuration["App:Name"] ?? "AppOfHolding",
                Version = _configuration["App:Version"] ?? "1.0.0"
            }
        };

        return Ok(healthInfo);
    }

    [HttpGet("detailed")]
    public IActionResult GetDetailed()
    {
        var detailedInfo = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Environment = _environment.EnvironmentName,
            Application = new
            {
                Name = _configuration["App:Name"] ?? "AppOfHolding",
                Version = _configuration["App:Version"] ?? "1.0.0"
            },
            Server = new
            {
                MachineName = System.Environment.MachineName,
                OSVersion = System.Environment.OSVersion.ToString(),
                ProcessorCount = System.Environment.ProcessorCount,
                DotNetVersion = System.Environment.Version.ToString()
            },
            Configuration = new
            {
                Port = _configuration["App:Port"] ?? _configuration["ASPNETCORE_URLS"] ?? "5000",
                ContentRootPath = _environment.ContentRootPath,
                WebRootPath = _environment.WebRootPath
            }
        };

        return Ok(detailedInfo);
    }
}
