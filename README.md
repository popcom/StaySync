# StaySync — Hotel Room Occupancy API (Clean Architecture, .NET 8)

> A production-minded reference implementation for assigning travellers to rooms and querying hotel occupancy — built with **Clean Architecture**, **CQRS**, **EF Core (writes)** + **Dapper (reads)**, full **unit & integration tests**, and **Docker + GitHub Actions**.


[![Build](https://img.shields.io/github/actions/workflow/status/popcom/StaySync/ci.yml?branch=main)](https://github.com/popcom/StaySync/actions)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](#license)


## Table of Contents

* [Features](#features)
* [Architecture](#architecture)
* [Project Layout](#project-layout)
* [Quick Start](#quick-start)
* [Configuration](#configuration)
* [Database, Migrations & Seed](#database-migrations--seed)
* [API Key Auth](#api-key-auth)
* [Endpoints](#endpoints)
* [Examples](#examples)
* [Testing](#testing)
* [CI/CD](#cicd)
* [Docker](#docker)
* [Observability & Health](#observability--health)
* [Design Decisions (Highlights)](#design-decisions-highlights)
* [Troubleshooting](#troubleshooting)
* [License](#license)

---

## Features

* ✅ **Acceptance criteria covered**

  * List **rooms to be occupied today** (hotel-local “today”)
  * List **all rooms in a travel group**
  * Get **an individual room**
  * **Move a traveller** from one room to another (never over-occupied)
* 🧱 **Clean Architecture + CQRS** (thin controllers, testable handlers)
* 🗄️ **EF Core 8** for transactional writes, **Dapper** for fast read models
* 🔐 **API Key** per hotel (hashed at rest), strict **hotel scoping** on all queries
* 🌍 **Hotel-local time** using IANA/Windows time zones (via TimeZoneConverter)
* 🔄 **Concurrency safety** with `rowversion` (shadow concurrency token)
* 🧪 **Unit tests** + **Integration tests** with **Testcontainers (SQL Server)**
* 📦 **Dockerized** runtime & **GitHub Actions** pipeline

---

## Architecture

**Tech stack:** .NET 8, ASP.NET Core Web API (controllers), EF Core (SqlServer), Dapper, FluentValidation, TimeZoneConverter, xUnit/FluentAssertions/Moq, Testcontainers.

**Read/Write split**

* **Commands (writes)** → EF Core + UoW
* **Queries (reads)** → Dapper projections with parameterized SQL

**Multi-tenancy**

* Every table is **scoped by `HotelId`**. All queries/commands filter by the current hotel derived from the **API key**.

**“Today”**

* Computed with a **hotel-local clock** using the hotel’s timezone. `DateOnly` is stored as `date` in SQL.

```
Client ──(X-Api-Key)──> Web API (Controllers)
                         │
                         ▼
                 IDispatcher (CQRS)
               ┌──────────┴──────────┐
               │ Queries (Dapper)    │
               │ Commands (EF Core)  │
               └──────────┬──────────┘
                          ▼
                     SQL Server
```

---

## Project Layout

```
.
├─ src/
│  ├─ StaySync.Domain/            # Entities, Value Objects, Domain exceptions
│  ├─ StaySync.Application/       # CQRS handlers, DTOs, validators, clock/context
│  ├─ StaySync.Infrastructure/    # EF DbContext, configs, repositories, UoW
│  └─ StaySync.WebApi/            # Controllers, middleware, DI composition
├─ tests/
│  ├─ StaySync.UnitTests/
│  ├─ StaySync.WebApi.UnitTests/
│  └─ StaySync.IntegrationTests/  # Spins up SQL Server via Testcontainers
└─ StaySync.sln
```

---

## Quick Start

### Prerequisites

* **.NET 8 SDK**
* **Docker** (for integration tests and containerized run)
* **SQL Server** (local instance or via Docker)

### Run locally (with your SQL)

1. Set env vars (Windows PowerShell examples):

```powershell
$env:ConnectionStrings__Sql="Server=localhost,1433;Database=StaySync;User Id=...;Password=...;Encrypt=False;TrustServerCertificate=True"
$env:Seed__Enabled="true"   # first run only to auto-migrate & seed demo data
dotnet run --project src/StaySync.WebApi/StaySync.WebApi.csproj
```

2. Open **[http://localhost:5211/swagger](http://localhost:5211/swagger)**
3. Use `X-Api-Key: demo-key` (from seed) to call endpoints.

### Run with Docker Compose (API + SQL)

```bash
docker compose up --build
# API → http://localhost:8080/swagger
# Use header: X-Api-Key: demo-key
```

---

## Configuration

| Key                      | Description                                    | Example                                                                                               |
| ------------------------ | ---------------------------------------------- | ----------------------------------------------------------------------------------------------------- |
| `ConnectionStrings__Sql` | SQL Server connection string                   | `Server=sql,1433;Database=StaySync;User Id=...;Password=...;Encrypt=False;TrustServerCertificate=True` |
| `ASPNETCORE_ENVIRONMENT` | Hosting environment                            | `Development` / `Production`                                                                          |
| `Seed__Enabled`          | Auto migrate & seed on startup (dev/test only) | `true`                                                                                                |

> 12-factor config: **no secrets in code**. Use env vars or platform secret stores. Double underscores map to `:` (e.g., `ConnectionStrings__Sql` → `ConnectionStrings:Sql`).

---

## Database, Migrations & Seed

* Schema uses precise types: `char(4)` (`RoomCode`), `char(6)` (`GroupId`), `date` for DOB/arrival/assignment.
* **Unique indexes**: `(HotelId, RoomCode)`, `(HotelId, GroupId)`, `(GroupId, Surname, FirstName, DateOfBirth)`, `(RoomId, AssignedOnDate, TravellerId)`.
* **Seed** (dev/test): creates one demo hotel (API key `demo-key`), a small set of rooms, a sample travel group, and assignments for **hotel-local today**.

Migrations:

```bash
# Add (from Infrastructure folder)
dotnet ef migrations add Initial --context StaySyncDbContext --output-dir Persistence/Migrations

# Update database (dev)
dotnet ef database update --context StaySyncDbContext
```

> Production: run migrations once as a job/init step before starting the API.

---

## API Key Auth

* Every request must include `X-Api-Key: <key>`.
* Keys are stored as **SHA-256 hashes**; the middleware resolves the hotel and populates a request-scoped **CurrentHotelContext** (hotel id + timezone).

---

## Endpoints

Base path: `/api/v1`

| Method | Route                     | Description                                                     |
| ------ | ------------------------- | --------------------------------------------------------------- |
| `GET`  | `/rooms/occupancy/today`  | Rooms to be occupied **today** (hotel-local)                    |
| `GET`  | `/rooms/{roomCode}`       | Details of a single room (travellers, bed count)                |
| `GET`  | `/groups/{groupId}/rooms` | All rooms in a travel group                                     |
| `POST` | `/rooms/move`             | Move a traveller between rooms (same date), never over-occupied |

---

## Examples

### 1) Today’s occupancy

```bash
curl -s -H "X-Api-Key: demo-key" http://localhost:8080/api/v1/rooms/occupancy/today
```

### 2) Room details

```bash
curl -s -H "X-Api-Key: demo-key" http://localhost:8080/api/v1/rooms/0101
```

### 3) Move a traveller (hotel-local today)

```bash
DATE=$(curl -s -H "X-Api-Key: demo-key" http://localhost:8080/api/v1/rooms/occupancy/today | jq -r .date)

curl -s -X POST http://localhost:8080/api/v1/rooms/move \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: demo-key" \
  -d "{
        \"groupId\": \"A12B34\",
        \"surname\": \"Doe\",
        \"firstName\": \"John\",
        \"dateOfBirth\": \"1988-03-02\",
        \"fromRoomCode\": \"0101\",
        \"toRoomCode\": \"0102\",
        \"assignedOnDate\": \"$DATE\"
      }"
```

---

## Testing

### Unit tests

```bash
dotnet test tests/StaySync.UnitTests/StaySync.UnitTests.csproj -c Release
dotnet test tests/StaySync.WebApi.UnitTests/StaySync.WebApi.UnitTests.csproj -c Release
```

### Integration tests (spins up SQL via Testcontainers)

```bash
dotnet test tests/StaySync.IntegrationTests/StaySync.IntegrationTests.csproj -c Release
```

Coverage philosophy: protect **contracts** (DTOs/validators), **rules** (capacity, not-found), and **edges** (auth, date/time, concurrency). Integration tests verify the real pipeline: middleware → handlers → EF/Dapper → SQL.

---

## CI/CD

**GitHub Actions** workflow (`.github/workflows/ci.yml`) does:

* On **PR**: restore, build, run unit + integration tests.
* On push to **main**: same tests, then build a **Docker image** and push to **GHCR**:

  * `ghcr.io/popcom/staysync-webapi:latest`
  * `ghcr.io/popcom/staysync-webapi:sha-6b08ee6033f50103c3649afd2b20414ac287725b`

Use any platform (Compose/Kubernetes) to deploy the immutable image. Rollback by redeploying the previous tag.

---

## Docker

### Build & run the API (local SQL)

```bash
docker build -t staysync-webapi:local -f src/StaySync.WebApi/Dockerfile .
docker run -d -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__Sql="Server=host.docker.internal,1433;Database=StaySync;User Id=...;Password...;Encrypt=False;TrustServerCertificate=True" \
  -e Seed__Enabled=true \
  staysync-webapi:local
```

### Compose (API + SQL Server)

```bash
docker compose up --build
# Open http://localhost:8080/swagger (X-Api-Key: demo-key)
```

---

## Observability & Health

* **Correlation ID**: requests return/set `X-Request-Id` header and include it in logs.
* **Health checks**: `/health` (liveness). You can add `/ready` for readiness.
* Structured logging contains hotel id, route, status, and latency.

---

## Design Decisions (Highlights)

* **EF writes + Dapper reads** for a pragmatic CQRS split.
* **Hotel scoping** on every table + per-hotel unique constraints.
* **Hotel-local “today”** (IANA/Windows handled via **TimeZoneConverter**).
* **Never over-occupied** enforced by transaction + unique/index constraints.
* **ProblemDetails** (RFC 7807) for consistent error payloads.

---

## Troubleshooting

* **401 Unauthorized** → Missing/invalid `X-Api-Key`.
* **404 on move** → `assignedOnDate` must be **hotel-local today** (use `/rooms/occupancy/today`’s `date`).
* **DB connection error** → Verify `ConnectionStrings__Sql`, and for local dev add `Encrypt=False;TrustServerCertificate=True`.
* **Port in use** → change `-p 8080:8080` to another host port.
* **GHCR image push fails (lowercase)** → ensure image path is all lowercase.

---

## License

MIT — see [LICENSE](./LICENSE).

---

**Tip:** If you want a one-liner to try the service with Docker (API + SQL), use `docker compose up --build`, then open **Swagger** and send requests with `X-Api-Key: demo-key`.
