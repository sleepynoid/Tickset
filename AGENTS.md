# AGENTS.md - NexaDesk Helpdesk/Ticketing System

## Project context

NexaDesk is a planned modular-monolith ASP.NET Core Web API for PT Arunika Distribusi Nusantara. The domain is centralized IT helpdesk/ticketing for incidents and service requests, including routing, assignment, SLA, escalation, audit, and reporting.

This is also a learning project for a developer coming from the JavaScript/TypeScript ecosystem, especially Next.js, TanStack, NestJS, and Node.js. Prefer idiomatic C#/.NET patterns, but explain them against familiar JS/TS concepts when useful.

## Current state

- Fresh ASP.NET Core Web API scaffold; `Program.cs` still exposes only `/weatherforecast`.
- Target framework is `net10.0`; nullable reference types and implicit usings are enabled.
- The only direct package is `Microsoft.AspNetCore.OpenApi` version `10.0.12`.
- PostgreSQL is the selected database, but its provider, EF Core `DbContext`, migrations, authentication, test project, Docker setup, and business modules do not exist yet.
- This is a single project; there is no solution file or workspace package manager.

## Learning instructions

- Treat this repository as a hands-on C# and .NET learning project, not only a feature-delivery project. The developer's background is JavaScript/TypeScript, NestJS, and Laravel, so explain unfamiliar concepts using those ecosystems as context without replacing idiomatic .NET patterns.
- **Perbandingan utama: Laravel.** Gunakan Laravel sebagai pembanding utama untuk semua penjelasan. Tambahkan pembanding lain (NestJS, Prisma, Next.js, TanStack) hanya jika relevan.
- Use these ecosystem comparisons when relevant: ASP.NET Core Minimal APIs/controllers are comparable to Laravel routes/controllers; ASP.NET Core dependency injection is comparable to Laravel service providers; EF Core is comparable to Eloquent ORM; `appsettings.json` is comparable to Laravel `.env`/config; middleware and endpoint filters are comparable to Laravel middleware; `IHostedService` is comparable to Laravel queue workers/scheduled tasks.
- For frontend/API consumption, explain how ASP.NET Core APIs, OpenAPI, and DTOs fit with TanStack Query for server-state fetching/caching, TanStack Router for route-driven data loading, and TanStack Table for reporting data. Do not assume TanStack replaces backend validation, authorization, or business rules.
- Explain C# features such as records, pattern matching, nullable reference types, and dependency injection in contrast to TypeScript where relevant.
- Use this project to learn .NET-specific patterns: minimal hosting in `Program.cs`, middleware ordering, `appsettings.json`, `IHostedService`, EF Core migrations, configuration, and structured logging.
- Generate snippets only: show the smallest key fragment, explain what it does, and let the developer write it into the project. Do not generate a full file, full module, or full application unless explicitly requested.
- Follow a teaching loop: explain the goal and relevant files/concepts, provide one small snippet, let the developer implement it, then verify it before continuing.
- Before implementing a phase, state the goal, the files/concepts involved, the developer's implementation task, and the verification checkpoint. Work incrementally instead of generating the whole application at once.
- **Setiap kali developer menanyakan konsep atau error, tambahkan penjelasan ke `learning-notes.md`** — file ini adalah knowledge base pembelajaran yang terus bertambah. Format: judul, perbandingan dengan ekosistem lain, contoh code, dan penjelasan kenapa.

## Learning plan

Follow this order so the developer learns the platform while building the product:

1. **C# and hosting:** learn records/classes, nullable references, enums, pattern matching, interfaces, dependency injection, `Program.cs`, configuration, middleware, and Minimal APIs. Compare with TypeScript types/classes, NestJS modules/providers, and Next.js route handlers.
2. **Persistence:** learn EF Core entities, `DbContext`, LINQ, change tracking, relationships, and migrations. Compare with Prisma/Drizzle schema, client queries, and migrations.
3. **API design:** learn DTOs, model binding, validation, status codes, OpenAPI, and endpoint organization. Compare with NestJS DTOs/pipes/controllers and Next.js API route handlers.
4. **Domain behavior:** implement ticket priority and allowed status transitions as explicit C# domain logic, then test it. Compare with TypeScript service/domain functions, while preferring a strong typed state model in C#.
5. **Authentication and authorization:** learn JWT, claims, policies, and resource authorization. Compare with NextAuth/Auth.js or NestJS guards, but keep authorization enforced by the API.
6. **Async processing:** learn `IHostedService`, cancellation tokens, scoped services, and structured logging for SLA checks. Compare with Node.js workers and scheduled jobs.
7. **Production practices:** learn integration testing, optimistic concurrency, audit logging, Docker, health checks, and observability. Verify each feature through a small end-to-end slice before moving on.

## Commands

```bash
dotnet restore
dotnet build
dotnet run
dotnet test
```

- `dotnet build` currently succeeds with zero warnings and errors.
- `dotnet test` has no test project to run until one is added.
- Development URLs from `Properties/launchSettings.json`: `http://localhost:5111` and `https://localhost:7133`.
- OpenAPI is mapped only when `ASPNETCORE_ENVIRONMENT=Development`.

