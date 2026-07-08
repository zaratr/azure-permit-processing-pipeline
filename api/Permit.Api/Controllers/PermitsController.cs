using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Permit.Api.Data;
using Permit.Api.Models;

namespace Permit.Api.Controllers;

/// <summary>
/// Read endpoints for permit applications — the surface the Angular dashboard
/// already calls (GET /api/permits, GET /api/permits/{id}/status).
///
/// Previously these endpoints did not exist, so the dashboard's list and status
/// views pointed at dead URLs. This controller backs them with the registered
/// PermitDbContext (in-memory provider for dev/test).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PermitsController : ControllerBase
{
    private readonly PermitDbContext _db;
    private readonly ILogger<PermitsController> _logger;

    public PermitsController(PermitDbContext db, ILogger<PermitsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// List all permit applications (drives the dashboard's permit-list view).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermitApplication>>> GetPermits()
    {
        var permits = await _db.PermitApplications
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return Ok(permits);
    }

    /// <summary>
    /// Get the status of a single permit by its ApplicationId
    /// (drives the dashboard's permit-status polling view).
    /// </summary>
    [HttpGet("{applicationId}/status")]
    public async Task<ActionResult<object>> GetPermitStatus(int applicationId)
    {
        var permit = await _db.PermitApplications
            .FirstOrDefaultAsync(p => p.ApplicationId == applicationId);

        if (permit is null)
        {
            return NotFound(new { applicationId, status = "not_found" });
        }

        return Ok(new
        {
            permit.ApplicationId,
            Status = permit.Status.ToString(),
            permit.LicenseType,
            permit.LastUpdatedAt,
        });
    }
}
