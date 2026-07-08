namespace Permit.Api.Models;

/// <summary>
/// Lifecycle states for a permit application.
/// Mirrors the queue flow: Draft → Submitted → Reviewing → Approved/Rejected.
/// </summary>
public enum PermitStatus
{
    Draft = 0,
    Submitted = 1,
    Reviewing = 2,
    Approved = 3,
    Rejected = 4,
}
