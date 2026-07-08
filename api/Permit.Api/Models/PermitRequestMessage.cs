using System.ComponentModel.DataAnnotations;

namespace Permit.Api.Models
{
    /// <summary>
    /// Wire/queue DTO for a permit request — the minimal payload enqueued for
    /// async processing by the Azure Function. Validation annotations enforce
    /// field-level integrity before the message is accepted.
    /// </summary>
    public class PermitRequestMessage
    {
        [Range(1, int.MaxValue, ErrorMessage = "ApplicationId must be a positive integer.")]
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "ApplicantEmail is required.")]
        [EmailAddress(ErrorMessage = "ApplicantEmail must be a valid email address.")]
        public string ApplicantEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "LicenseType is required.")]
        public string LicenseType { get; set; } = string.Empty;
    }
}
