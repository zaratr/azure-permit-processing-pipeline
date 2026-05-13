# Azure Permit Processing Pipeline

Event-driven permit processing on Azure — Web API → Storage Queues → Functions — upgraded in 2026 with an **eBPF zero-instrumentation observability layer** that traces network latency and cold starts at the Linux kernel level with zero code changes to the application.

**Skills demonstrated:** Azure Functions · Azure Storage Queues · .NET · Angular · event-driven architecture · eBPF kernel observability · Prometheus · Grafana

## 2026 Update: eBPF Zero-Instrumentation Observability

See [`observability/README.md`](observability/README.md) for full details.

Instead of adding OpenTelemetry SDK calls to the .NET API or Function host, eBPF probes hook directly into the Linux kernel to capture:
- TCP socket RTT for every call from the API to Azure Storage queues
- Function cold-start detection via process exec events
- Network retransmit detection — no application-side instrumentation required

```
Permit API ──▶ Linux kernel eBPF probes ──▶ BPF ring buffer
Azure Function ──────────────────────────────────┘
                                                 │
                                          user-space reader
                                                 │
                                      Prometheus :8000 ──▶ Grafana
```

A pre-built Grafana dashboard (`observability/ebpf-grafana-dashboard.json`) provides p50/p95/p99 latency panels for the queue-to-function processing path.

## Architecture

```
[Dashboard UI] --calls--> [Permit API] --enqueues--> [Azure Storage Queue: permit-requests]
                                       ^                                     |
                                       |                                     v
                               (polls for status)                 [Azure Function Processor]
                                                                           |
                                                                           v
                                                                  [SQL / Logging]
```

### How the queue works
1. The Web API exposes `POST /api/queue/enqueue` which accepts a permit request payload.
2. The payload is serialized to JSON, Base64 encoded, and sent to the `permit-requests` queue in Azure Storage.
3. An Azure Function subscribes to the queue via `QueueTrigger("permit-requests")` and processes each message, simulating database updates and status notifications.

## Running the API locally
1. Navigate to `api/Permit.Api`.
2. Provide an Azure Storage connection string via `AzureStorage:ConnectionString` in `appsettings.json` or `ConnectionStrings:Storage` in environment variables.
3. Restore and run the API (for example with `dotnet restore` then `dotnet run`).
4. Test enqueueing with:
   ```bash
   curl -X POST http://localhost:5000/api/queue/enqueue \
     -H "Content-Type: application/json" \
     -d '{"applicationId":1001,"applicantEmail":"user@example.com","licenseType":"Electrical"}'
   ```

## Running the Azure Function locally
1. Navigate to `functions/PermitProcessor.Function`.
2. Ensure `local.settings.json` contains `AzureWebJobsStorage` pointing to your Azurite/emulator or Azure Storage.
3. Start the Functions host with `func start` (Azure Functions Core Tools required).
4. Watch the console logs for processed queue messages.

## Configure Azure Storage Emulator (Azurite)
1. Install Azurite (`npm install -g azurite`).
2. Start Azurite: `azurite --location ./azurite --debug azurite.log`.
3. Use the connection string `UseDevelopmentStorage=true` in both the API and Functions settings to target the emulator.

## Running the Angular dashboard
1. Navigate to `dashboard/permit-dashboard-ui`.
2. Install dependencies with `npm install` (requires Node.js and Angular CLI).
3. Update `apiBaseUrl` in `src/app/core/api.service.ts` if your API host differs.
4. Run `ng serve` and open the dashboard in your browser (typically `http://localhost:4200`).
5. The dashboard polls for permit lists and statuses while providing an entry point to enqueue new requests.

## Solutions and Projects
- `api/Permit.Api` – ASP.NET Core Web API that enqueues permit requests.
- `functions/PermitProcessor.Function` – Azure Function that processes queued permit messages.
- `dashboard/permit-dashboard-ui` – Angular UI that lists permits and monitors their statuses.
