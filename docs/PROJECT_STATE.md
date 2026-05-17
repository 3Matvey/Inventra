# Inventra Project State

Last updated: 2026-05-16

This document is the handoff map for continuing work after context loss. Treat the codebase as the source of truth, but keep this file in sync when architecture or implemented scope changes.

## Product Goal

Inventra is an ASP.NET Core/C# web application for inventory management.

Core assignment requirements:

- Inventories and items are displayed as table views by default.
- Do not put repeated view/edit/delete buttons in every table row.
- Anonymous users can view inventories/items and use search in read-only mode.
- Authenticated users can create inventories, comment, like, and add/edit items when allowed.
- Admins can manage users and act as owner for every inventory.
- Users authenticate via at least two social providers.
- Every page should expose full-text search from the top header.
- Inventories have custom item fields.
- Inventories have custom inventory number formats.
- Optimistic locking is required for inventories and items.
- Discussion posts need near-real-time updates within 2-5 seconds.
- UI must support two languages and two themes.

Current implementation focus:

- Backend first.
- Frontend, cloud image upload UX, and visual polish are still future work.

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

- PostgreSQL 18.3 for local development.
- EF Core 10/Npgsql.
- `compose.yaml` uses `.env` variables and volume path `/var/lib/postgresql`.

## Team Rules

Important working preferences:

- Do not make unrequested infrastructure changes.
- Do not change Docker Compose, migrations, secrets, or deployment-related configuration without explicit permission.
- Use `apply_patch` for manual file edits.
- Keep public API objects one public type per file.
- Add XML summaries for externally consumed interfaces/DTOs when useful.
- Use the project's existing style and C# extension block DI pattern.
- Keep methods short where practical, roughly 5-10 lines, but query code may naturally be longer.
- Interfaces are for ports/boundaries. Avoid interfaces for classes with no meaningful alternate implementation.
- Use `IUseCase` as the marker for use case registration.

## Domain Layer

Important base types:

- `Entity`
  - `Id` is generated in code with `Guid.CreateVersion7()`.
  - EF uses `ValueGeneratedNever()`.
- `AuditableEntity`
  - `CreatedAt` and `UpdatedAt` are handled by `AuditInterceptor`.

Important entities:

- `UserAccount`
  - Domain-facing user profile.
  - Stores `UserName`, `Email`, `IsBlocked`, `IsAdmin`.
- `Inventory`
  - Aggregate root for settings, fields, access, custom ID format, tags, comments.
  - Has `Version` for optimistic locking.
- `InventoryField`
  - Custom field definition.
  - Types: single-line text, multi-line text, number, link, boolean.
  - Has `ShowInTable` and `Order`.
- `InventoryIdFormatElement`
  - Custom ID format element.
  - Supports fixed text, random numbers, GUID, date/time, sequence.
- `InventoryItem`
  - Has internal `Id`, user-facing `CustomId`, optional `SequenceNumber`, `Version`, custom field values, likes.
- `ItemFieldValue`
  - Stores typed nullable columns: text, number, boolean.
- `ItemLike`
  - One like per item per user.
- `InventoryAccessGrant`
  - Explicit write access for a user.
- `InventoryComment`
  - Linear discussion post.

Helpers:

- `FieldValue`
- `Guard`

## Application Layer

No MediatR. Use cases are concrete classes with `ExecuteAsync`.

Use case registration:

- `IUseCase` is a marker interface.
- `AddUseCases()` scans the Application assembly for concrete classes assignable to `IUseCase`.

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

Important helpers/services:

- `InventoryPermissions`
  - Static helper, replaced the old `IInventoryPermissionService`.
  - Used by use cases for owner/admin/write-access checks.
- `InventoryAccessLoader`
- `InventoryCustomIdComposer`
- `InventoryCustomIdGenerator`

Implemented inventory write use cases:

- `CreateInventoryUseCase`
- `UpdateInventorySettingsUseCase`
- `SetPublicWriteAccessUseCase`
- `GrantInventoryAccessUseCase`
- `RevokeInventoryAccessUseCase`
- `AddInventoryFieldUseCase`
- `UpdateInventoryFieldUseCase`
- `RemoveInventoryFieldUseCase`
- `ReorderInventoryFieldsUseCase`
- `AddInventoryIdFormatElementUseCase`
- `UpdateInventoryIdFormatElementUseCase`
- `RemoveInventoryIdFormatElementUseCase`
- `ReorderInventoryIdFormatElementsUseCase`
- `PreviewInventoryCustomIdUseCase`

