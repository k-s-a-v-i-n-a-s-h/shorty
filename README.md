# Shorty

A high-performance URL shortener.

## Tech Stack

- **Proxy**: [Caddy](./infra/Caddyfile)
- **Frontend**: [Next.js (React)](./src/Shorty.Frontend)
- **Management**: [C# (.NET 10 / EF Core)](./src/Shorty.Management)
- **Redirection**: [Rust (Axum / SQLx)](./src/Shorty.Engine)
- **Messaging**: NATS
- **Storage**: SQLite
- **Infrastructure**: [Docker Compose](./infra/docker-compose.yml)
- **Benchmarks**: k6

## Performance

Results from a 1:4 Write/Read parallel load test (100 VUs).  
*Hardware: i7-13700HX (16 Cores), 16GB RAM*


| Metric | Result |
| :--- | :--- |
| **Throughput** | 31.46k req/s |
| **P95 Latency** | 5ms |
| **P99 Latency** | 12ms |
| **Success Rate** | 99.1% |

Test scripts and raw reports are available in the [`/benchmarks`](./benchmarks) directory.
