# Task 3.1: API Endpoint Updates

## Connection to Use Cases
- UC-05: API Endpoint Updates

## Task Goal
Update all API endpoints in ProcessEndpoints.cs to inject IMediator instead of specific handler types and replace direct handler calls with mediator.Send() calls while maintaining identical HTTP request/response behavior and FluentResults processing.

## Description of Changes

### New files
- No new files required

### Changes to existing files

#### File: `src/server/ProcessGraph.API/Endpoints/ProcessEndpoints.cs`

**Method `CreateProcess`:**
- Change parameter injection from `[FromServices] ICommandHandler<CreateProcessCommand, Guid> handler` to `[FromServices] IMediator mediator`
- Replace handler call `await handler.HandleAsync(command, cancellationToken)` with `await mediator.Send(command, cancellationToken)`
  - Parameters remain: CreateProcessRequest request, CancellationToken cancellationToken
  - Returns: Same IResult with identical HTTP responses
  - Logic: Request routing changes from direct handler to Mediator pipeline, but response processing remains identical

**Method `GetProcess`:**
- Change parameter injection from `[FromServices] IQueryHandler<GetProcessQuery, ProcessResponse> handler` to `[FromServices] IMediator mediator`
- Replace handler call `await handler.HandleAsync(query, cancellationToken)` with `await mediator.Send(query, cancellationToken)`
  - Parameters remain: Guid id, CancellationToken cancellationToken
  - Returns: Same IResult with identical HTTP responses
  - Logic: Query routing through Mediator instead of direct handler injection

**Method `UpdateProcess`:**
- Change parameter injection from `[FromServices] UpdateProcessCommandHandler handler` to `[FromServices] IMediator mediator`
- Replace handler call `await handler.HandleAsync(command, cancellationToken)` with `await mediator.Send(command, cancellationToken)`
  - Parameters remain: Guid id, UpdateProcessRequest request, CancellationToken cancellationToken
  - Returns: Same IResult with identical HTTP responses
  - Logic: Command processing through Mediator pipeline maintains all existing behavior

**Method `DeleteProcess`:**
- Change parameter injection from `[FromServices] DeleteProcessCommandHandler handler` to `[FromServices] IMediator mediator`
- Replace handler call `await handler.HandleAsync(command, cancellationToken)` with `await mediator.Send(command, cancellationToken)`
  - Parameters remain: Guid id, CancellationToken cancellationToken
  - Returns: Same IResult with identical HTTP responses
  - Logic: Delete operation routing through Mediator with preserved error handling

**Updated using statements:**
- Add using: `using Mediator;` (for IMediator interface)
- Remove using: `using ProcessGraph.Application.Abstractions.Pipeline.Messaging;` (direct handler interfaces no longer used)
- Remove using: `using ProcessGraph.Application.Processes.DeleteProcess;` (handler type no longer directly referenced)
- Remove using: `using ProcessGraph.Application.Processes.UpdateProcess;` (handler type no longer directly referenced)

### Component Integration
The Mediator integration will:
- Route all requests through IMediator.Send() instead of direct handler calls
- Maintain identical request/response flows and error handling
- Preserve all FluentResults processing and HTTP status code mapping
- Enable centralized request pipeline management through Mediator
- Reduce coupling between API layer and specific handler implementations

## Test Cases

### End-to-End Tests
1. **TC-E2E-01:** CreateProcess Endpoint Flow
   - Input data: Valid CreateProcessRequest JSON
   - Expected result: HTTP 201 Created with process ID, identical to current behavior
   - Note: Request flows through Mediator instead of direct handler but produces same result

2. **TC-E2E-02:** GetProcess Endpoint Flow
   - Input data: Valid process ID GUID
   - Expected result: HTTP 200 OK with ProcessResponse data, identical to current behavior
   - Note: Query processing through Mediator maintains exact same response format

3. **TC-E2E-03:** UpdateProcess Endpoint Flow
   - Input data: Valid process ID and UpdateProcessRequest JSON
   - Expected result: HTTP 200 OK for success or 404 for not found, identical to current behavior
   - Note: Command processing preserves all validation and error handling

4. **TC-E2E-04:** DeleteProcess Endpoint Flow
   - Input data: Valid process ID GUID
   - Expected result: HTTP 204 No Content for success or 404 for not found, identical to current behavior
   - Note: Delete operation maintains existing business logic and response codes

5. **TC-E2E-05:** Error Handling Preservation
   - Input data: Invalid requests (missing data, non-existent IDs, validation failures)
   - Expected result: Same error responses and HTTP status codes as current implementation
   - Note: FluentResults error processing remains completely unchanged

### Unit Tests
1. **TC-UNIT-01:** IMediator Injection Validation
   - Endpoint methods under test: All four endpoint methods
   - Input data: Valid request parameters and IMediator instance
   - Expected result: IMediator can be injected and resolved successfully

2. **TC-UNIT-02:** Send Method Integration
   - Method under test: mediator.Send() calls in all endpoints
   - Input data: Valid command/query objects and cancellation tokens
   - Expected result: Send method returns results compatible with existing response processing

3. **TC-UNIT-03:** Request/Response Type Compatibility
   - Types under test: All request commands/queries and their response types
   - Input data: Existing request objects with Mediator interface compatibility
   - Expected result: All types work correctly with mediator.Send() generic method

### Regression Tests
- Run all existing API tests from the `tests/` directory
- Ensure functionality is not broken: All endpoint tests pass without modification
- Verify HTTP client integration tests continue to work
- Confirm OpenAPI/Swagger documentation generation remains correct
- Test authentication and authorization if implemented

## Acceptance Criteria
- [ ] CreateProcess endpoint injects IMediator and uses mediator.Send(command)
- [ ] GetProcess endpoint injects IMediator and uses mediator.Send(query)
- [ ] UpdateProcess endpoint injects IMediator and uses mediator.Send(command)
- [ ] DeleteProcess endpoint injects IMediator and uses mediator.Send(command)
- [ ] All endpoints maintain identical HTTP request/response behavior
- [ ] FluentResults processing remains unchanged in all endpoints
- [ ] Error handling and status code mapping preserved exactly
- [ ] All endpoint method signatures unchanged except for handler parameter
- [ ] All existing API tests continue to pass
- [ ] No breaking changes to HTTP contracts or API documentation

## Notes
### Implementation Details
- Only change parameter injection type and method call - all other code remains identical
- Preserve all existing error handling, status code mapping, and response formatting
- Maintain XML documentation comments on all endpoint methods
- Keep all existing validation and security patterns

### Method Call Pattern
```csharp
// Before:
var result = await handler.HandleAsync(command, cancellationToken);

// After:
var result = await mediator.Send(command, cancellationToken);
```

### Response Processing Preservation
- All Results.Created(), Results.Ok(), Results.BadRequest(), Results.NotFound() calls remain identical
- Error message extraction from FluentResults continues unchanged
- HTTP response body formatting stays the same
- Status code logic preserved exactly

### API Contract Maintenance
- HTTP method verbs (POST, GET, PATCH, DELETE) unchanged
- Request/response body schemas identical
- URL routing patterns preserved
- Content-Type and Accept headers unchanged
- OpenAPI specification remains valid

### Error Scenario Validation
- Test all error paths: validation failures, not found scenarios, internal errors
- Verify exception handling behavior is preserved
- Ensure proper HTTP status codes for all error conditions
- Confirm error response body formats remain consistent

### Performance Considerations
- Mediator adds minimal overhead compared to direct handler injection
- Request routing performance should be within 5% of baseline
- Memory allocation patterns should remain similar
- Response time benchmarks should show negligible difference