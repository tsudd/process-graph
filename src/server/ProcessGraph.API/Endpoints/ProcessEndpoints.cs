using Microsoft.AspNetCore.Mvc;
using ProcessGraph.API.Requests;
using ProcessGraph.Application.Models;
using ProcessGraph.Application.Processes.CreateProcess;
using ProcessGraph.Application.Processes.GetProcess;
using ProcessGraph.Application.Processes.GetProcesses;
using ProcessGraph.Application.Processes.DeleteProcess;
using ProcessGraph.Application.Processes.UpdateProcess;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Domain.Shared;
using Mediator;

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
            .Produces<ProcessResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/", GetProcesses)
            .WithName("GetProcesses")
            .Produces<PagedResult<ProcessResponse>>();

        group.MapPatch("/{id:guid}", UpdateProcess)
            .WithName("UpdateProcess")
            .Produces<ProcessResponse>()
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
    /// <param name="mediator">The mediator instance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created process</returns>
    /// <response code="201">Process created successfully</response>
    /// <response code="400">Invalid request data</response>
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

    /// <summary>
    /// Retrieves a process by its unique identifier
    /// </summary>
    /// <param name="id">The process ID</param>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The process details</returns>
    /// <response code="200">Process found and returned</response>
    /// <response code="404">Process not found</response>
    private static async Task<IResult> GetProcess(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetProcessQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            // TODO: shift to problem details and automate with filters/validators
            : Results.NotFound(result.Errors.FirstOrDefault()?.Message ?? "Process not found");
    }

    /// <summary>
    /// Retrieves a paginated list of processes with filtering, sorting, and search capabilities
    /// </summary>
    /// <param name="searchTerm">Fuzzy search on process name</param>
    /// <param name="connectionFilter">Filter by connection status: any, withConnections, withoutConnections</param>
    /// <param name="sortBy">Sort by field: name, createdAt</param>
    /// <param name="sortDirection">Sort direction: asc, desc</param>
    /// <param name="page">Page number (starting from 1)</param>
    /// <param name="limit">Items per page (max 100)</param>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list of processes</returns>
    /// <response code="200">Paginated list of processes returned successfully</response>
    /// <response code="400">Invalid query parameters</response>
    private static async Task<IResult> GetProcesses(
        [FromQuery] string? searchTerm,
        [FromQuery] string? connectionFilter,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] int page,
        [FromQuery] int limit,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        // Set defaults
        if (page <= 0) page = 1;
        if (limit <= 0) limit = 20;
        
        // Validate parameters
        if (page < 1)
            return Results.BadRequest("Page must be greater than 0");
        
        if (limit < 1 || limit > 100)
            return Results.BadRequest("Limit must be between 1 and 100");

        // Parse enum values
        var parsedConnectionFilter = ParseConnectionFilter(connectionFilter);
        var parsedSortBy = ParseSortBy(sortBy);
        var parsedSortDirection = ParseSortDirection(sortDirection);

        var query = new GetProcessesQuery(
            SearchTerm: searchTerm,
            ConnectionFilter: parsedConnectionFilter,
            SortBy: parsedSortBy,
            SortDirection: parsedSortDirection,
            Page: page,
            Limit: limit);

        var result = await mediator.Send(query, cancellationToken);
        
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors.FirstOrDefault()?.Message ?? "Query failed");
    }

    private static ProcessConnectionFilter ParseConnectionFilter(string? value)
    {
        return value?.ToLowerInvariant() switch
        {
            "withconnections" => ProcessConnectionFilter.WithConnections,
            "withoutconnections" => ProcessConnectionFilter.WithoutConnections,
            "any" or null => ProcessConnectionFilter.Any,
            _ => ProcessConnectionFilter.Any
        };
    }

    private static ProcessSortBy ParseSortBy(string? value)
    {
        return value?.ToLowerInvariant() switch
        {
            "name" => ProcessSortBy.Name,
            "createdat" or null => ProcessSortBy.CreatedAt,
            _ => ProcessSortBy.CreatedAt
        };
    }

    private static SortDirection ParseSortDirection(string? value)
    {
        return value?.ToLowerInvariant() switch
        {
            "asc" => SortDirection.Ascending,
            "desc" or null => SortDirection.Descending,
            _ => SortDirection.Descending
        };
    }

    /// <summary>
    /// Updates an existing process with new data
    /// </summary>
    /// <param name="id">The process ID to update</param>
    /// <param name="request">The process update request</param>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated process details</returns>
    /// <response code="200">Process updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Process not found</response>
    private static async Task<IResult> UpdateProcess(
        [FromRoute] Guid id,
        [FromBody] UpdateProcessRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProcessCommand(
            id,
            request.Name,
            request.Description,
            request.ProcessSettings
            );
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Results.Ok()
            : Results.NotFound(result.Errors.FirstOrDefault()?.Message ?? "Process not found");
    }

    /// <summary>
    /// Deletes a process by its unique identifier
    /// </summary>
    /// <param name="id">The process ID to delete</param>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Process deleted successfully</response>
    /// <response code="404">Process not found</response>
    private static async Task<IResult> DeleteProcess(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator, 
        CancellationToken cancellationToken)
    {
        var command = new DeleteProcessCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result.Errors.FirstOrDefault()?.Message ?? "Process not found");
    }
}