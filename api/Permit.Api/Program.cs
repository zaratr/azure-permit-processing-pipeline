using Microsoft.EntityFrameworkCore;
using Permit.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS — the Angular dashboard runs on a different origin (localhost:4200)
// and needs to call the API. Without this, the dashboard's reads are blocked.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ── Persistence ─────────────────────────────────────────────────────────────
// In-memory provider for local dev/test — no SQL Server required to run the API.
// A production deploy swaps this for UseSqlServer(connectionString).
// The DbContext is now registered and reachable by the PermitsController
// (previously it existed but was never added to DI — dead code).
builder.Services.AddDbContext<PermitDbContext>(options =>
    options.UseInMemoryDatabase("permit-processing"));

var app = builder.Build();

// Ensure the seed data (defined in PermitDbContext.OnModelCreating) is present
// so the dashboard's GET /permits has rows to render on first run.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PermitDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Mark partial so WebApplicationFactory<Program> can reference the generated
// Program class in tests (required by the .NET 8 minimal-hosting pattern).
public partial class Program { }
