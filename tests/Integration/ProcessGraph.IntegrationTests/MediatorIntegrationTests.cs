using FluentResults;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ProcessGraph.Application.Extensions;
using ProcessGraph.Application.Processes.CreateProcess;
using ProcessGraph.Application.Processes.UpdateProcess;
using ProcessGraph.Application.Processes.DeleteProcess;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;
using System.Collections.Immutable;
using Xunit;

namespace ProcessGraph.IntegrationTests;

public class MediatorIntegrationTests
{
    private readonly IServiceProvider _serviceProvider;

    public MediatorIntegrationTests()
    {
        var services = new ServiceCollection();
        
        // Add Application layer (includes Mediator)
        services.AddApplication();
        
        // Add mock repositories for testing
        services.AddScoped<IProcessRepository, MockProcessRepository>();
        services.AddScoped<IUnitOfWork, MockUnitOfWork>();
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task MediatorServiceResolution_ShouldSucceed()
    {
        // Arrange & Act
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        
        // Assert
        Assert.NotNull(mediator);
    }

    [Fact]
    public async Task HandlerDiscovery_ShouldFindAllHandlers()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        
        // Act & Assert - Test that command handlers can be resolved
        var createCommand = new CreateProcessCommand("Test", "Description");
        var result = await mediator.Send(createCommand);
        Assert.True(result.IsSuccess);

        var processId = result.Value;
        
        var updateCommand = new UpdateProcessCommand(processId, "Updated", "Updated Description", null);
        var updateResult = await mediator.Send(updateCommand);
        Assert.True(updateResult.IsSuccess);

        var deleteCommand = new DeleteProcessCommand(processId);
        var deleteResult = await mediator.Send(deleteCommand);
        Assert.True(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task CreateProcessCommand_ThroughMediator_ShouldReturnGuid()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var command = new CreateProcessCommand("Integration Test Process", "Test Description");
        
        // Act
        var result = await mediator.Send(command);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
    }

    [Fact]
    public async Task UpdateProcessCommand_ThroughMediator_ShouldSucceed()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var createCommand = new CreateProcessCommand("Original Name", "Original Description");
        var createResult = await mediator.Send(createCommand);
        var processId = createResult.Value;
        
        // Act
        var updateCommand = new UpdateProcessCommand(processId, "Updated Name", "Updated Description", null);
        var result = await mediator.Send(updateCommand);
        
        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteProcessCommand_ThroughMediator_ShouldSucceed()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var createCommand = new CreateProcessCommand("To Delete", "Will be deleted");
        var createResult = await mediator.Send(createCommand);
        var processId = createResult.Value;
        
        // Act
        var deleteCommand = new DeleteProcessCommand(processId);
        var result = await mediator.Send(deleteCommand);
        
        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateProcessCommand_NonExistentProcess_ShouldFail()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var nonExistentId = Guid.NewGuid();
        
        // Act
        var updateCommand = new UpdateProcessCommand(nonExistentId, "Updated Name", "Updated Description", null);
        var result = await mediator.Send(updateCommand);
        
        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("not found", result.Errors.First().Message);
    }

    [Fact]
    public async Task DeleteProcessCommand_NonExistentProcess_ShouldFail()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var nonExistentId = Guid.NewGuid();
        
        // Act
        var deleteCommand = new DeleteProcessCommand(nonExistentId);
        var result = await mediator.Send(deleteCommand);
        
        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("not found", result.Errors.First().Message);
    }
}

// Mock implementations for testing
public class MockProcessRepository : IProcessRepository
{
    private readonly Dictionary<Guid, Process> _processes = new();

    public void Add(Process process)
    {
        _processes[process.Id] = process;
    }

    public void Update(Process process)
    {
        _processes[process.Id] = process;
    }

    public void Delete(Process process)
    {
        _processes.Remove(process.Id);
    }

    public async Task<Process?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken); // Simulate async operation
        return _processes.TryGetValue(id, out var process) ? process : null;
    }

    public async Task<IImmutableList<Process>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken); // Simulate async operation
        return _processes.Values.ToImmutableList();
    }
}

public class MockUnitOfWork : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken); // Simulate async operation
        return 1; // Return number of affected rows
    }
}