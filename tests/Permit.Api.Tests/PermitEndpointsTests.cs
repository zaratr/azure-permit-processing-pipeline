using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Permit.Api.Tests;

/// <summary>
/// End-to-end tests for the permit API using WebApplicationFactory.
///
/// These verify the two previously-missing GET endpoints (which the Angular
/// dashboard was calling against dead URLs) and the validation on the enqueue
/// path. The API runs with the in-memory EF provider, so no external DB is
/// required.
/// </summary>
public class PermitEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PermitEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPermits_ReturnsSeededData()
    {
        // The dashboard's permit-list view calls GET /api/permits — this was
        // previously a dead endpoint. It must now return the seeded rows.
        var response = await _client.GetAsync("/api/permits");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var permits = await response.Content.ReadFromJsonAsync<List<PermitApplicationDto>>();
        Assert.NotNull(permits);
        Assert.NotEmpty(permits);  // seed data present
    }

    [Fact]
    public async Task GetPermitStatus_ReturnsStatus_ForExistingApplication()
    {
        // The dashboard's permit-status view calls GET /api/permits/{id}/status.
        // Seed application 1001 exists; it must return a status.
        var response = await _client.GetAsync("/api/permits/1001/status");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<PermitStatusDto>();
        Assert.NotNull(body);
        Assert.Equal(1001, body!.ApplicationId);
        Assert.False(string.IsNullOrEmpty(body.Status));
    }

    [Fact]
    public async Task GetPermitStatus_ReturnsNotFound_ForMissingApplication()
    {
        var response = await _client.GetAsync("/api/permits/9999/status");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Enqueue_RejectsInvalidEmail()
    {
        // Validation annotations on PermitRequestMessage must reject a bad email.
        var response = await _client.PostAsJsonAsync("/api/queue/enqueue", new
        {
            ApplicationId = 1,
            ApplicantEmail = "not-an-email",
            LicenseType = "Test",
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Enqueue_RejectsZeroApplicationId()
    {
        var response = await _client.PostAsJsonAsync("/api/queue/enqueue", new
        {
            ApplicationId = 0,
            ApplicantEmail = "valid@example.com",
            LicenseType = "Test",
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task HealthCheck_IsNotRequired_ButApiBoots()
    {
        // The API must boot and serve at least one endpoint without crashing —
        // confirms the DbContext registration + seed EnsureCreated works.
        var response = await _client.GetAsync("/api/permits");
        Assert.True(response.IsSuccessStatusCode);
    }

    // Lightweight DTOs for deserialization (don't couple tests to the API models).
    private record PermitApplicationDto(int Id, int ApplicationId, string ApplicantEmail, string LicenseType);
    private record PermitStatusDto(int ApplicationId, string Status);
}