## Verified references

- `Context.md`: business problem, current manual workflow, expected improvement, and business rules.
- `Goal.md`: target architecture, domain entities, MVP scope, workflow, priority matrix, SLA, escalation, and implementation direction.
- `plan.md`: step-by-step learning and implementation plan; follow it before inventing a new phase order.
- `Program.cs`: current application entrypoint and scaffold behavior.
- `Tickset.csproj`: framework, language settings, and package references.

## Architecture and domain target

Use a modular monolith with these planned modules:

```text
Identity | Ticketing | Assignment | ServiceLevel | Approvals
KnowledgeBase | Notifications | Reporting | Auditing
```

### Folder structure (layered)

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
- Alur dependency: `Endpoints -> Services -> Data`; `DTOs` menjadi kontrak request/response dan `Models` menjadi entity persistence.
- `Services` tidak boleh mengembalikan entity database langsung ke client; map entity ke DTO di boundary API.
- `Models` tidak boleh bergantung pada `Services`, `Data`, atau `Endpoints`.
- Daftarkan service di `Program.cs` melalui DI; jangan membuat service atau `DbContext` dengan `new` di endpoint.

**Perbandingan dengan NestJS:**

| .NET Layer | NestJS |
|------------|--------|
| `Models/` | `entities/` |
| `Data/` | `prisma/` atau `database/` |
| `Services/` | `services/` |
| `DTOs/` | `dto/` |
| `Endpoints/` | `controllers/` |
| `Middlewares/` | `middleware/` |
| `Enums/` | `enums/` |

Important planned entities include `User`, `Role`, `Permission`, `Team`, `Ticket`, `TicketType`, `TicketCategory`, `TicketAssignment`, `TicketStatusHistory`, `TicketComment`, `SlaPolicy`, `SlaInstance`, `BusinessCalendar`, `EscalationRule`, `EscalationEvent`, `Notification`, and `AuditLog`. See `Goal.md` section 19 before designing the model.

## Implementation order

Implement one working slice at a time and stop at each checkpoint.

1. **Foundation:** use PostgreSQL; add the EF Core PostgreSQL provider and design package; create `ApplicationDbContext` and `BaseEntity`; configure a connection; create and apply the first migration.
2. **Identity:** add users, roles, permissions, teams, JWT registration/login/refresh, and authorization. Checkpoint: register and login return a token.
3. **Ticketing core:** add ticket types/categories, create/list/detail endpoints, ticket numbering, impact/urgency priority calculation, and allowed status transitions. Checkpoint: creating a ticket returns its number and calculated priority.
4. **Assignment:** route category to team, allow supervisor assignment and team-member self-claim, and protect claim with optimistic concurrency. Checkpoint: a competing claim receives a conflict.
5. **SLA and escalation:** add policy, business calendar, SLA instance, pause/resume for `PendingRequester`, and an `IHostedService` checker. Checkpoint: escalation events fire once at 75%, 90%, and 100%.
6. **Communication:** add public comments, internal notes, history, and secure attachments.
7. **Notifications and audit:** record important status, assignment, priority, and SLA changes; add in-app notifications.
8. **Reporting and delivery:** add agent/supervisor dashboards, unit and integration tests, then Docker Compose for API plus database.

## Start here

1. Read `Goal.md` sections 19-20 and the relevant business rules in `Context.md`.
2. PostgreSQL is selected; add packages separately, for example `Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.Design`, and `Npgsql.EntityFrameworkCore.PostgreSQL`; do not assume they are installed yet.
3. Install or verify the `dotnet-ef` CLI tool, then create the first `DbContext`, migration, and database update.
4. Verify each small change with `dotnet build`; add tests when the first domain behavior is implemented.

The first implementation task is Phase 0: database foundation. Do not begin Identity or Ticketing until the `DbContext`, connection configuration, first migration, and database update have been understood and verified.

## Non-negotiable domain rules

- Business hours are Monday-Friday, 08:00-17:00, Asia/Jakarta.
- Ticket numbers use `INC-YYYY-NNNNNN` for incidents.
- Priority is calculated from impact and urgency; see `Goal.md` section 11.
- Valid primary workflow is `New -> Open -> InProgress -> Resolved -> Closed`, with pending, reopened, and cancelled transitions defined in `Goal.md`.
- Only allowed state transitions may execute; status changes require history.
- Requesters cannot resolve tickets; only resolved tickets can be closed.
- Resolution SLA pauses at `PendingRequester` and resumes when the requester replies.
- Escalation thresholds are 75% to agent, 90% to agent and lead, and 100% to manager; each level fires once.
- Requesters see public comments only; internal notes are for agents/supervisors.

## Conventions

- Follow the existing concise .NET style in `Program.cs`; use PascalCase for types/methods and camelCase for locals/parameters.
- Keep the modular boundaries clear as modules are introduced.
- Do not treat the planned architecture or entities as implemented until the code and configuration exist.