Implemented item write use cases:

- `CreateInventoryItemUseCase`
- `UpdateInventoryItemUseCase`
- `DeleteInventoryItemUseCase`
- `LikeInventoryItemUseCase`
- `UnlikeInventoryItemUseCase`

Implemented identity/admin use cases:

- `CompleteExternalLoginUseCase`
- `GetCurrentUserProfileUseCase`
- `GetUsersPageUseCase`
- `ChangeUserAdminRoleUseCase`
- `ChangeUserBlockStatusUseCase`
- `DeleteUserUseCase`

Read/query contracts:

- `IInventoryQueries`
- `IInventoryItemQueries`

Common query DTOs:

- `PageRequest`
- `PagedResult<T>`
- `AutocompleteOptionDto`

Inventory query DTO namespace:

- `Inventra.Application.Inventories.Queries.Dto`

Item query DTO namespace:

- `Inventra.Application.Items.Queries.Dto`

## Infrastructure.Data

Uses EF Core + PostgreSQL + snake_case naming.

Main types:

- `AppDbContext`
- `AuditInterceptor`
- command repositories in `Repositories`
- read models/queries in `Queries`

DI registration:

- `AddDataServices(configuration)`
- Registers database, repositories, query services, unit of work.

Command repositories return tracked aggregates.

Read queries are `AsNoTracking()`-oriented and live in partial classes:

- `InventoryQueries.cs`
- `InventoryQueries.Lists.cs`
- `InventoryQueries.Details.cs`
- `InventoryQueries.Tags.cs`
- `InventoryQueries.Models.cs`
- `InventoryItemQueries.cs`
- `InventoryItemQueries.Table.cs`
- `InventoryItemQueries.Details.cs`
- `InventoryItemQueries.Statistics.cs`
- `InventoryItemQueries.Models.cs`

Important indexes/constraints:

- unique item custom ID within inventory: `(inventory_id, custom_id)`
- unique item sequence within inventory when sequence exists
- unique item like: `(item_id, user_id)`
- unique inventory access grant: `(inventory_id, user_id)`
- unique inventory tag: `(inventory_id, tag_id)`
- unique tag name
- unique category name
- unique domain user name/email

Sequence generation:

- `InventorySequenceProvider` uses PostgreSQL atomic upsert.
- First issued sequence is `1`.
- Gaps are acceptable if item saving fails after sequence reservation.

Full-text search:

- PostgreSQL FTS is used instead of `ILIKE`.
- Inventory FTS uses `simple` configuration.
- Migration `20260513200138_AddInventoryFullTextSearch` adds the GIN expression index.
- Search ranks by `ts_rank` and falls back to recent updates/creation.

Migrations currently present:

- Data:
  - `20260512180748_InitialCreate`
  - `20260513200138_AddInventoryFullTextSearch`
- Identity:
  - `20260512181119_InitialIdentity`

## Infrastructure.Identity

Uses ASP.NET Core Identity with cookie auth.

Main types:

- `ApplicationUser : IdentityUser<Guid>`
  - Currently empty but intentionally keeps a project-owned Identity user type.
- `ApplicationIdentityDbContext`
- `CurrentUser`
- `ExternalIdentityService`
- `AuthenticationSession`
- `IdentityAccountService`
- `IdentityRoles`

Identity and domain users:

- `ApplicationUser` is the Identity/auth user.
- `UserAccount` is the domain/application user.
- Both share the same `Guid`.
- `CompleteExternalLoginUseCase` creates/updates `UserAccount` after external Identity login.

Providers:

- Google is configured when `Authentication:Google:ClientId` and `Authentication:Google:ClientSecret` exist.
- GitHub is configured when `Authentication:GitHub:ClientId` and `Authentication:GitHub:ClientSecret` exist.
- Facebook is not currently wired.

External login flow:

1. `GET /auth/external/{provider}`
2. API validates provider scheme.
3. API returns `Challenge(...)`.
4. Provider returns to middleware callback:
   - Google: `/signin-google`
   - GitHub: `/signin-github`
5. Middleware writes the temporary `Identity.External` cookie.
6. Middleware redirects to app callback:
   - `/auth/external-login/callback`
7. `CompleteExternalLoginUseCase` calls `ExternalIdentityService.CompleteSignInAsync()`.
8. `SignInManager.GetExternalLoginInfoAsync()` reads the external cookie.
9. Existing/new `ApplicationUser` is signed in with the application cookie.
10. Matching `UserAccount` is created/updated.

