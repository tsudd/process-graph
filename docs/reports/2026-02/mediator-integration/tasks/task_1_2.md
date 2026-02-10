# Task 1.2: Request Type Interface Updates

## Connection to Use Cases
- UC-03: Request Type Migration

## Task Goal
Update all existing command and query classes to implement Mediator.IRequest<TResponse> interface while maintaining compatibility with existing custom ICommand and IQuery interfaces, preserving FluentResults return types.

## Description of Changes

### New files
- No new files required

### Changes to existing files

#### File: `src/server/ProcessGraph.Application/Processes/CreateProcess/CreateProcessCommand.cs`

**Record `CreateProcessCommand`:**
- Update interface inheritance: Add `Mediator.IRequest<Result<Guid>>` alongside existing `ICommand<Result<Guid>>`
  - Current: `ICommand<Result<Guid>>`
  - Target: `ICommand<Result<Guid>>, Mediator.IRequest<Result<Guid>>`
  - Logic: Dual interface implementation enables gradual migration while maintaining semantic clarity

#### File: `src/server/ProcessGraph.Application/Processes/GetProcess/GetProcessQuery.cs`

**Record `GetProcessQuery`:**
- Update interface inheritance: Add `Mediator.IRequest<Result<ProcessResponse>>` alongside existing `IQuery<ProcessResponse>`
  - Current: `IQuery<ProcessResponse>`
  - Target: `IQuery<ProcessResponse>, Mediator.IRequest<Result<ProcessResponse>>`
  - Logic: Query types map to Result<T> for FluentResults compatibility

#### File: `src/server/ProcessGraph.Application/Processes/UpdateProcess/UpdateProcessCommand.cs`

**Record `UpdateProcessCommand`:**
- Update interface inheritance: Add `Mediator.IRequest<Result>` alongside existing `ICommand`
  - Current: `ICommand`
  - Target: `ICommand, Mediator.IRequest<Result>`
  - Logic: Commands returning Result (not Result<T>) use base Result type

#### File: `src/server/ProcessGraph.Application/Processes/DeleteProcess/DeleteProcessCommand.cs`

**Record `DeleteProcessCommand`:**
- Update interface inheritance: Add `Mediator.IRequest<Result>` alongside existing `ICommand<Result>`
  - Current: `ICommand<Result>`
  - Target: `ICommand<Result>, Mediator.IRequest<Result>`
  - Logic: Delete operations return Result without additional data

### Component Integration
All request types will implement both custom semantic interfaces (ICommand/IQuery) and Mediator interfaces:
- Maintains existing semantic distinction between commands and queries
- Enables Mediator routing through IRequest<T> interface
- Preserves FluentResults return type patterns
- Allows gradual removal of custom interfaces in future if desired

## Test Cases

### End-to-End Tests
1. **TC-E2E-01:** Request Type Compilation Validation
   - Input data: All four request types with updated interface inheritance
   - Expected result: All request types compile without errors
   - Note: Interface changes are structural only - no behavioral changes

2. **TC-E2E-02:** FluentResults Compatibility Maintenance
   - Input data: Request types with Result and Result<T> return types
   - Expected result: All return types remain compatible with FluentResults patterns
   - Note: No changes to actual Result types or error handling

### Unit Tests
1. **TC-UNIT-01:** Interface Implementation Verification
   - Types under test: CreateProcessCommand, GetProcessQuery, UpdateProcessCommand, DeleteProcessCommand
   - Input data: Request type instantiation
   - Expected result: Each type successfully implements both custom and Mediator interfaces

2. **TC-UNIT-02:** Generic Type Constraint Validation
   - Constraints under test: Mediator.IRequest<TResponse> generic constraints
   - Input data: Request types with Result and Result<T> generic parameters
   - Expected result: All generic type constraints satisfied without compiler errors

### Regression Tests
- Run all existing tests from the `tests/` directory
- Ensure functionality is not broken: All command and query instantiation continues to work
- Verify serialization/deserialization if used in tests
- Confirm no changes to request type behavior or semantics

## Acceptance Criteria
- [ ] CreateProcessCommand implements both ICommand<Result<Guid>> and Mediator.IRequest<Result<Guid>>
- [ ] GetProcessQuery implements both IQuery<ProcessResponse> and Mediator.IRequest<Result<ProcessResponse>>
- [ ] UpdateProcessCommand implements both ICommand and Mediator.IRequest<Result>
- [ ] DeleteProcessCommand implements both ICommand<Result> and Mediator.IRequest<Result>
- [ ] All request types compile without errors or warnings
- [ ] FluentResults return types are preserved exactly
- [ ] No behavioral changes to existing request processing
- [ ] All existing tests continue to pass
- [ ] No breaking changes to public contracts

## Notes
### Implementation Details
- Use explicit interface implementation if needed to resolve naming conflicts
- Maintain exact same record constructors and properties
- Preserve XML documentation comments on request types
- Keep consistent naming and namespace patterns

### FluentResults Integration Strategy
- Result<T> types map directly to Mediator response types
- Base Result type (for commands with no return data) maps to Mediator.IRequest<Result>
- Query types always return Result<T> where T is the data type
- Error handling patterns remain completely unchanged

### Interface Coexistence
- Custom ICommand/IQuery interfaces provide semantic meaning
- Mediator IRequest<T> interfaces enable framework integration
- Both interfaces can coexist indefinitely or custom ones can be removed later
- This approach minimizes risk and maintains backward compatibility

### Compilation Verification
- Verify all request types compile in both Debug and Release modes
- Check for any ambiguous reference warnings
- Ensure IntelliSense and tooling continue to work properly
- Validate that source generators process the new interfaces correctly