# Task 2.2: Dependency Injection Configuration Update

## Connection to Use Cases
- UC-04: Dependency Injection Update

## Task Goal
Replace manual handler registrations with Mediator's automatic handler discovery while removing custom pipeline registrations and ensuring all handlers are properly discovered and registered with appropriate service lifetime (Scoped).

## Description of Changes

### New files
- No new files required

### Changes to existing files

#### File: `src/server/ProcessGraph.Application/Extensions/ServicesCollectionExtensions.cs`

**Method `AddApplication`:**
- Remove manual handler registrations:
  - Remove line: `services.AddScoped<ICommandHandler<CreateProcessCommand, Guid>, CreateProcessCommandHandler>();`
  - Remove line: `services.AddScoped<IQueryHandler<GetProcessQuery, ProcessResponse>, GetProcessQueryHandler>();`
  - Remove line: `services.AddScoped(typeof(IRequestPipeline<,>), typeof(RequestPipeline<,>));`
- Add Mediator service registration:
  - Add call: `services.AddMediator(options => { options.ServiceLifetime = ServiceLifetime.Scoped; });`
  - Parameters:
    - `options` — Mediator configuration options
    - `ServiceLifetime` — Scoped (matching existing handler lifetime)
  - Returns: IServiceCollection for method chaining
  - Logic: Automatically discovers and registers all handlers implementing IRequestHandler<TRequest, TResponse> in the ProcessGraph.Application assembly

**Updated using statements:**
- Add using: `using Mediator.DependencyInjection;` (for AddMediator extension)
- Remove using: `using ProcessGraph.Application.Processes.CreateProcess;` (no longer needed)
- Remove using: `using ProcessGraph.Application.Processes.GetProcess;` (no longer needed)
- Remove using: `using ProcessGraph.Application.Abstractions;` (RequestPipeline no longer used)

### Component Integration
The Mediator registration will:
- Automatically scan ProcessGraph.Application assembly for handlers
- Register all classes implementing IRequestHandler<TRequest, TResponse>
- Use Scoped lifetime matching existing handler registrations
- Replace the custom RequestPipeline with Mediator's built-in pipeline
- Enable IMediator interface injection throughout the application

## Test Cases

### End-to-End Tests
1. **TC-E2E-01:** Handler Discovery Validation
   - Input data: ProcessGraph.Application assembly with four handlers implementing IRequestHandler
   - Expected result: All handlers (Create, Get, Update, Delete) are automatically discovered and registered
   - Note: Assembly scanning should find all four handler classes

2. **TC-E2E-02:** Service Resolution Integration Test
   - Input data: DI container with Mediator registration
   - Expected result: IMediator can be resolved and used to send requests to handlers
   - Note: Basic request flow through Mediator works end-to-end

3. **TC-E2E-03:** Service Lifetime Verification
   - Input data: Handler instances resolved through IMediator
   - Expected result: Handlers are created with Scoped lifetime (new instance per request scope)
   - Note: Matches existing handler lifetime behavior

### Unit Tests
1. **TC-UNIT-01:** Mediator Service Registration
   - Service under test: IMediator registration in DI container
   - Input data: ServiceCollection with AddMediator() call
   - Expected result: IMediator service is registered and resolvable

2. **TC-UNIT-02:** Handler Registration Verification
   - Services under test: All four handler types
   - Input data: DI container after AddMediator() call
   - Expected result: All handlers can be resolved both directly and through IMediator

3. **TC-UNIT-03:** Pipeline Registration Removal
   - Service under test: IRequestPipeline<,> registration
   - Input data: DI container after removing custom pipeline registration
   - Expected result: IRequestPipeline<,> is no longer registered (throws exception when resolved)

### Regression Tests
- Run all existing tests from the `tests/` directory
- Ensure functionality is not broken: DI container resolves all required services
- Verify application startup succeeds without DI resolution errors
- Confirm no circular dependencies or service resolution conflicts

## Acceptance Criteria
- [ ] Manual handler registrations removed from AddApplication method
- [ ] IRequestPipeline<,> registration removed
- [ ] AddMediator() call added with Scoped service lifetime configuration
- [ ] Mediator.DependencyInjection using statement added
- [ ] Unused using statements removed
- [ ] All four handlers automatically discovered by assembly scanning
- [ ] IMediator service is registered and resolvable
- [ ] Handler service lifetime remains Scoped
- [ ] Application starts successfully without DI errors
- [ ] All existing tests continue to pass

## Notes
### Implementation Details
- Assembly scanning targets the calling assembly (ProcessGraph.Application)
- ServiceLifetime.Scoped maintains existing handler lifetime behavior
- Mediator will register handlers using their IRequestHandler<TRequest, TResponse> interface
- Custom interfaces (ICommandHandler, IQueryHandler) are not registered by Mediator

### Service Registration Pattern
```csharp
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    // Remove these lines:
    // services.AddScoped(typeof(IRequestPipeline<,>), typeof(RequestPipeline<,>));
    // services.AddScoped<ICommandHandler<CreateProcessCommand, Guid>, CreateProcessCommandHandler>();
    // services.AddScoped<IQueryHandler<GetProcessQuery, ProcessResponse>, GetProcessQueryHandler>();

    // Add this line:
    services.AddMediator(options =>
    {
        options.ServiceLifetime = ServiceLifetime.Scoped;
    });

    return services;
}
```

### Assembly Scanning Verification
- Mediator scans for types implementing IRequestHandler<TRequest, TResponse>
- All four handlers should be discovered automatically
- No configuration needed for specific handler types
- Assembly scanning is performed at application startup

### Error Prevention
- Verify no duplicate service registrations
- Check for any missing dependencies in handler constructors
- Ensure assembly scanning finds all expected handlers
- Validate service lifetime consistency

### Migration Safety
- Gradual removal approach: remove old registrations, add Mediator registration
- Verify each step compiles and resolves services correctly
- Test application startup after each change
- Maintain ability to quickly rollback if issues arise