# NexaDesk Implementation Plan

## Purpose

NexaDesk is a hands-on project for learning C# and the .NET ecosystem while solving a realistic IT helpdesk problem. The developer's background is JavaScript/TypeScript, especially Next.js, TanStack, NestJS, and Node.js.

The plan deliberately builds one small working slice at a time. Do not implement the entire MVP in one pass. Each phase must be understood, implemented by the developer with snippets, and verified before the next phase starts.

## Current baseline

- ASP.NET Core Web API scaffold targeting `net10.0`.
- `Program.cs` currently exposes only `/weatherforecast`.
- OpenAPI is available in Development through `Microsoft.AspNetCore.OpenApi`.
- PostgreSQL is selected, but its provider, EF Core `DbContext`, migrations, authentication, tests, Docker, and business modules do not exist yet.
- The repository is a single project with no solution file.

## Folder structure (layered)

```text
Tickset/
├── Models/          ← Domain entities (User, Ticket, Role, etc.)
├── Data/            ← DbContext, migrations, configuration
├── Services/        ← Business logic (UserService, TicketService, etc.)
├── DTOs/            ← Request/Response objects (CreateUserDto, etc.)
├── Endpoints/       ← Minimal API route handlers
├── Middlewares/     ← Custom middleware
├── Enums/           ← Enumerations (TicketStatus, Priority, etc.)
└── learning-notes.md ← Knowledge base pembelajaran
```

**Aturan layered structure:**
- `Models/` → hanya entity class, tidak ada logic
- `Data/` → DbContext dan konfigurasi database
- `Services/` → business logic, akses data via `ApplicationDbContext`
- `DTOs/` → data transfer object untuk request/response
- `Endpoints/` → route handler, panggil `Services/`
- Jangan akses `DbContext` langsung dari `Endpoints/`, gunakan `Services/` sebagai perantara

### Alur antar-layer

```text
HTTP request
    ↓
Endpoints → DTOs
    ↓
Services → Models
    ↓
Data/ApplicationDbContext → PostgreSQL
    ↓
Services → response DTO
    ↓
HTTP response
```

Rules:

- `Endpoints` menangani HTTP binding, status code, dan memanggil service; jangan taruh business rule di sini.
- `Services` menangani validasi bisnis, authorization yang terkait resource, transaksi, dan mapping entity ke DTO.
- `Data` menangani `ApplicationDbContext`, EF Core configuration, migrations, dan query persistence.
- `Models` hanya mendeskripsikan entity persistence; jangan membuatnya bergantung pada service atau endpoint.
- `DTOs` menjadi kontrak API; jangan mengembalikan entity database langsung ke client.
- Daftarkan service melalui DI di `Program.cs`; jangan membuat service atau `DbContext` dengan `new` di endpoint.

## Working method

For every task, follow this loop:

1. Explain the goal and why it is needed for NexaDesk.
2. Identify the files and .NET concepts involved.
3. Compare the concept with a familiar Next.js, TanStack, NestJS, or Node.js equivalent when useful.
4. Show only the smallest relevant code snippet.
5. Let the developer write the change directly.
6. Run the focused verification command.
7. Stop at the checkpoint and explain what was learned before continuing.

Do not generate a full file, module, or application unless explicitly requested.

## Phase 0: Foundation

**Goal:** establish persistence and understand how a .NET application is configured and connected to a database.

**Start with:** PostgreSQL has been selected for this project. Do not substitute SQLite for the foundation: the target includes server-database behavior such as concurrency, schema evolution, and production-like migrations.

**Learn:** project packages, `appsettings.json`, configuration binding, dependency injection, EF Core, `DbContext`, entities, migrations, and connection strings.

**Developer tasks:**

1. Add the EF Core runtime, design package, and PostgreSQL provider separately.
2. Verify or install the `dotnet-ef` CLI tool.
3. Create a small `BaseEntity` with identity and timestamps.
4. Create `ApplicationDbContext` and register it in `Program.cs`.
5. Add a development connection string without committing secrets.
6. Create the initial migration and apply it to the database.

**Layer placement:** `BaseEntity` and persistence entities go in `Models/`; `ApplicationDbContext` and EF configuration go in `Data/`; connection registration stays in `Program.cs`.

**Comparison:** EF Core is the .NET equivalent of Prisma/Drizzle; `DbContext` is the database session/client boundary; migrations are similar to Prisma or Drizzle migrations.

**Checkpoint:** `dotnet build` succeeds and `dotnet ef database update` completes successfully.

## Phase 1: Identity

**Goal:** establish users, teams, roles, permissions, and API authentication before protecting business resources.

**Learn:** classes and records, relationships, password handling, JWT, claims, policies, model binding, and authorization.

**Developer tasks:**

1. Model `User`, `Role`, `Permission`, `Team`, and team membership.
2. Add registration and login DTOs and endpoints.
3. Issue and validate access tokens.
4. Add authorization policies for roles and permissions.

**Layer placement:** entities go in `Models/`, auth request/response records in `DTOs/`, password/token business logic in `Services/`, and HTTP routes in `Endpoints/`.

**Comparison:** ASP.NET Core DI is similar to NestJS providers; JWT claims and policies are comparable to NestJS guards or NextAuth/Auth.js session authorization, but authorization must remain enforced by the API.

**Checkpoint:** registration and login return a token, and a protected endpoint rejects unauthenticated requests.

## Phase 2: Ticketing core

**Goal:** implement the smallest useful business flow: a requester creates and reads a ticket.

**Learn:** DTOs, validation, Minimal API endpoints or controllers, status codes, LINQ, domain methods, enums, and explicit state transitions.

**Developer tasks:**

