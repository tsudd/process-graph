# Task 2.1: Handler Interface Migration

## Connection to Use Cases
- UC-02: Handler Interface Migration

## Task Goal
Update all existing handler classes to implement Mediator.IRequestHandler<TRequest, TResponse> interface while maintaining compatibility with existing custom handler interfaces and preserving FluentResults return types and method signatures.

## Description of Changes

### New files
- No new files required

### Changes to existing files

#### File: `src/server/ProcessGraph.Application/Processes/CreateProcess/CreateProcessCommand.cs`

**Class `CreateProcessCommandHandler`:**
- Update interface inheritance: Add `Mediator.IRequestHandler<CreateProcessCommand, Result<Guid>>` alongside existing `ICommandHandler<CreateProcessCommand, Guid>`
  - Parameters: Same constructor parameters (IProcessRepository processRepository, IUnitOfWork unitOfWork)
  - Returns: Result<Guid> (unchanged)
  - Current: `ICommandHandler<CreateProcessCommand, Guid>`
  - Target: `ICommandHandler<CreateProcessCommand, Guid>, Mediator.IRequestHandler<CreateProcessCommand, Result<Guid>>`
- Add method `Handle(CreateProcessCommand request, CancellationToken cancellationToken)` — Mediator contract method
  - Parameters:
    - `request` — CreateProcessCommand instance (same as existing HandleAsync parameter)
    - `cancellationToken` — CancellationToken for async operation
  - Returns: Task<Result<Guid>>
  - Logic: Call existing HandleAsync method to avoid code duplication

#### File: `src/server/ProcessGraph.Application/Processes/GetProcess/GetProcessQuery.cs`

**Class `GetProcessQueryHandler`:**
- Update interface inheritance: Add `Mediator.IRequestHandler<GetProcessQuery, Result<ProcessResponse>>` alongside existing `IQueryHandler<GetProcessQuery, ProcessResponse>`
  - Parameters: Same constructor parameters (ISqlConnectionFactory sqlConnectionFactory)
  - Returns: Result<ProcessResponse> (unchanged)
  - Current: `IQueryHandler<GetProcessQuery, ProcessResponse>`
  - Target: `IQueryHandler<GetProcessQuery, ProcessResponse>, Mediator.IRequestHandler<GetProcessQuery, Result<ProcessResponse>>`
- Add method `Handle(GetProcessQuery request, CancellationToken cancellationToken)` — Mediator contract method
  - Parameters:
    - `request` — GetProcessQuery instance
    - `cancellationToken` — CancellationToken for async operation
  - Returns: Task<Result<ProcessResponse>>
  - Logic: Delegate to existing HandleAsync method implementation

#### File: `src/server/ProcessGraph.Application/Processes/UpdateProcess/UpdateProcessCommand.cs`

**Class `UpdateProcessCommandHandler`:**
- Update interface inheritance: Add `Mediator.IRequestHandler<UpdateProcessCommand, Result>` alongside existing `ICommandHandler<UpdateProcessCommand>`
  - Parameters: Same constructor parameters (IProcessRepository processRepository, IUnitOfWork unitOfWork)
  - Returns: Result (unchanged)
  - Current: `ICommandHandler<UpdateProcessCommand>`
  - Target: `ICommandHandler<UpdateProcessCommand>, Mediator.IRequestHandler<UpdateProcessCommand, Result>`
- Add method `Handle(UpdateProcessCommand request, CancellationToken cancellationToken)` — Mediator contract method
  - Parameters:
    - `request` — UpdateProcessCommand instance
    - `cancellationToken` — CancellationToken for async operation
  - Returns: Task<Result>
  - Logic: Forward to existing HandleAsync method without modification

#### File: `src/server/ProcessGraph.Application/Processes/DeleteProcess/DeleteProcessCommand.cs`

**Class `DeleteProcessCommandHandler`:**
- Update interface inheritance: Add `Mediator.IRequestHandler<DeleteProcessCommand, Result>` alongside existing `ICommandHandler<DeleteProcessCommand>`
  - Parameters: Same constructor parameters (IProcessRepository processRepository, IUnitOfWork unitOfWork)
  - Returns: Result (unchanged)
  - Current: `ICommandHandler<DeleteProcessCommand>`
  - Target: `ICommandHandler<DeleteProcessCommand>, Mediator.IRequestHandler<DeleteProcessCommand, Result>`
