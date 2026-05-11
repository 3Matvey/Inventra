# Inventra Project State

Last updated: 2026-05-11

This file is a handoff note for continuing work in a new chat/context. It captures the current architecture, decisions, implemented pieces, style rules, and next steps.

## Goal

Inventra is an ASP.NET/C# inventory management web application.

Core product concepts:

- Users create arbitrary inventories.
- Each inventory defines settings, access rules, custom item fields, and a custom item ID format.
- Other users with write access create/edit/delete items using that inventory template.
- Everyone can view inventories/items.
- Authenticated users can create inventories, comment, like, and add items when allowed.
- Admins can manage users and act as owner for every inventory.

Important assignment requirements:

- Items and inventories must be shown as tables by default.
- No repeated row buttons like `[Edit][Delete]` in every row.
- Full-text search available from top header.
- Custom inventory numbers are required.
- Custom fields are required.
- Optimistic locking is required for inventories and items.
- PostgreSQL is likely the DB.
- CSS/front-end will be done near the end.

## Current Solution Structure

Projects:

- `Inventra.Domain`
- `Inventra.Application`
- `Inventra.Infrastructure.Data`
- `Inventra.Api`

Solution file:

- `Inventra.slnx`

## Coding Style Rules

Project-specific style preferences from the user:

- Methods should generally be short, around 5-10 lines when possible.
- Do not add interfaces for use cases when there is only one implementation.
- Interfaces are for boundaries/ports, not for every class.
- Avoid unnecessary `sealed`, especially for obvious classes like EF configurations.
- Use helper classes/methods to remove repeated guard checks.
- Do not silently overwrite user edits. Read files before changing them.
- Prefer readable `using` statements over fully qualified names like `Domain.Entities.Inventory`.
- `DependencyInjection` extension files should use the new C# extension block style:

```csharp
public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDataServices(IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.AddRepositories();
            services.AddUnitOfWork();

            return services;
        }

        private IServiceCollection AddDatabase(IConfiguration configuration)
        {
            ...
        }
    }
}
```

## Domain Layer

Important base classes:

- `Entity`
  - `Id` is generated in code using `Guid.CreateVersion7()`.
  - EF must use `ValueGeneratedNever()`.

- `AuditableEntity`
  - Has `CreatedAt` and `UpdatedAt`.
  - These are now set by EF Core `AuditInterceptor`.
  - Domain methods should not call `Touch`.
  - Domain constructors should not take audit-only `createdAt`.

Important entities:

- `UserAccount`
  - `UserName`
  - `Email`
  - `IsBlocked`
  - `IsAdmin`

- `Category`
  - Database-driven category table.
  - No enum category code.

- `Tag`

- `Inventory`
  - Aggregate root for inventory settings.
  - Owns fields/access grants/id format elements/tags/comments.
  - Has `Version` for optimistic locking.
  - Important methods:
    - `UpdateSettings`
    - `SetPublicWriteAccess`
    - `GrantAccess`
    - `RevokeAccess`
    - `AddField`
    - `UpdateField`
    - `RemoveField`
    - `ReorderFields`
    - `AddIdFormatElement`
    - `UpdateIdFormatElement`
    - `RemoveIdFormatElement`
    - `ReorderIdFormatElements`
    - `ReplaceTags`
    - `AddComment`

- `InventoryField`
  - Custom field definition.
  - Types: single-line text, multi-line text, number, link, boolean.
  - Has `ShowInTable` and `Order`.

- `InventoryIdFormatElement`
  - Custom item ID format element.
  - Types: fixed text, random 20-bit, random 32-bit, random 6-digit, random 9-digit, GUID, date/time, sequence.

- `InventoryItem`
  - Item inside inventory.
  - Has internal `Id`.
  - Has user-facing `CustomId`.
  - Has nullable `SequenceNumber`.
  - Has `Version`.
  - Owns field values and likes.

- `ItemFieldValue`
  - Stores custom field value using typed nullable columns:
    - `TextValue`
    - `NumberValue`
    - `BooleanValue`

- `ItemLike`
  - One like per item per user.

- `InventoryAccessGrant`
  - Explicit write access for a user.

- `InventoryComment`
  - Linear discussion post.
  - `CreatedAt` is business data, not only audit.

Value objects/helpers:

- `FieldValue`
  - Used by Application/domain methods to map request values into typed field values safely.
