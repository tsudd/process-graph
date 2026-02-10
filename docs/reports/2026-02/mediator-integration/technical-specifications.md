# Technical Specification: Mediator NuGet Package Integration

## 1. General Description

### 1.1. Task Overview
Integrate the Mediator NuGet package (https://github.com/martinothamar/Mediator) into the ProcessGraph.Application project to replace the current custom CQRS pipeline implementation while maintaining compatibility with FluentResults.

### 1.2. Development Objective
- Replace the custom `IRequestPipeline<TRequest, TResponse>` and `IRequestHandler<TRequest, TResponse>` implementation with the Mediator library
- Simplify dependency injection registration by leveraging Mediator's automatic handler discovery
- Maintain backward compatibility with existing FluentResults integration
- Improve maintainability and reduce boilerplate code

### 1.3. Relationship to Existing System
This is a refactoring task that will modernize the CQRS implementation without changing the public API or business logic. The existing commands and queries (CreateProcessCommand, GetProcessQuery, UpdateProcessCommand, DeleteProcessCommand) will be migrated to use Mediator while preserving their current behavior.

## 2. List of Use Cases

### UC-01: Package Installation and Configuration (New)

**Actors:**
- Developer
- Build System
- Dependency Injection Container

**Preconditions:**
- ProcessGraph.Application project exists
- .NET 10.0 framework is configured
- Current custom pipeline implementation is functional

**Main Success Scenario:**
1. Developer adds Mediator NuGet package reference to ProcessGraph.Application.csproj
2. System downloads and installs the package with compatible version
3. Developer updates ServicesCollectionExtensions.cs to register Mediator services
4. DI container successfully registers all Mediator components
5. Build system compiles project without errors

**Alternative Scenarios:**

**A1: Package version incompatibility (at step 2)**
1. System detects .NET version or dependency conflicts
2. Developer selects compatible Mediator version
3. Return to step 2 of main scenario

**A2: DI registration conflicts (at step 4)**
1. System detects conflicting service registrations
2. Developer resolves conflicts by removing old registrations
3. Return to step 4 of main scenario

**Postconditions:**
- Mediator package is installed and configured
- DI container has Mediator services registered
- Project builds successfully

**Acceptance Criteria:**
- ✅ Mediator package version is compatible with .NET 10.0
- ✅ No package dependency conflicts exist
- ✅ AddMediator() extension is properly called in DI configuration
- ✅ Project compiles without errors after package installation

### UC-02: Handler Interface Migration (Modification of existing)

**Actors:**
- Developer
- Compiler
- Existing Handler Classes

**Preconditions:**
- Mediator package is installed
- Current handlers implement custom interfaces (ICommandHandler, IQueryHandler)
- Handlers use FluentResults return types

**Main Success Scenario:**
1. Developer identifies all existing handler classes
2. Developer updates handler interfaces to implement Mediator's IRequestHandler<TRequest, TResponse>
3. Developer maintains FluentResults return types in handler signatures
4. Compiler validates interface implementations
5. All handlers compile successfully with new interfaces

**Alternative Scenarios:**

**A1: Interface signature mismatch (at step 3)**
1. Compiler reports interface implementation errors
2. Developer adjusts handler method signatures to match Mediator contracts
3. Developer ensures FluentResults compatibility is maintained
4. Return to step 4 of main scenario

**A2: Generic constraint conflicts (at step 2)**
1. Compiler reports generic constraint violations
2. Developer reviews and adjusts generic constraints on request/response types
3. Return to step 2 of main scenario

**Postconditions:**
- All handlers implement Mediator interfaces
- FluentResults integration is preserved
- Handler method signatures are consistent
- All handlers compile without errors

**Acceptance Criteria:**
- ✅ CreateProcessCommandHandler implements IRequestHandler<CreateProcessCommand, Result<Guid>>
- ✅ GetProcessQueryHandler implements IRequestHandler<GetProcessQuery, Result<ProcessResponse>>
- ✅ UpdateProcessCommandHandler implements IRequestHandler<UpdateProcessCommand, Result>
- ✅ DeleteProcessCommandHandler implements IRequestHandler<DeleteProcessCommand, Result>
- ✅ All handlers maintain FluentResults return types
- ✅ No compilation errors in handler classes

### UC-03: Request Type Migration (Modification of existing)

**Actors:**
- Developer
- Compiler
- Command/Query Classes

**Preconditions:**
- Handlers are migrated to Mediator interfaces
- Current requests implement custom IRequest<TResponse>, ICommand<TResponse>, IQuery<TResponse>
- FluentResults are used in request type definitions

**Main Success Scenario:**
1. Developer reviews existing command and query classes
2. Developer updates base interfaces to implement Mediator.IRequest<TResponse>
3. Developer ensures FluentResults generic types are preserved
4. Developer validates that all request types compile correctly
5. Compiler confirms successful compilation of all request types

**Alternative Scenarios:**

**A1: Interface inheritance conflicts (at step 2)**
1. Compiler reports multiple interface inheritance issues
2. Developer resolves conflicts by updating interface hierarchy
3. Return to step 2 of main scenario

**A2: Generic type constraint mismatches (at step 3)**
1. Compiler reports generic constraint violations
2. Developer adjusts generic constraints to match Mediator requirements
3. Return to step 3 of main scenario

**Postconditions:**
- All commands implement Mediator.IRequest<Result<T>> or Mediator.IRequest<Result>
- All queries implement Mediator.IRequest<Result<T>>
- FluentResults integration is maintained
- All request types compile successfully

**Acceptance Criteria:**
- ✅ CreateProcessCommand implements Mediator.IRequest<Result<Guid>>
- ✅ GetProcessQuery implements Mediator.IRequest<Result<ProcessResponse>>
- ✅ UpdateProcessCommand implements Mediator.IRequest<Result>
- ✅ DeleteProcessCommand implements Mediator.IRequest<Result>
- ✅ Custom ICommand and IQuery interfaces can coexist or are properly removed
- ✅ No breaking changes to public command/query contracts

### UC-04: Dependency Injection Update (Modification of existing)

**Actors:**
- Developer
- DI Container
- Application Startup

**Preconditions:**
- Mediator package is configured
- Custom pipeline registrations exist in ServicesCollectionExtensions.cs
- Individual handler registrations are manually configured

**Main Success Scenario:**
1. Developer removes manual handler registrations from ServicesCollectionExtensions.cs
2. Developer removes custom RequestPipeline registration
3. Developer adds Mediator service registration with assembly scanning
4. DI container automatically discovers and registers all handlers
5. Application startup successfully resolves all dependencies

**Alternative Scenarios:**

**A1: Handler discovery fails (at step 4)**
1. DI container cannot find handlers in assembly
2. Developer configures explicit assembly scanning parameters
3. Return to step 4 of main scenario

**A2: Service registration conflicts (at step 5)**
1. Application startup reports duplicate or conflicting registrations
2. Developer removes conflicting custom registrations
3. Return to step 5 of main scenario

**Postconditions:**
- Custom pipeline registrations are removed
- Mediator handles all request routing
- All handlers are automatically discovered and registered
- Application starts successfully

**Acceptance Criteria:**
- ✅ Manual handler registrations are removed from ServicesCollectionExtensions.cs
- ✅ IRequestPipeline<,> registration is removed
- ✅ services.AddMediator() is called with proper assembly configuration
- ✅ All handlers are discovered automatically via assembly scanning
- ✅ Application resolves dependencies without DI errors

### UC-05: API Endpoint Updates (Modification of existing)

**Actors:**
- Developer
- API Endpoints
- HTTP Clients

**Preconditions:**
- Mediator is fully configured and registered
- API endpoints inject specific handler types
- Existing endpoints work with current handler injection

**Main Success Scenario:**
1. Developer identifies all API endpoints that inject specific handlers
2. Developer updates endpoints to inject IMediator instead of specific handlers
3. Developer replaces direct handler calls with mediator.Send() calls
4. Developer validates that request/response flows remain unchanged
5. API endpoints process requests successfully through Mediator

**Alternative Scenarios:**

**A1: Mediator injection fails (at step 2)**
1. DI container cannot resolve IMediator
2. Developer verifies Mediator registration in DI configuration
3. Return to step 2 of main scenario

**A2: Send method compatibility issues (at step 3)**
1. Mediator.Send() calls fail due to type mismatches
2. Developer adjusts request/response type mappings
3. Return to step 3 of main scenario

**Postconditions:**
- All endpoints use IMediator instead of specific handlers
- Request processing flows through Mediator pipeline
- HTTP responses remain identical to previous implementation
- No breaking changes to API contracts

**Acceptance Criteria:**
- ✅ CreateProcess endpoint injects IMediator and calls mediator.Send(command)
- ✅ GetProcess endpoint injects IMediator and calls mediator.Send(query)
- ✅ UpdateProcess endpoint injects IMediator and calls mediator.Send(command)
- ✅ DeleteProcess endpoint injects IMediator and calls mediator.Send(command)
- ✅ All endpoints maintain identical HTTP response behavior
- ✅ FluentResults processing remains unchanged in endpoints

### UC-06: Legacy Code Cleanup (New)

**Actors:**
- Developer
- Build System
- Code Analysis Tools

**Preconditions:**
- Mediator integration is complete and functional
- Custom pipeline classes still exist in codebase
- All functionality has been migrated to Mediator

**Main Success Scenario:**
1. Developer runs comprehensive tests to verify Mediator functionality
2. Developer identifies obsolete custom pipeline classes
3. Developer removes unused IRequestPipeline, RequestPipeline, and related classes
4. Developer removes unused custom interfaces (if no longer needed)
5. Build system confirms project compiles without the removed classes

**Alternative Scenarios:**

**A1: Dependency on removed classes found (at step 4)**
1. Compiler reports missing class references
2. Developer identifies and updates remaining dependencies
3. Return to step 4 of main scenario

**A2: Test failures after cleanup (at step 1)**
1. Tests fail indicating incomplete migration
2. Developer identifies and fixes remaining integration issues
3. Return to step 1 of main scenario

**Postconditions:**
- All custom pipeline classes are removed
- Project compiles successfully
- All tests pass
- Codebase contains only Mediator-based implementation

**Acceptance Criteria:**
- ✅ IRequestPipeline<,> interface is removed or marked obsolete
- ✅ RequestPipeline<,> class is removed
- ✅ Custom IRequestHandler is removed if no longer needed
- ✅ All tests continue to pass after cleanup
- ✅ No unused using statements remain
- ✅ Project builds successfully with reduced complexity

## 3. Non-Functional Requirements

### 3.1. Performance
- **Requirement**: Mediator integration must not degrade request processing performance by more than 5%
- **Measurement**: Response time benchmarks for existing API endpoints
- **Rationale**: Ensure the abstraction layer doesn't introduce significant overhead

### 3.2. Compatibility
- **Requirement**: Full compatibility with FluentResults return types must be maintained
- **Validation**: All existing Result<T> and Result return types continue to work
- **Rationale**: Preserve existing error handling and success/failure semantics

### 3.3. Maintainability
- **Requirement**: Reduce boilerplate code by eliminating manual DI registrations
- **Measurement**: Reduction in lines of code in ServicesCollectionExtensions.cs
- **Benefit**: Automatic handler discovery simplifies maintenance

### 3.4. Testability
- **Requirement**: Existing unit tests must continue to work with minimal modifications
- **Validation**: All current tests pass after migration
- **Rationale**: Preserve investment in test coverage

## 4. Constraints and Assumptions

### 4.1. Technical Constraints
- **Framework Version**: Must maintain .NET 10.0 compatibility
- **Package Compatibility**: Mediator package must be compatible with current dependency versions
- **FluentResults**: Must preserve existing FluentResults integration patterns
- **Breaking Changes**: No breaking changes to public API contracts allowed

### 4.2. Business Constraints
- **Timeline**: Integration should be completed as a single cohesive change
- **Risk**: Minimize risk by maintaining existing behavior exactly
- **Testing**: No dedicated test writing required for initial integration

### 4.3. Assumptions
- **Package Stability**: Assuming Mediator package (https://github.com/martinothamar/Mediator) is stable and production-ready
- **Assembly Scanning**: Assuming automatic handler discovery will work with current project structure
- **Performance**: Assuming Mediator overhead is negligible for current request volumes
- **Compatibility**: Assuming Mediator's IRequest<T> is compatible with FluentResults patterns
- **Version Selection**: Assuming latest stable version of Mediator is appropriate

## 5. Open Questions

1. **Package Version Selection**: Which specific version of the Mediator package should be used? Should we use the latest stable version or a specific LTS version for stability?

2. **Interface Coexistence**: Should the custom ICommand<T> and IQuery<T> interfaces be kept alongside Mediator's IRequest<T>, or should they be completely replaced? Keeping them would provide a cleaner semantic separation but adds complexity.

3. **Pipeline Extensibility**: Does the current implementation require any custom pipeline behaviors (logging, validation, caching) that need to be preserved? If so, how should these be implemented using Mediator's pipeline behavior system?

4. **Error Handling Strategy**: Should error handling and Result<T> unwrapping be centralized in a Mediator pipeline behavior, or kept distributed in individual endpoints?

5. **Migration Strategy**: Should this be done as a big-bang replacement or incremental migration? The current specification assumes big-bang, but incremental might be safer.

6. **Performance Validation**: What specific performance benchmarks should be established before and after migration to validate no degradation occurs?

7. **Rollback Plan**: What specific rollback strategy should be implemented if issues are discovered after deployment? Should we maintain a feature flag or branch strategy?

8. **Handler Registration Scope**: Should handlers be registered as Scoped, Transient, or Singleton? Current manual registration uses Scoped - should this be preserved?