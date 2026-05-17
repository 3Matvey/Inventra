# Inventra Project State

Last updated: 2026-05-17

This is the handoff map for continuing work after context loss. The codebase is still the source of truth, but this file should stay close enough that a new chat can resume without guessing.

## Product Goal

Inventra is an ASP.NET Core/C# inventory management web app.

Core requirements:

- Inventories and items are table-first.
- No repeated view/edit/delete buttons in every table row.
- Anonymous users can view inventories/items and search in read-only mode.
- Authenticated users can create inventories, comment, like, and add/edit items when allowed.
- Admins can manage users and act as owner of every inventory.
- Users authenticate via at least two social providers.
- Full-text search is available from every page header.
- Inventories support custom item fields.
- Inventories support custom inventory number formats.
- Optimistic locking is required for inventories and items.
- Inventory discussion posts update automatically within 2-5 seconds.
- UI supports two languages and two themes.

## Solution Structure

Projects:

- `Inventra.Domain`
- `Inventra.Application`
- `Inventra.Infrastructure.Data`
- `Inventra.Infrastructure.Identity`
- `Inventra.Api`

Solution:

- `Inventra.slnx`

Database:

- PostgreSQL 18.3 in local Docker Compose.
- EF Core 10 + Npgsql.
- `compose.yaml` uses root `.env`.
- Postgres volume path is `/var/lib/postgresql`; do not change it without user approval.

## Working Rules

Important user preferences:

- Do not make unrequested infrastructure changes.
- Do not change Docker Compose, migrations, secrets, or deployment config without explicit permission.
- Use `apply_patch` for manual edits.
- Public API objects should be one public type per file.
- Add XML summaries for externally consumed interfaces/DTOs where useful.
- Keep methods short where practical, about 5-10 lines.
- Query code may be longer, but prefer named helpers and readable sources.
- Interfaces are for ports/boundaries, not every class.
- Use `IUseCase` marker for use case registration.
- Use new C# extension block syntax where extension methods are requested.

## Domain Layer

Base types:

- `Entity`
  - `Id` generated with `Guid.CreateVersion7()`.
  - EF config uses `ValueGeneratedNever()`.
- `AuditableEntity`
  - `CreatedAt` and `UpdatedAt` are set by `AuditInterceptor`.

Important entities:

- `UserAccount`
  - Domain user profile.
  - `UserName`, `Email`, `IsBlocked`, `IsAdmin`.
- `Inventory`
  - Aggregate root for settings, fields, access grants, custom ID format, tags, comments.
  - Has `Version`.
- `InventoryField`
  - Custom field definition.
  - Types: single-line text, multi-line text, number, link, boolean.
- `InventoryIdFormatElement`
  - Custom ID element.
  - Supports fixed text, random numbers, GUID, date/time, sequence.
- `InventoryItem`
  - Has internal `Id`, user-facing `CustomId`, optional `SequenceNumber`, `Version`, values, likes.
- `InventoryComment`
  - Now inherits `AuditableEntity`; no manual `createdAt` is passed.
- `ItemLike`
  - One like per item per user.

## Application Layer

No MediatR. Use cases are concrete classes with `ExecuteAsync`.

Use case registration:

- `IUseCase` marker.
- `AddUseCases()` scans concrete classes assignable to `IUseCase`.

Result pattern:

- `Result`
- `Result<T>`
- `Error`
- `ErrorType`
- `ResultExtensions.Match(...)`

Important ports:

- `ICurrentUser`
- `IUnitOfWork`
- `IInventoryRepository`
- `IInventoryItemRepository`
- `ICategoryRepository`
- `ITagRepository`
- `IUserRepository`
- `IIdentityAccountService`
- `IExternalIdentityService`
- `IAuthenticationSession`
- `ICustomIdGenerator`
- `IInventorySequenceProvider`

Application helpers:

- `InventoryPermissions`
  - Static helper for owner/admin/write/comment/like decisions.
- `InventoryRepositoryExtensions`
  - New extension block over `IInventoryRepository`.
  - `LoadWithManageAccessAsync(...)`
  - `LoadWithManageAccessAndVersionAsync(...)`
- `ItemConcurrency`
  - Checks expected item version for update/delete.
- `InventoryCustomIdComposer`
- `InventoryCustomIdGenerator`

Implemented inventory use cases:

- create/update settings
- public write access
- grant/revoke explicit access
- add/update/remove/reorder fields
- add/update/remove/reorder custom ID elements
- preview custom ID
- add comment

Implemented item use cases:

- create/update/delete item
- like/unlike item

Implemented identity/admin use cases:

- complete external login
- get current user profile
- list users
- block/unblock user
- add/remove admin role
- delete user

Read/query contracts:

- `ICategoryQueries`
- `IInventoryQueries`
- `IInventoryItemQueries`
- `IInventoryCommentQueries`
- `IUserQueries`