- `Guard`
  - Uses `CallerArgumentExpression`.
  - Has helpers such as `RequiredId`, `Required`, `Optional`, `NonNegative`, `RequiredIds`, `RequiredCompleteIdSet`.

## Application Layer

No MediatR.

Use cases are concrete classes with `ExecuteAsync`.

Result pattern is in:

- `Inventra.Application/Common/Results`

Types:

- `Result`
- `Result<T>`
- `Error`
- `ErrorType`
- `ResultExtensions`

Ports/interfaces:

- `ICurrentUser`
- `IUnitOfWork`
- `IInventoryRepository`
- `IInventoryItemRepository`
- `ICategoryRepository`
- `ITagRepository`
- `IUserRepository`
- `IInventoryPermissionService`
- `ICustomIdGenerator`
- `IInventorySequenceProvider`

`IDateTimeProvider` was removed/replaced by built-in `TimeProvider`.

Implemented write use cases:

Inventories:

- `CreateInventoryUseCase`
- `UpdateInventorySettingsUseCase`

Fields:

- `AddInventoryFieldUseCase`
- `UpdateInventoryFieldUseCase`
- `RemoveInventoryFieldUseCase`
- `ReorderInventoryFieldsUseCase`

Access:

- `SetPublicWriteAccessUseCase`
- `GrantInventoryAccessUseCase`
- `RevokeInventoryAccessUseCase`

Custom ID format:

- `AddInventoryIdFormatElementUseCase`
- `UpdateInventoryIdFormatElementUseCase`
- `RemoveInventoryIdFormatElementUseCase`
- `ReorderInventoryIdFormatElementsUseCase`
- `PreviewInventoryCustomIdUseCase`

Custom ID generation:

- `InventoryCustomIdComposer`
- `InventoryCustomIdGenerator`

Important current design:

- `ICustomIdGenerator.Generate(...)` does not fetch sequence itself.
- It only formats the custom ID from:
  - inventory format elements
  - nullable sequence number
  - creation timestamp
- `IInventorySequenceProvider` is responsible for issuing sequence numbers.

Items:

- `CreateInventoryItemUseCase`
- `UpdateInventoryItemUseCase`
- `DeleteInventoryItemUseCase`
- `LikeInventoryItemUseCase`
- `UnlikeInventoryItemUseCase`

Item create flow:

1. Check authentication.
2. Load inventory.
3. Check `CanWriteItems`.
4. If inventory ID format contains sequence element, ask `IInventorySequenceProvider` for the next number.
5. Generate custom ID.
6. Create `InventoryItem` with `CustomId` and nullable `SequenceNumber`.
7. Set custom field values.
8. Add item.
9. Save changes.

## Infrastructure.Data

Uses EF Core + PostgreSQL.

Packages:

- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.Design`
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `EFCore.NamingConventions`

Current naming:

- `UseSnakeCaseNamingConvention()` is enabled.
- Do not manually call `ToTable(...)` unless there is a special reason.
- Database tables/columns should be snake_case.
- Raw SQL must use snake_case names.

DbContext:

- `AppDbContext : DbContext, IUnitOfWork`
- `SaveChangesAsync` comes from `DbContext`.
- `IUnitOfWork` is registered to use the same scoped `AppDbContext`.

Audit:

- `AuditInterceptor` sets:
  - `CreatedAt` for added `AuditableEntity`
  - `UpdatedAt` for modified `AuditableEntity`
- Registered in `AddDbContext` using `.AddInterceptors(...)`.

Configurations:

- Located in `Inventra.Infrastructure.Data/Configurations`.
- Use `IEntityTypeConfiguration<T>`.
- `EntityConfiguration.ConfigureId()` sets:
  - key
  - `ValueGeneratedNever()`

Important EF indexes:

- Inventory item custom ID:
  - unique `(inventory_id, custom_id)`
- Inventory item sequence:
  - unique partial `(inventory_id, sequence_number)` where `sequence_number IS NOT NULL`
- Item like:
  - unique `(item_id, user_id)`
- Inventory access grant:
  - unique `(inventory_id, user_id)`
- Inventory tag:
  - unique `(inventory_id, tag_id)`
- Tag name unique.
- Category name unique.
- User name and email unique.

Repositories:

- `InventoryRepository`
- `InventoryItemRepository`
- `CategoryRepository`
- `TagRepository`
- `UserRepository`

These are command repositories and should return tracked entities.

`InventoryRepository.GetByIdAsync` includes:

- fields
- access grants
- id format elements
- tags
- comments

`InventoryItemRepository.GetByIdAsync` includes:

- field values
- likes

Sequence:

- Data model: `InventorySequence`
  - `InventoryId`
  - `NextValue`
- `InventorySequenceProvider` uses PostgreSQL atomic upsert:

```sql
INSERT INTO inventory_sequences (inventory_id, next_value)
VALUES (@inventoryId, 2)
ON CONFLICT (inventory_id)
DO UPDATE SET next_value = inventory_sequences.next_value + 1
RETURNING inventory_sequences.next_value - 1 AS "Value"
```

This means:

- first sequence is 1;
- next stored value becomes 2;
- concurrent calls are serialized by PostgreSQL row update;
- gaps are possible if item save fails after sequence issue, and that is acceptable for inventory IDs.

Dependency injection:

- `Inventra.Infrastructure.Data/DependencyInjection.cs`
- Public extension:

```csharp
services.AddDataServices(configuration);
```

It registers:

- database
- repositories
- unit of work

## API Layer

Currently mostly default scaffold.

Program.cs currently has:

- controllers
- OpenAPI
- HTTPS redirection
- authorization

Still needed:

- call `builder.Services.AddDataServices(builder.Configuration)`
- add Application DI extension later
- add `ControllerBaseWithResult`
- add controllers
- auth/current user implementation
- permission service implementation

## Local PostgreSQL / Docker Compose Plan

For local migrations/dev, use PostgreSQL in Docker Compose.

Suggested approach:

- Add `docker-compose.yml` at repo root.
- Run Postgres locally with exposed port.
- Store local connection string in `Inventra.Api/appsettings.Development.json`.
- For real deployment, only change `ConnectionStrings:DefaultConnection`.

Example local connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=inventra;Username=inventra;Password=inventra"
  }
}
```

Recommended compose service:

```yaml
services:
  postgres:
    image: postgres:17
    container_name: inventra-postgres
    environment:
      POSTGRES_DB: inventra
      POSTGRES_USER: inventra
      POSTGRES_PASSWORD: inventra
    ports:
      - "5432:5432"
    volumes:
      - inventra-postgres-data:/var/lib/postgresql/data

volumes:
  inventra-postgres-data:
```

Do not hardcode production connection strings.

## Migrations Plan

Needed next:

1. Add Docker Compose for local Postgres.
2. Add local `DefaultConnection`.
3. Wire `AddDataServices` in API `Program.cs`.
4. Ensure EF tools are available:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.7
```

5. Create first migration from Data project with API as startup project:

```powershell
dotnet ef migrations add InitialCreate `
  --project Inventra.Infrastructure.Data `
  --startup-project Inventra.Api `
  --output-dir Migrations
```

6. Apply migration:

```powershell
dotnet ef database update `
  --project Inventra.Infrastructure.Data `
  --startup-project Inventra.Api
```

## Important Remaining Work

Backend priorities:

1. Wire Data services in API.
2. Add Docker Compose and local connection string.
3. Create initial migration and inspect generated schema carefully.
4. Add Application DI extension.
5. Implement `IInventoryPermissionService`.
6. Implement `ICurrentUser`.
7. Add API controllers:
   - inventories
   - fields
   - access settings
   - id format
   - items
   - likes
8. Add comments use cases/controllers.
9. Add admin user use cases/controllers.
10. Add read-side queries:
    - latest inventories
    - top inventories
    - inventory details
    - items table
    - user owned/writable inventories
    - tag/user autocomplete
    - search
    - statistics

Deadline note:

- User has about 9-10 days from 2026-05-11.
- Frontend is planned for the last couple of days.
- Backend should be made runnable quickly now.

## Build Commands

Common build commands:

```powershell
$env:DOTNET_CLI_HOME='M:\ItransitionTasks\Inventra\.dotnet'
dotnet build Inventra.Application\Inventra.Application.csproj
dotnet build Inventra.Infrastructure.Data\Inventra.Infrastructure.Data.csproj
dotnet build Inventra.Api\Inventra.Api.csproj
```

Sometimes sandbox blocks NuGet config/package restore. If build fails with an access or restore issue, rerun with elevated permissions.
