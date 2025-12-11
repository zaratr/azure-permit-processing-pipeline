namespace Permit.Api.Models
{
    public class PermitRequestMessage
    {
        public int ApplicationId { get; set; }
        public string ApplicantEmail { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
    }
}