Query DTO namespaces:

- `Inventra.Application.Common.Queries.Dto`
- `Inventra.Application.Inventories.Queries.Dto`
- `Inventra.Application.Inventories.Comments.Dto`
- `Inventra.Application.Items.Queries.Dto`

## Infrastructure.Data

Main types:

- `AppDbContext`
- `AuditInterceptor`
- command repositories in `Repositories`
- read queries in `Queries`

DI:

- `AddDataServices(configuration)`
- Registers database, repositories, read queries, unit of work.

Read query style:

- Query sources use named `AsNoTracking()` variables before query expressions.
- Avoid inline `dbContext.Table.AsNoTracking()` inside `from`/`join`.

Read query classes:

- `CategoryQueries`
- `UserQueries`
- `InventoryCommentQueries`
- `InventoryQueries` partials:
  - `InventoryQueries.cs`
  - `InventoryQueries.Lists.cs`
  - `InventoryQueries.Details.cs`
  - `InventoryQueries.Tags.cs`
  - `InventoryQueries.Models.cs`
- `InventoryItemQueries` partials:
  - `InventoryItemQueries.cs`
  - `InventoryItemQueries.Table.cs`
  - `InventoryItemQueries.Details.cs`
  - `InventoryItemQueries.Statistics.cs`
  - `InventoryItemQueries.Models.cs`

Important constraints/indexes:

- unique item custom ID within inventory: `(inventory_id, custom_id)`
- unique item sequence within inventory where sequence is not null
- unique item like: `(item_id, user_id)`
- unique inventory access grant: `(inventory_id, user_id)`
- unique inventory tag: `(inventory_id, tag_id)`
- unique tag name
- unique category name
- unique domain user name/email

Optimistic locking:

- `Inventory.Version` is configured with `.IsConcurrencyToken()`.
- `InventoryItem.Version` is configured with `.IsConcurrencyToken()`.
- Application-level `expectedVersion` is also checked before mutations.
- `DbUpdateConcurrencyException` is mapped to `409 Conflict` by API middleware.

Full-text search:

- PostgreSQL FTS is used instead of `ILike` for inventory search.
- Uses `simple` configuration.
- Migration `20260513200138_AddInventoryFullTextSearch` adds GIN expression index.

Migrations currently present:

- Data:
  - `20260512180748_InitialCreate`
  - `20260513200138_AddInventoryFullTextSearch`
- Identity:
  - `20260512181119_InitialIdentity`

## Infrastructure.Identity

Uses ASP.NET Core Identity with cookies.

Main types:

- `ApplicationUser : IdentityUser<Guid>`
  - Constructor assigns `Id = Guid.CreateVersion7()`.
- `ApplicationIdentityDbContext`
- `CurrentUser`
- `ExternalIdentityService`
- `AuthenticationSession`
- `IdentityAccountService`
- `IdentityRoles`

Auth providers:

- Google.
- GitHub.
- Facebook is not currently wired.

External login flow:

1. `GET /auth/external/{provider}`
2. API validates provider scheme.
3. API creates `AuthenticationProperties` with `Items["LoginProvider"] = provider`.
4. Provider returns to middleware callback:
   - Google: `/signin-google`
   - GitHub: `/signin-github`
5. Middleware writes `Identity.External` cookie.
6. Middleware redirects to `/auth/external-login/callback`.
7. `ExternalIdentityService` reads `SignInManager.GetExternalLoginInfoAsync()`.
8. User is found by external login, or linked by email, or created.
9. Domain `UserAccount` is created/updated with same `Guid`.

Duplicate provider/email behavior:

- Google first, then GitHub with the same email links GitHub login to the same `ApplicationUser`.
- Expected DB state: one `AspNetUsers` row, two `AspNetUserLogins` rows.
- Old fallback GitHub users may need manual cleanup in dev DB if created before the fix.

Session policy:

- JWT is not used.
- Browser/Insomnia use HttpOnly Identity application cookie.
- Frontend calls `/auth/me` for current user state.
- Redis/session invalidation is deferred.

## API Layer

Program:

- controllers
- SignalR
- Application/Data/Identity services
- OpenAPI + Scalar in development
- `ExceptionHandlingMiddleware`
- HTTPS redirection
- authentication/authorization
- controllers
- SignalR hub mapping

Exception middleware:

- `ExceptionHandlingMiddleware`
- `ExceptionMapper`

Mappings:

- `DbUpdateConcurrencyException` -> `409 Persistence.ConcurrencyConflict`
- PostgreSQL unique violation -> `409 Persistence.UniqueViolation`
- PostgreSQL foreign key violation -> `409 Persistence.ForeignKeyViolation`
- `DomainException` -> `400 Domain.ValidationFailed`
- `KeyNotFoundException` -> `404 Common.NotFound`
- `ArgumentException` -> `400 Common.BadRequest`
- unknown -> `500 Common.UnhandledException`

