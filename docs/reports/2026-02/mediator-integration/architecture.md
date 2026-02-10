# Architecture Document: Mediator NuGet Package Integration

## 1. Task Description

This document defines the target architecture for integrating the Mediator NuGet package (https://github.com/martinothamar/Mediator) into the ProcessGraph.Application project. The integration will replace the current custom CQRS pipeline implementation while maintaining full compatibility with FluentResults and preserving existing API behavior.

**Referenced Technical Specifications:** `/docs/reports/2026-02/mediator-integration/technical-specifications.md`

**Key Requirements:**
- Replace custom `IRequestPipeline<TRequest, TResponse>` with Mediator
- Maintain FluentResults `Result<T>` and `Result` return types
- Enable automatic handler discovery via assembly scanning
- Update API endpoints to use `IMediator` instead of specific handler injections
- Preserve existing command/query semantics through custom interfaces

## 2. Functional Architecture

### 2.1. Functional Components

#### Component: Command Processing
**Purpose:** Handle state-changing operations in the system

**Functions:**
- CreateProcess: Create a new process entity
  - Input data: `CreateProcessCommand` (Name, Description)
  - Output data: `Result<Guid>` (Process ID)
  - Related user cases: UC-02, UC-05

- UpdateProcess: Modify existing process data
  - Input data: `UpdateProcessCommand` (Id, Name, Description, Settings)
  - Output data: `Result` (Success/Failure)
  - Related user cases: UC-02, UC-05

- DeleteProcess: Remove process from system
  - Input data: `DeleteProcessCommand` (Id)
  - Output data: `Result` (Success/Failure)
  - Related user cases: UC-02, UC-05

**Dependencies:**
- Domain layer for business logic validation
- Infrastructure layer for persistence
- Mediator for request routing

#### Component: Query Processing
**Purpose:** Handle data retrieval operations

**Functions:**
- GetProcess: Retrieve process by ID
  - Input data: `GetProcessQuery` (Id)
  - Output data: `Result<ProcessResponse>` (Process details)
  - Related user cases: UC-02, UC-05

**Dependencies:**
- Infrastructure layer for data access
- Application models for response DTOs
- Mediator for request routing

#### Component: Request Mediation
**Purpose:** Route requests to appropriate handlers and manage pipeline execution

**Functions:**
- RequestRouting: Route incoming requests to registered handlers
  - Input data: `IRequest<TResponse>` implementations
  - Output data: Typed response from handler
  - Related user cases: UC-01, UC-04, UC-05

- HandlerDiscovery: Automatically discover and register handlers
  - Input data: Assembly scanning configuration
  - Output data: Registered handler services
  - Related user cases: UC-01, UC-04

**Dependencies:**
- Dependency Injection container
- Handler implementations
- Request type definitions

### 2.2. Functional Component Diagram

```
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│   API Layer     │────── │ Request Mediation│────── │Command Processing│
│                 │       │                 │       │                 │
│ - Endpoints     │       │ - IMediator     │       │ - Create        │
│ - HTTP Routing  │       │ - Auto Discovery│       │ - Update        │
│ - Request/      │       │ - Pipeline Mgmt │       │ - Delete        │
│   Response      │       │                 │       │                 │
└─────────────────┘       └─────────────────┘       └─────────────────┘
                                   │
                                   │
                          ┌─────────────────┐
                          │Query Processing │
                          │                 │
                          │ - GetProcess    │
                          │ - Data Retrieval│
                          │                 │
                          └─────────────────┘
```

## 3. System Architecture

### 3.1. Architectural Style

**Pattern:** Layered Architecture with CQRS and Mediator Pattern

**Reason for Choice:**
- **Layered Architecture:** Maintains clear separation of concerns between API, Application, Domain, and Infrastructure layers
- **CQRS:** Separates command and query responsibilities for better scalability and maintainability
- **Mediator Pattern:** Decouples request senders from receivers, enabling flexible request routing and pipeline behaviors

### 3.2. System Components

#### Component: ProcessGraph.API
**Type:** Web API / Presentation Layer

**Purpose:** HTTP API endpoints for external clients

**Functions Implemented:**
- HTTP request handling
- Request validation and mapping
- Response formatting
- Error handling and status code management

**Technologies:** ASP.NET Core 10.0, Minimal APIs

**Interfaces:**
- Incoming: HTTP requests from clients
- Outgoing: `IMediator.Send()` calls to Application layer

**Dependencies:**
- ProcessGraph.Application (IMediator, Request/Response DTOs)
- Microsoft.AspNetCore.App framework

#### Component: ProcessGraph.Application  
**Type:** Application Services Layer

**Purpose:** Business workflow orchestration and CQRS implementation

**Functions Implemented:**
- Command processing (CreateProcess, UpdateProcess, DeleteProcess)
- Query processing (GetProcess)
- Request mediation and routing
- Handler registration and discovery

**Technologies:** .NET 10.0, Mediator NuGet package, FluentResults

**Interfaces:**
- Incoming: `IMediator.Send()` from API layer
- Outgoing: Domain services, Infrastructure repositories

**Dependencies:**
- **New:** Mediator package (latest stable version)
- **Existing:** FluentResults, ProcessGraph.Domain, ProcessGraph.Infrastructure
- **Removed:** Custom RequestPipeline classes

#### Component: ProcessGraph.Domain
**Type:** Domain Layer

**Purpose:** Core business logic and domain models

**Functions Implemented:**
- Process entity business logic
- Domain rules and validation
- Value objects and domain services

**Technologies:** .NET 10.0

**Interfaces:**
- Incoming: Commands from Application handlers
- Outgoing: Repository interfaces (defined in Domain, implemented in Infrastructure)

**Dependencies:**
- No changes - remains pure domain layer

#### Component: ProcessGraph.Infrastructure
**Type:** Data Access / Infrastructure Layer  

**Purpose:** External system integrations and data persistence

**Functions Implemented:**
- Database access via Dapper
- Repository implementations
- Unit of Work pattern
- SQL connection management

**Technologies:** .NET 10.0, Dapper, SQL Server

**Interfaces:**
- Incoming: Repository calls from Application layer
- Outgoing: Database connections

**Dependencies:**
- No changes - existing infrastructure remains intact

### 3.3. Component Diagram

```
┌─────────────────────────────────────────────────┐
│                ProcessGraph.API                 │
│  ┌─────────────┐    ┌─────────────────────────┐ │
│  │ Endpoints   │───▶│     IMediator           │─┼─┐
│  │             │    │                         │ │ │
│  │ - Process   │    └─────────────────────────┘ │ │
│  │   CRUD      │                                │ │
│  │             │                                │ │
│  └─────────────┘                                │ │
└─────────────────────────────────────────────────┘ │
                                                    │
                                                    ▼
┌─────────────────────────────────────────────────┐ │
│            ProcessGraph.Application             │ │
│  ┌─────────────────┐    ┌─────────────────────┐ │ │
│  │    Mediator     │───▶│     Handlers        │ │ │
│  │                 │    │                     │ │ │
│  │ - Auto Discovery│    │ - Command Handlers  │ │ │
│  │ - Request       │    │ - Query Handlers    │ │ │
│  │   Routing       │    │ - FluentResults     │ │ │
│  │                 │    │                     │ │ │
│  └─────────────────┘    └─────────────────────┘ │ │
└─────────────────────────────────────────────────┘ │
                    │                               │
                    ▼                               │
┌─────────────────────────────────────────────────┐ │
│              ProcessGraph.Domain                │ │
│  ┌─────────────────────────────────────────────┐ │ │
│  │            Process Entity                   │ │ │
│  │                                             │ │ │
│  │ - Business Rules                            │ │ │
│  │ - Domain Services                           │ │ │
│  │ - Repository Interfaces                     │ │ │
│  └─────────────────────────────────────────────┘ │ │
└─────────────────────────────────────────────────┘ │
                    │                               │
                    ▼                               │
┌─────────────────────────────────────────────────┐ │
│           ProcessGraph.Infrastructure           │ │
│  ┌─────────────────────────────────────────────┐ │ │
│  │          Repository Implementations         │ │ │
│  │                                             │ │ │
│  │ - Dapper Data Access                        │ │ │
│  │ - SQL Connection Factory                    │ │ │
│  │ - Unit of Work                              │ │ │
│  └─────────────────────────────────────────────┘ │ │
└─────────────────────────────────────────────────┘ │
                                                    │
                    HTTP Requests                   │
                    ▲                               │
                    │                               │
              ┌─────────────┐                       │
              │   Clients   │                       │
              └─────────────┘                       │
                                                    │
                    Mediator Request Flow            │
                    ◀────────────────────────────────┘
```

## 4. Data Model

### 4.1. Conceptual Data Model

**No Changes Required:** The Mediator integration is purely an architectural refactoring that does not affect the data model. All existing entities, relationships, and database schema remain unchanged.

**Existing Entities:**
- **Process:** Core domain entity with business logic
- **ProcessSettings:** Configuration value object  
- **ProcessResponse:** Application layer DTO for API responses

**Existing Relationships:**
- Process 1:1 ProcessSettings (composition)
- Process 1:N GraphModel (future extension)

## 5. Interfaces

### 5.1. External APIs

**No Changes to Public API Contracts:** All existing HTTP endpoints maintain identical request/response contracts.

#### API: Process Management API

**Protocol:** REST over HTTP/HTTPS

**Base URL:** `/api/v1/processes`

**Authentication:** [Existing authentication preserved]

**Endpoints:** [All existing endpoints preserved with identical signatures]

- **POST /processes** - Create process
- **GET /processes/{id}** - Get process by ID  
- **PATCH /processes/{id}** - Update process
- **DELETE /processes/{id}** - Delete process

### 5.2. Internal Interfaces

#### Interface: API Layer → Application Layer

**Current State:**
```csharp
// Direct handler injection
ICommandHandler<CreateProcessCommand, Guid> handler
```

**Target State:**
```csharp
// Mediator injection
IMediator mediator
```

**Protocol:** In-process method calls

**Request Flow:**
```csharp
// Current: Direct handler call
var result = await handler.HandleAsync(command, cancellationToken);

// Target: Mediator call  
var result = await mediator.Send(command, cancellationToken);
```

#### Interface: Mediator → Handler Implementations

**Protocol:** In-process method calls via reflection and dependency injection

**Handler Contract (Target State):**
```csharp
// Commands
public class CreateProcessCommandHandler : IRequestHandler<CreateProcessCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProcessCommand request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}

// Queries  
public class GetProcessQueryHandler : IRequestHandler<GetProcessQuery, Result<ProcessResponse>>
{
    public async Task<Result<ProcessResponse>> Handle(GetProcessQuery request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

#### Interface: Request Type Definitions

**Current State:**
```csharp
public sealed record CreateProcessCommand(string Name, string? Description) 
    : ICommand<Result<Guid>>;
```

**Target State:**
```csharp
public sealed record CreateProcessCommand(string Name, string? Description) 
    : ICommand<Result<Guid>>, Mediator.IRequest<Result<Guid>>;
```

## 6. Technology Stack

### 6.1. Backend

**Programming Language:** C# 12.0

**Framework:** .NET 10.0

**Reason for Selection:** Maintaining compatibility with existing project requirements and leveraging latest .NET performance improvements.

### 6.2. New Dependencies

#### Mediator Package

**Package:** Mediator (https://github.com/martinothamar/Mediator)

**Recommended Version:** Latest stable version compatible with .NET 10.0

**Reason for Selection:**
- High performance (source generators)
- Minimal allocations
- .NET native implementation
- Strong typing support
- Compatible with existing FluentResults patterns

**Package Installation:**
```xml
<PackageReference Include="Mediator.Abstractions" Version="[latest-stable]" />
<PackageReference Include="Mediator.SourceGenerator" Version="[latest-stable]">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
</PackageReference>
```

### 6.3. Existing Dependencies (Preserved)

**Application Layer:**
- FluentResults (existing version)
- Microsoft.Extensions.DependencyInjection.Abstractions

**Infrastructure Dependencies:**
- Dapper (unchanged)
- SQL Server connectivity (unchanged)

**API Dependencies:**
- ASP.NET Core 10.0 (unchanged)

### 6.4. Removed Dependencies

**Custom Pipeline Classes:** (to be removed after migration)
- `IRequestPipeline<TRequest, TResponse>`
- `RequestPipeline<TRequest, TResponse>`  
- Associated pipeline registration code

## 7. Security

### 7.1. Security Impact Assessment

**No Security Changes:** The Mediator integration is a pure architectural refactoring that maintains the existing security posture.

**Preserved Security Features:**
- All existing authentication mechanisms remain unchanged
- Authorization policies continue to be enforced at the API layer
- Input validation patterns are preserved
- Error handling and information disclosure controls remain intact

**Risk Assessment:** **LOW** - No new security vulnerabilities introduced as the change is purely internal architectural.

## 8. Scalability and Performance

### 8.1. Performance Considerations

**Mediator Overhead:**
- **Source Generator Optimization:** The Mediator package uses compile-time source generation for minimal runtime overhead
- **Zero Allocation:** Request routing uses pre-compiled delegates rather than reflection
- **Expected Impact:** < 5% performance degradation requirement from specifications

**Benchmarking Strategy:**
- Establish baseline performance metrics for existing endpoints
- Compare request processing times before and after migration
- Monitor memory allocation patterns

### 8.2. Scalability Benefits

**Improved Maintainability:**
- Automatic handler discovery reduces manual registration overhead
- Consistent request/response patterns improve developer productivity
- Centralized pipeline management enables future cross-cutting concerns

## 9. Reliability and Fault Tolerance

### 9.1. Error Handling Strategy

**FluentResults Preservation:** All existing error handling patterns using FluentResults are maintained without modification.

**Error Flow:**
1. Handler returns `Result<T>` or `Result`
2. Mediator passes result through unchanged
3. API layer processes results identically to current implementation

**Backward Compatibility:** 100% compatibility with existing error handling code.

## 10. Deployment

### 10.1. Migration Strategy

**Big-Bang Deployment Approach:** Complete migration in single deployment to avoid complexity of dual systems.

**Deployment Steps:**

#### Phase 1: Package Installation and Interface Updates
1. Install Mediator NuGet packages
2. Update all request types to implement `Mediator.IRequest<TResponse>`
3. Update all handlers to implement `Mediator.IRequestHandler<TRequest, TResponse>`
4. Verify compilation success

#### Phase 2: Dependency Injection Configuration  
1. Remove manual handler registrations from `ServicesCollectionExtensions.cs`
2. Remove custom `IRequestPipeline<,>` registration
3. Add Mediator registration with assembly scanning:
   ```csharp
   services.AddMediator(options =>
   {
       options.ServiceLifetime = ServiceLifetime.Scoped;
   });
   ```

#### Phase 3: API Layer Updates
1. Update all endpoint methods to inject `IMediator` instead of specific handlers
2. Replace `handler.HandleAsync()` calls with `mediator.Send()`
3. Verify request/response flow integrity

#### Phase 4: Legacy Cleanup  
1. Remove unused `IRequestPipeline<,>` and `RequestPipeline<,>` classes
2. Remove unused custom interfaces (if no longer needed)
3. Clean up unused using statements

#### Phase 5: Validation and Testing
1. Run comprehensive integration tests
2. Verify all endpoints respond correctly
3. Validate performance benchmarks
4. Confirm error handling behavior

### 10.2. Rollback Plan

**Rollback Strategy:** Git-based rollback to previous commit

**Prerequisites for Rollback:**
- Maintain feature branch during development
- Comprehensive testing before deployment
- Database migrations not required (no schema changes)

**Rollback Steps:**
1. Revert to previous git commit
2. Rebuild and deploy previous version
3. Verify system functionality restoration

### 10.3. Configuration Management

**No Configuration Changes Required:** The Mediator integration does not require any external configuration changes.

**DI Container Configuration:**
```csharp
// In ProcessGraph.Application/Extensions/ServicesCollectionExtensions.cs

public static IServiceCollection AddApplication(this IServiceCollection services)
{
    // Remove manual registrations:
    // services.AddScoped(typeof(IRequestPipeline<,>), typeof(RequestPipeline<,>));
    // services.AddScoped<ICommandHandler<CreateProcessCommand, Guid>, CreateProcessCommandHandler>();
    // services.AddScoped<IQueryHandler<GetProcessQuery, ProcessResponse>, GetProcessQueryHandler>();

    // Add Mediator:
    services.AddMediator(options =>
    {
        options.ServiceLifetime = ServiceLifetime.Scoped;
    });

    return services;
}
```

## 11. Implementation Details

### 11.1. Handler Registration Strategy

**Automatic Discovery:** Mediator will use assembly scanning to automatically discover and register all handlers.

**Assembly Configuration:**
- **Target Assembly:** `ProcessGraph.Application`
- **Handler Interface:** `IRequestHandler<TRequest, TResponse>`
- **Service Lifetime:** `Scoped` (matching current manual registration)

**Registration Code:**
```csharp
services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
}); // Automatically scans the calling assembly (ProcessGraph.Application)
```

### 11.2. Request/Response Flow Through Mediator

**Current Flow:**
```
API Endpoint → Specific Handler Injection → Handler.HandleAsync() → Result<T>
```

**Target Flow:**  
```
API Endpoint → IMediator Injection → mediator.Send() → Handler.Handle() → Result<T>
```

**Implementation Example:**

**Before:**
```csharp
private static async Task<IResult> CreateProcess(
    [FromBody] CreateProcessRequest request,
    [FromServices] ICommandHandler<CreateProcessCommand, Guid> handler,
    CancellationToken cancellationToken)
{
    var command = new CreateProcessCommand(request.Name, request.Description);
    var result = await handler.HandleAsync(command, cancellationToken);
    return result.IsSuccess
        ? Results.Created($"/api/v1/processes/{result.Value}", result.Value)
        : Results.BadRequest(result.Errors.FirstOrDefault()?.Message ?? "Creation failed");
}
```

**After:**
```csharp
private static async Task<IResult> CreateProcess(
    [FromBody] CreateProcessRequest request,
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken)
{
    var command = new CreateProcessCommand(request.Name, request.Description);
    var result = await mediator.Send(command, cancellationToken);
    return result.IsSuccess
        ? Results.Created($"/api/v1/processes/{result.Value}", result.Value)
        : Results.BadRequest(result.Errors.FirstOrDefault()?.Message ?? "Creation failed");
}
```

### 11.3. Integration Points Between Projects

**ProcessGraph.API → ProcessGraph.Application:**
- **Current:** Direct handler interface dependencies
- **Target:** Single `IMediator` interface dependency
- **Benefit:** Reduced coupling, simplified dependency management

**ProcessGraph.Application Internal:**
- **Current:** Manual handler registration in DI
- **Target:** Automatic discovery via Mediator
- **Benefit:** Reduced boilerplate, automatic registration of new handlers

**ProcessGraph.Application → ProcessGraph.Domain/Infrastructure:**
- **No Changes:** Handler implementations continue to use existing domain and infrastructure dependencies

### 11.4. Backwards Compatibility Strategy

**Custom Interface Coexistence:**
```csharp
// Option 1: Maintain semantic interfaces for clarity
public interface ICommand<TResponse> : Mediator.IRequest<TResponse>, IBaseCommand;
public interface IQuery<TResponse> : Mediator.IRequest<Result<TResponse>>;

// Option 2: Direct Mediator interface usage (simpler)
public sealed record CreateProcessCommand(string Name, string? Description) 
    : Mediator.IRequest<Result<Guid>>;
```

**Recommendation:** Use Option 1 to maintain semantic clarity and enable future extensions.

**Migration Path:**
1. **Phase 1:** Add Mediator interfaces alongside existing interfaces
2. **Phase 2:** Update DI configuration to use Mediator
3. **Phase 3:** Optionally remove custom interfaces if desired (future refactoring)

## 12. Open Questions

**No Blocking Questions Identified:** The technical specifications provide sufficient detail for implementation. The following items are recommended for future consideration but do not block the current implementation:

1. **Performance Benchmarking:** Specific performance test scenarios to validate < 5% degradation requirement
2. **Pipeline Behaviors:** Future consideration for implementing cross-cutting concerns (logging, validation) using Mediator pipeline behaviors  
3. **Handler Scope Management:** Validation that Scoped lifetime is optimal for current usage patterns
4. **Interface Evolution:** Long-term strategy for custom ICommand/IQuery interfaces vs pure Mediator interfaces

---

**Document Version:** 1.0  
**Last Updated:** February 10, 2026  
**Approved By:** [To be filled during review]