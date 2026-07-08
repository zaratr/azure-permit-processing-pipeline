namespace Permit.Api.Models;

/// <summary>
/// Domain entity for a persisted permit application.
///
/// Distinct from <see cref="PermitRequestMessage"/> (the wire/queue DTO): the
/// request message is the minimal payload enqueued for async processing, while
/// this entity is the full record the API persists and the dashboard reads.
/// </summary>
public class PermitApplication
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public string ApplicantEmail { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty;
    public PermitStatus Status { get; set; } = PermitStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdatedAt { get; set; }
}