Auth endpoints:

- `GET /auth/external/{provider}`
- `GET /auth/external-login/callback`
- `POST /auth/logout`
- `GET /auth/me`

Admin endpoints:

- `GET /admin/users`
- `POST /admin/users/{userId}/block`
- `POST /admin/users/{userId}/unblock`
- `POST /admin/users/{userId}/admin-role`
- `DELETE /admin/users/{userId}/admin-role`
- `DELETE /admin/users/{userId}`

Read endpoints:

- `GET /categories`
- `GET /users/autocomplete`
- `GET /inventories/latest`
- `GET /inventories/top`
- `GET /inventories/search`
- `GET /inventories/{inventoryId}`
- `GET /inventories/owned/{ownerId}`
- `GET /inventories/writable/{userId}`
- `GET /inventories/{inventoryId}/statistics`
- `GET /inventories/{inventoryId}/items`
- `GET /items/{itemId}`
- `GET /tags/cloud`
- `GET /tags/autocomplete`

Inventory write endpoints:

- `POST /inventories`
- `PUT /inventories/{inventoryId}/settings`
- `PUT /inventories/{inventoryId}/public-access`
- `POST /inventories/{inventoryId}/access-grants`
- `DELETE /inventories/{inventoryId}/access-grants/{userId}?expectedVersion={version}`

Field endpoints:

- `POST /inventories/{inventoryId}/fields`
- `PUT /inventories/{inventoryId}/fields/{fieldId}`
- `DELETE /inventories/{inventoryId}/fields/{fieldId}?expectedVersion={version}`
- `PUT /inventories/{inventoryId}/fields/order`

Custom ID endpoints:

- `POST /inventories/{inventoryId}/id-format/elements`
- `PUT /inventories/{inventoryId}/id-format/elements/{elementId}`
- `DELETE /inventories/{inventoryId}/id-format/elements/{elementId}?expectedVersion={version}`
- `PUT /inventories/{inventoryId}/id-format/elements/order`
- `GET /inventories/{inventoryId}/id-format/preview`

Item endpoints:

- `POST /inventories/{inventoryId}/items`
- `PUT /items/{itemId}`
- `DELETE /items/{itemId}?expectedVersion={version}`
- `POST /items/{itemId}/like`
- `DELETE /items/{itemId}/like`

Comments/SignalR:

- `GET /inventories/{inventoryId}/comments`
- `POST /inventories/{inventoryId}/comments`
- SignalR hub: `/hubs/inventory-discussion`
- Hub methods:
  - `JoinInventoryDiscussion(Guid inventoryId)`
  - `LeaveInventoryDiscussion(Guid inventoryId)`
- Event sent after successful POST:
  - `commentAdded`

Mutation requests that edit existing inventory/item content require `expectedVersion`.

## Local Development

Docker:

- `compose.yaml`
- service `postgres`
- image `postgres:18.3`
- env from root `.env`
- volume path `/var/lib/postgresql`

Expected root `.env`:

```env
POSTGRES_DB=inventra
POSTGRES_USER=inventra
POSTGRES_PASSWORD=...
POSTGRES_PORT=5432
```

Secrets:

- `builder.Configuration.AddPortableSecrets()` is used in `Program.cs`.
- Auth provider secrets may come from portable secrets/env.
- Do not commit real provider secrets.

## Current Backend Status

Backend core is close to MVP complete.

Still worth doing:

1. Manual smoke test major flows on live DB.
2. Seed or manually insert demo categories:
   - Equipment
   - Furniture
   - Book
   - Other
3. Decide first-admin bootstrap strategy.
4. Decide language/theme persistence:
   - localStorage only, or
   - server-side user preferences.
5. Decide image upload strategy:
   - current backend supports `ImageUrl`,
   - real cloud upload integration is not implemented.
6. Consider global search scope beyond inventories if time allows.

Deferred:

- Redis session invalidation/cache.
- Form auth with email confirmation.
- Cloud uploader integration.
- Document previews.
- CSV/Excel export.
- Optional arbitrary unlimited fields.
- Verified GitHub email hardening.

## Build Notes

Useful commands:

```powershell
$env:DOTNET_CLI_HOME='M:\ItransitionTasks\Inventra\.dotnet'
dotnet build Inventra.Application\Inventra.Application.csproj
dotnet build Inventra.Infrastructure.Data\Inventra.Infrastructure.Data.csproj
dotnet build Inventra.Infrastructure.Identity\Inventra.Infrastructure.Identity.csproj
dotnet build Inventra.Api\Inventra.Api.csproj
```

Known local issue:

- If `Inventra.Api` is running in Visual Studio, build can fail because DLLs are locked.
- Stop the API before rebuilding the API project.
- Sandbox may block user-level NuGet config. If build fails with NuGet access denied, rerun with elevated permission.
