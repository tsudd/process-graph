using Microsoft.AspNetCore.Mvc;
using ProcessGraph.Application.Processes.CreateProcess;
using ProcessGraph.Application.Processes.GetProcess;
using ProcessGraph.Application.Processes.Dtos;
using ProcessGraph.Application.Abstractions.Pipeline;

namespace ProcessGraph.API.Endpoints;

public static class ProcessEndpoints
{
    public static RouteGroupBuilder MapProcessEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/processes")
            .WithTags("Processes");

        group.MapPost("/", CreateProcess)
            .WithName("CreateProcess")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        
        group.MapGet("/{id:guid}", GetProcess)
            .WithName("GetProcess")
            .Produces<GetProcessDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        
        group.MapGet("/", GetProcesses)
            .WithName("GetProcesses")
            .Produces<IEnumerable<GetProcessDto>>();
        
        group.MapPut("/{id:guid}", UpdateProcess)
            .WithName("UpdateProcess")
            .Produces<GetProcessDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
        
        group.MapDelete("/{id:guid}", DeleteProcess)
            .WithName("DeleteProcess")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        
        return group;
    }

    /// <summary>
    /// Creates a new process with the provided details
    /// </summary>
    /// <param name="request">The process creation request</param>
    /// <param name="handler">The create process handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created process</returns>
    /// <response code="201">Process created successfully</response>
    /// <response code="400">Invalid request data</response>
    private static async Task<IResult> CreateProcess(
        [FromBody] CreateProcessRequest request,
        [FromServices] IRequestHandler<CreateProcess, FluentResults.Result<Guid>> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateProcess(request.Name, request.Description);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess 
            ? Results.Created($"/api/v1/processes/{result.Value}", result.Value)
            : Results.BadRequest(result.Errors.FirstOrDefault()?.Message ?? "Creation failed");
    }
    
    /// <summary>
    /// Retrieves a process by its unique identifier
    /// </summary>
    /// <param name="id">The process ID</param>
    /// <param name="handler">The get process handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The process details</returns>
    /// <response code="200">Process found and returned</response>
    /// <response code="404">Process not found</response>
    private static async Task<IResult> GetProcess(
        [FromRoute] Guid id,
        [FromServices] IRequestHandler<GetProcess, FluentResults.Result<GetProcessDto>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetProcess(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.IsSuccess 
            ? Results.Ok(result.Value)
            // TODO: shift to problem details and automate with filters/validators
            : Results.NotFound(result.Errors.FirstOrDefault()?.Message ?? "Process not found");
    }
    
    /// <summary>
    /// Retrieves a list of all processes
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all processes</returns>
    /// <response code="200">List of processes returned successfully</response>
    private static async Task<IResult> GetProcesses(
        CancellationToken cancellationToken)
    {
        return Results.Ok(new List<GetProcessDto>());
    }
    
    /// <summary>
    /// Updates an existing process with new data
    /// </summary>
    /// <param name="id">The process ID to update</param>
    /// <param name="request">The process update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated process details</returns>
    /// <response code="200">Process updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Process not found</response>
    private static async Task<IResult> UpdateProcess(
        [FromRoute] Guid id,
        [FromBody] UpdateProcessRequest request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Deletes a process by its unique identifier
    /// </summary>
    /// <param name="id">The process ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Process deleted successfully</response>
    /// <response code="404">Process not found</response>
    private static async Task<IResult> DeleteProcess(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return Results.NoContent();
    }
}
