using Microsoft.AspNetCore.Mvc;

namespace CloudNativeApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<StatusController> _logger;

    public StatusController(IConfiguration configuration, ILogger<StatusController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("health")]
    public IActionResult GetHealth([FromQuery] bool simulateCrash = false)
    {
        _logger.LogInformation("Health-check anropades. Applikationen är vaken.");

        if (simulateCrash)
        {
            _logger.LogError("Ett simulerat fel framkallades via health-endpointen!");
            throw new Exception("Kritiskt fel: Simulerad krasch för Application Insights!");
        }

        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }

    [HttpGet("secret")]
    public IActionResult GetSecret()
    {
        _logger.LogInformation("Försöker hämta API-nyckel från konfigurationen...");
        var secretValue = _configuration["ExternalServices:VendorApiKey"];

        if (string.IsNullOrEmpty(secretValue))
        {
            _logger.LogWarning("Hemligheten 'ExternalServices:VendorApiKey' kunde inte hittas eller är tom!");
            return NotFound("Ingen hemlighet hittades. Kontrollera Key Vault och Managed Identity-rättigheter.");
        }

        return Ok(new { SecretMessage = secretValue, Message = "Hemlighet hämtades framgångsrikt från Key Vault!" });
    }
}