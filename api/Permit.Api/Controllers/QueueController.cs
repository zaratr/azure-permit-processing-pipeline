using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Permit.Api.Models;

namespace Permit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController : ControllerBase
    {
        private const string QueueName = "permit-requests";
        private readonly ILogger<QueueController> _logger;
        private readonly IConfiguration _configuration;

        public QueueController(IConfiguration configuration, ILogger<QueueController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("enqueue")]
        public async Task<IActionResult> Enqueue([FromBody] PermitRequestMessage request)
        {
            if (request == null)
            {
                return BadRequest("A permit request payload is required.");
            }

            var connectionString = _configuration.GetConnectionString("Storage")
                                   ?? _configuration["AzureStorage:ConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                const string missingSettingMessage = "Azure Storage connection string is not configured.";
                _logger.LogError(missingSettingMessage);
                return StatusCode(StatusCodes.Status500InternalServerError, missingSettingMessage);
            }

            var queueClient = new QueueClient(connectionString, QueueName);
            await queueClient.CreateIfNotExistsAsync();

            var payload = JsonSerializer.Serialize(request);
            var encodedPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
            await queueClient.SendMessageAsync(encodedPayload);

            _logger.LogInformation("Enqueued permit request {ApplicationId} for {LicenseType}", request.ApplicationId, request.LicenseType);
            return Accepted(new { request.ApplicationId, Queue = QueueName });
        }
    }
}