Important fix already applied:

- `AuthenticationProperties.Items["LoginProvider"] = provider` is set in `AuthController`.
- Without this, the external cookie was valid, but `GetExternalLoginInfoAsync()` returned `null`.

Known current Identity issue:

- Duplicate email across providers is not handled yet.
- Current behavior:
  - Google creates user with real email.
  - GitHub with the same email fails with `DuplicateEmail`.
- Intended next fix:
  - `FindByLogin(provider, providerKey)`.
  - If missing, read email.
  - `FindByEmail(email)`.
  - If found, `AddLoginAsync(existingUser, loginInfo)` and sign in.
  - If not found, create a new user and link login.
- GitHub can provide fallback emails like `{ProviderKey}@GitHub.external` if email claim is missing.
- GitHub email retrieval has been improved enough that the debugger showed a real `ClaimTypes.Email`, but verify the actual provider configuration before relying on it.

Cookie/session policy:

- JWT is not used.
- Browser/API clients authenticate via ASP.NET Identity application cookie.
- Cookie is HttpOnly and sent automatically by the browser/Insomnia for the same host.
- Frontend should call `/auth/me` to learn current user state.
- Redis-based session invalidation was discussed but intentionally deferred.

## API Layer

Program:

- Controllers.
- Application services.
- Data services.
- Identity services.
- OpenAPI and Scalar in development.
- `UseAuthentication()`.
- `UseAuthorization()`.

Base controller:

- `ApiControllerBase`
- Uses `ResultExtensions.Match(...)` / `FromResult(...)` to map application results.

Current auth endpoints:

- `GET /auth/external/{provider}`
- `GET /auth/external-login/callback`
- `POST /auth/logout`
- `GET /auth/me`

Current admin endpoints:

- `GET /admin/users`
- `POST /admin/users/{userId}/block`
- `POST /admin/users/{userId}/unblock`
- `POST /admin/users/{userId}/admin-role`
- `DELETE /admin/users/{userId}/admin-role`
- `DELETE /admin/users/{userId}`

Current read endpoints:

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

Still missing API endpoints:

- Inventory write endpoints.
- Inventory field write endpoints.
- Inventory custom ID format write endpoints.
- Access management write endpoints.
- Item create/update/delete endpoints.
- Like/unlike endpoints.
- Discussion comments endpoints.
- SignalR hub endpoint.
- User preferences endpoints if theme/language are stored server-side.

## Local Development

Docker Compose:

- File: `compose.yaml`
- Service: `postgres`
- Image: `postgres:18.3`
- Env values come from root `.env`.
- Volume path must stay `/var/lib/postgresql` for this Postgres image/version.

Expected root `.env` shape:

```env
POSTGRES_DB=inventra
POSTGRES_USER=inventra
POSTGRES_PASSWORD=...
POSTGRES_PORT=5432
```

Development connection string currently exists in:

- `Inventra.Api/appsettings.Development.json`

Secrets:

- `builder.Configuration.AddPortableSecrets()` is used in `Program.cs`.
- Auth provider secrets may come from portable secrets/env.
- Do not commit real provider secrets.

## Current Backend Backlog

Highest priority:

1. Fix external provider linking by email to avoid `DuplicateEmail`.
2. Add write API controllers for existing use cases.
3. Add comments backend:
   - list comments
   - add comment
   - SignalR or near-real-time polling/hub updates
4. Verify optimistic locking behavior and HTTP conflict mapping.
5. Verify item custom ID duplicate conflicts map cleanly.
6. Verify like uniqueness and error mapping.

Secondary:

- Add endpoint/list for available external auth providers.
- Decide whether theme/language are server-side user preferences or client-only.
- Improve GitHub verified email handling later if needed.
- Add richer logging only where it helps production diagnostics.
- Review read queries with real data.

Deferred:

- Redis session invalidation/cache.
- Cloud image uploader integration.
- Document previews.
- CSV/Excel export.
- Form auth with email confirmation.
- Optional arbitrary unlimited fields.

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

- `Inventra.Api` running under Visual Studio can lock DLLs and make `dotnet build Inventra.Api\Inventra.Api.csproj` fail at copy time.
- Stop the running API before rebuilding the API project.
- Sandbox may block reading user-level NuGet config; rerun build with elevated permission when needed.