1. Model `Ticket`, `TicketType`, and `TicketCategory`.
2. Add create, list, and detail endpoints.
3. Generate incident numbers in the `INC-YYYY-NNNNNN` format.
4. Calculate priority from impact and urgency.
5. Implement allowed status transitions and status history.
6. Ensure requesters cannot resolve tickets and only resolved tickets can be closed.

**Layer placement:** ticket state and persistence entities go in `Models/`, priority/status logic belongs in a service or explicit domain component, DTOs go in `DTOs/`, and route handlers go in `Endpoints/`.

**Comparison:** Minimal API endpoints are similar to Next.js route handlers; controllers are similar to NestJS controllers; DTOs are request/response types rather than direct database models.

**Checkpoint:** creating a ticket returns its number, calculated priority, initial status, and assigned team when routing is available.

## Phase 3: Assignment and routing

**Goal:** ensure every active ticket has a clear team and owner without allowing competing claims.

**Learn:** service boundaries, resource authorization, optimistic concurrency, transactions, and conflict responses.

**Developer tasks:**

1. Route category/subcategory to a team.
2. Allow supervisors to assign an agent.
3. Allow an eligible team member to claim an unassigned ticket.
4. Add concurrency protection so two agents cannot successfully claim one ticket.
5. Record assignment history.

**Checkpoint:** the first eligible agent can claim a ticket and a competing claim receives a conflict response.

## Phase 4: SLA and escalation

**Goal:** measure response and resolution targets using business hours and escalate overdue work automatically.

**Learn:** value objects, time calculations, time zones, cancellation tokens, scoped services, hosted services, idempotency, and structured logging.

**Developer tasks:**

1. Add `SlaPolicy`, `SlaInstance`, `BusinessCalendar`, and holidays.
2. Use Monday-Friday, 08:00-17:00, Asia/Jakarta business hours.
3. Track first-response and resolution deadlines.
4. Pause resolution SLA at `PendingRequester` and resume on requester reply.
5. Add an `IHostedService` checker.
6. Fire each escalation only once at 75%, 90%, and 100%.

**Comparison:** `IHostedService` is similar to a Node.js worker or scheduled job, but it participates in .NET dependency injection and application lifetime management.

**Checkpoint:** a controlled test or time provider demonstrates the three escalation events without duplicate notifications.

## Phase 5: Communication and files

**Goal:** preserve ticket communication while enforcing visibility rules.

**Developer tasks:**

1. Add public comments and internal notes.
2. Restrict internal notes to agents and supervisors.
3. Record author, timestamps, and edit history.
4. Add secure attachments with size, type, storage, and access checks.

**Checkpoint:** requesters can see public comments but cannot read or create internal notes.

## Phase 6: Notifications and audit

**Goal:** make important changes observable and traceable.

**Developer tasks:**

1. Add in-app notifications for ticket and SLA events.
2. Add `AuditLog` for status, assignment, priority, SLA, and permission-sensitive changes.
3. Keep audit records append-oriented and include actor, action, target, timestamp, and relevant change data.

**Checkpoint:** a status or assignment change creates the expected audit record and notification.

## Phase 7: Reporting

**Goal:** expose operational information needed by agents, supervisors, and management.

**Developer tasks:**

1. Add agent views for assigned, unassigned, near-SLA, and breached tickets.
2. Add supervisor metrics for backlog, SLA compliance, response time, resolution time, and workload.
3. Add category, branch, priority, and monthly trend queries.
4. Keep reporting DTOs separate from persistence entities.

**Comparison:** the API's OpenAPI contract and DTOs provide data for TanStack Query; TanStack Router can coordinate route-driven loading; TanStack Table can render reporting results. These frontend tools do not replace API validation or authorization.

**Checkpoint:** dashboard endpoints return correct counts and metrics for seeded data.

## Phase 8: Testing and delivery

**Goal:** verify the system as a product and make it runnable by another developer.

**Developer tasks:**

1. Add a test project and unit tests for priority and status transitions first.
2. Add integration tests for authentication, ticket creation, assignment, and authorization.
3. Test SLA calculations with a controllable clock and business calendar fixtures.
4. Test concurrency and idempotent escalation behavior.
5. Add health checks, structured logging, and OpenTelemetry where useful.
6. Add Docker Compose for the API and database.
7. Document local setup and verification commands.

**Checkpoint:** focused tests pass, `dotnet build` is clean, and `docker compose up` can run the stack.

## First session checklist

Do these steps now, in order:

1. Read `Goal.md` sections 19-20 and `Context.md` sections 7-10.
2. PostgreSQL is already selected; continue with the PostgreSQL provider.
3. Check the installed .NET SDK with `dotnet --info`.
4. Check whether `dotnet-ef` is installed with `dotnet ef`.
5. Add only the Phase 0 EF Core packages required by PostgreSQL.
6. Ask for the first small snippet: the `BaseEntity` shape and why it is useful.
7. Write the entity yourself, then run `dotnet build`.
8. Continue with `ApplicationDbContext`, connection configuration, migration, and database update one step at a time.

Do not start Identity, Ticketing, or frontend integration until Phase 0 has been understood and its checkpoint passes.

## Domain guardrails

- Business hours are Monday-Friday, 08:00-17:00, Asia/Jakarta.
- Priority is calculated from impact and urgency.
- The primary workflow is `New -> Open -> InProgress -> Resolved -> Closed`, with pending, reopened, and cancelled transitions defined in `Goal.md`.
- Every status change requires history.
- Requesters cannot resolve tickets; only resolved tickets can be closed.
- Resolution SLA pauses at `PendingRequester` and resumes when the requester replies.
- Escalations at 75%, 90%, and 100% each execute once.
- Requesters see public comments only; internal notes are for agents and supervisors.
- MVP excludes email and WhatsApp intake, chatbot, AI classification, mobile app, multi-tenancy, problem/change management, and multi-level approvals.