- Add method `Handle(DeleteProcessCommand request, CancellationToken cancellationToken)` — Mediator contract method
  - Parameters:
    - `request` — DeleteProcessCommand instance
    - `cancellationToken` — CancellationToken for async operation
  - Returns: Task<Result>
  - Logic: Delegate to existing HandleAsync implementation

### Component Integration
Handler classes will implement both interfaces to support gradual migration:
- Custom interfaces (ICommandHandler, IQueryHandler) maintain semantic meaning and existing contracts
- Mediator interfaces (IRequestHandler<T,R>) enable automatic discovery and routing
- Delegation pattern avoids code duplication by having Handle() methods call existing HandleAsync() methods
- All existing business logic, error handling, and FluentResults patterns remain unchanged

## Test Cases

### End-to-End Tests
1. **TC-E2E-01:** Handler Interface Implementation Validation
   - Input data: All four handler classes with dual interface implementation
   - Expected result: All handlers compile and can be instantiated through both interface types
   - Note: Interface implementation only - no behavioral changes expected

2. **TC-E2E-02:** Method Signature Compatibility
   - Input data: Handler methods with both HandleAsync and Handle signatures
   - Expected result: Both method signatures work correctly and produce identical results
   - Note: Handle() delegates to HandleAsync() - same business logic execution

### Unit Tests
1. **TC-UNIT-01:** Dual Interface Resolution
   - Handlers under test: All four handler classes
   - Input data: Handler instantiation through both custom and Mediator interfaces
   - Expected result: Handlers can be resolved and cast to both interface types

2. **TC-UNIT-02:** Method Delegation Validation
   - Methods under test: Handle() and HandleAsync() methods on each handler
   - Input data: Same request object and cancellation token
   - Expected result: Both methods return identical results

### Regression Tests
- Run all existing tests from the `tests/` directory
- Ensure functionality is not broken: All existing handler tests continue to pass
- Verify business logic remains unchanged in all handlers
- Confirm error handling and FluentResults processing works identically

## Acceptance Criteria
- [ ] CreateProcessCommandHandler implements both ICommandHandler<CreateProcessCommand, Guid> and Mediator.IRequestHandler<CreateProcessCommand, Result<Guid>>
- [ ] GetProcessQueryHandler implements both IQueryHandler<GetProcessQuery, ProcessResponse> and Mediator.IRequestHandler<GetProcessQuery, Result<ProcessResponse>>
- [ ] UpdateProcessCommandHandler implements both ICommandHandler<UpdateProcessCommand> and Mediator.IRequestHandler<UpdateProcessCommand, Result>
- [ ] DeleteProcessCommandHandler implements both ICommandHandler<DeleteProcessCommand> and Mediator.IRequestHandler<DeleteProcessCommand, Result>
- [ ] All handlers maintain existing HandleAsync method signatures
- [ ] All handlers add new Handle method signatures matching Mediator contracts
- [ ] Handle methods correctly delegate to HandleAsync methods
- [ ] All handlers compile without errors or warnings
- [ ] FluentResults integration remains unchanged
- [ ] No changes to business logic or error handling
- [ ] All existing tests continue to pass

## Notes
### Implementation Strategy
- Use delegation pattern: Handle() methods call HandleAsync() methods
- Maintain all existing method signatures exactly
- Preserve constructor parameters and dependency injection patterns
- Keep all XML documentation and code comments

### Method Delegation Pattern
```csharp
// Example implementation pattern for all handlers:
public async Task<Result<T>> Handle(TRequest request, CancellationToken cancellationToken)
{
    return await HandleAsync(request, cancellationToken);
}
```

### Interface Coexistence Benefits
- Zero risk: existing code continues to work unchanged
- Gradual migration: can switch to Mediator interfaces incrementally
- Semantic clarity: custom interfaces maintain domain meaning
- Framework compatibility: Mediator interfaces enable automatic discovery

### Compilation Verification
- Verify all handlers compile with both interface implementations
- Check for any method signature ambiguity issues
- Ensure async/await patterns work correctly in delegation
- Validate that Mediator source generators recognize the implementations

### Error Handling Preservation
- All FluentResults error handling patterns remain identical
- Exception handling strategies are preserved
- Validation and business rule enforcement unchanged
- HTTP status code mapping in API layer unaffected