# eBPF Observability Layer

Zero-instrumentation monitoring for the Azure Permit Processing Pipeline using eBPF kernel probes.

## Why eBPF in 2026

Traditional observability requires SDK instrumentation — adding OpenTelemetry calls, logging statements, and tracing spans to application code. Every new service or language needs its own SDK. eBPF eliminates this:

- **Zero code changes** to the .NET API or Azure Functions
- **Kernel-level visibility** — probes `tcp_sendmsg` / `tcp_cleanup_rbuf` directly
- **Sub-microsecond overhead** at idle (BPF programs run in the kernel, not as sidecars)
- **Any language, any process** — Python, .NET, Node — all traced the same way

## What Is Monitored

| Signal | Source | How |
|---|---|---|
| TCP socket RTT | Permit API → Azure Storage | `tcp_sendmsg` / `tcp_cleanup_rbuf` kprobes |
| Function cold starts | Azure Function host process | `sched_process_exec` tracepoint |
| Network retransmits | All processes | `tcp_retransmit_skb` kprobe |
| Queue-to-function latency | Message enqueue → Function log | Correlated timestamp events |

## Quick Start

### Prerequisites (Linux only — kernel 5.8+)
```bash
# Ubuntu / Debian
sudo apt install bpfcc-tools linux-headers-$(uname -r)
pip install bcc prometheus_client
```

### Run the monitor
```bash
# Real eBPF mode (requires root or CAP_BPF)
sudo python3 observability/ebpf_permit_monitor.py 8000

# Simulation mode (no root needed — for dev/demo)
python3 observability/ebpf_permit_monitor.py 8000
```

Prometheus metrics are served at `http://localhost:8000`.

### Import the Grafana dashboard
In Grafana → Dashboards → Import → upload `observability/ebpf-grafana-dashboard.json`.
Datasource: your Prometheus instance scraping `:8000`.

## Architecture

```
Permit API (dotnet) ──┐
                      ├──▶  Linux kernel eBPF probes
Azure Function (func)─┘         │
                           BPF ring buffer
                                │
                      user-space reader (this script)
                                │
                       Prometheus /metrics
                                │
                          Grafana dashboard
```

## Production Deployment Notes

In a real Azure deployment, run this as a DaemonSet pod in AKS or as a systemd service on the VM hosting the Function host. The eBPF program is loaded once per node and traces all processes without per-service configuration.

For Windows-hosted Azure Functions: use Azure Monitor Network Insights + Connection Monitor as the eBPF equivalent (eBPF requires Linux kernel).
