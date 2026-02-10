# Task 1.1: Package Installation and Project Configuration

## Connection to Use Cases
- UC-01: Package Installation and Configuration

## Task Goal
Install the Mediator NuGet package and configure the ProcessGraph.Application project with necessary package references while maintaining compatibility with existing .NET 10.0 framework and FluentResults.

## Description of Changes

### New files
- No new files required

### Changes to existing files

#### File: `src/server/ProcessGraph.Application/ProcessGraph.Application.csproj`

**Package References Section:**
- Add package reference: `Mediator.Abstractions` (latest stable version compatible with .NET 10.0)
- Add package reference: `Mediator.SourceGenerator` with PrivateAssets settings
  - Parameters:
    - `Version` — Latest stable version from NuGet
    - `PrivateAssets` — "all" (to prevent transitive dependencies)
    - `IncludeAssets` — "runtime; build; native; contentfiles; analyzers"
  - Returns: Compile-time source generation capability
  - Logic: Enables automatic handler discovery and registration through source generators

**Verification Steps:**
- Confirm package compatibility with .NET 10.0
- Verify no dependency conflicts with existing packages (Dapper, Microsoft.Extensions.DependencyInjection, FluentResults)
- Ensure project compiles successfully after package installation

### Component Integration
The Mediator packages will integrate with the existing dependency injection system and provide:
- IMediator interface for request routing
- IRequestHandler<TRequest, TResponse> for handler contracts
- Automatic handler discovery via source generation
- Compatible service registration with current Scoped lifetime patterns

## Test Cases

### End-to-End Tests
1. **TC-E2E-01:** Project Compilation After Package Installation
   - Input data: Mediator packages installed in ProcessGraph.Application.csproj
   - Expected result: Project compiles without errors or warnings
   - Note: Hardcoded success expected - no functional changes yet

2. **TC-E2E-02:** Package Dependency Compatibility
   - Input data: All existing packages plus new Mediator packages
   - Expected result: No package version conflicts or dependency issues
   - Note: Package manager resolves dependencies successfully

### Unit Tests
1. **TC-UNIT-01:** Package Reference Validation
   - Package references under test: Mediator.Abstractions, Mediator.SourceGenerator
   - Input data: Project file with added package references
   - Expected result: Packages are correctly referenced with appropriate version constraints

2. **TC-UNIT-02:** Source Generator Integration
   - Generator under test: Mediator source generator compilation
   - Input data: Project build with Mediator source generator
   - Expected result: Source generator executes during compilation without errors

### Regression Tests
- Run all existing tests from the `tests/` directory
- Ensure functionality is not broken: All current tests pass
- Verify project builds in both Debug and Release configurations
- Confirm no new compiler warnings or errors introduced

## Acceptance Criteria
- [ ] Mediator.Abstractions package added to ProcessGraph.Application.csproj
- [ ] Mediator.SourceGenerator package added with correct PrivateAssets configuration
- [ ] Package versions are compatible with .NET 10.0 framework
- [ ] No package dependency conflicts exist
- [ ] Project compiles successfully without errors or warnings
- [ ] All existing tests continue to pass
- [ ] Build succeeds in both Debug and Release modes
- [ ] No new static analysis warnings introduced

## Notes
### Implementation Details
- Use latest stable versions available at implementation time
- Verify package signatures and authenticity from official NuGet sources
- Consider package vulnerability scanning if available in the development environment
- Document specific versions used for future reference and reproducible builds

### Version Selection Strategy
- Prefer latest stable version for newest features and bug fixes
- Avoid preview or beta versions for production stability
- Check GitHub releases and changelog for breaking changes
- Validate compatibility with existing Microsoft.Extensions.DependencyInjection version

### Troubleshooting
- If version conflicts occur, consider updating Microsoft.Extensions.DependencyInjection
- If source generator fails, verify build tools and SDK versions
- Check for existing conflicting abstractions or interfaces in the project