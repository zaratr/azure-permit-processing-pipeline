using System;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace PermitProcessor.Function
{
    public class PermitProcessor
    {
        private readonly ILogger<PermitProcessor> _logger;

        public PermitProcessor(ILogger<PermitProcessor> logger)
        {
            _logger = logger;
        }

        [FunctionName("PermitProcessor")]
        public void Run([QueueTrigger("permit-requests", Connection = "AzureWebJobsStorage")] string queueItem)
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(queueItem));
            var permitRequest = JsonSerializer.Deserialize<PermitRequestMessage>(decoded);

            if (permitRequest == null)
            {
                _logger.LogWarning("Received malformed permit request: {Decoded}", decoded);
                return;
            }

            _logger.LogInformation("Processing permit request {ApplicationId} for {LicenseType}", permitRequest.ApplicationId, permitRequest.LicenseType);

            // Pretend database update or any downstream processing
            _logger.LogInformation("Updating SQL records for application {ApplicationId}", permitRequest.ApplicationId);
            _logger.LogInformation("Emailing {Email} with updated status", permitRequest.ApplicantEmail);
        }
    }
}
