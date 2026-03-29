using Dapper;
using FluentResults;
using Mediator;
using ProcessGraph.Application.Abstractions.Data;
using ProcessGraph.Application.Models;
using ProcessGraph.Application.Processes.GetProcess;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Domain.Shared;

namespace ProcessGraph.Application.Processes.GetProcesses;

public sealed record GetProcessesQuery(
    string? SearchTerm = null,
    ProcessConnectionFilter ConnectionFilter = ProcessConnectionFilter.Any,
    ProcessSortBy SortBy = ProcessSortBy.CreatedAt,
    SortDirection SortDirection = SortDirection.Descending,
    int Page = 1,
    int Limit = 20
) : IQuery<Result<PagedResult<ProcessResponse>>>;

public sealed class GetProcessesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetProcessesQuery, Result<PagedResult<ProcessResponse>>>
{
    public async ValueTask<Result<PagedResult<ProcessResponse>>> Handle(GetProcessesQuery query, CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        var (whereClause, parameters) = BuildWhereClause(query);
        var orderByClause = BuildOrderByClause(query.SortBy, query.SortDirection);
        
        // Main query for data
        var sql = $"""
                   SELECT 
                       p.id as Id,
                       p.name as Name,
                       p.description as Description,
                       p.status as Status,
                       p.graph as Graph,
                       p.created_at as CreatedAt,
                       p.last_modified_at as LastModifiedAt,
                       p.settings_unit AS Unit
                   FROM processes p
                   {whereClause}
                   {orderByClause}
                   OFFSET @Offset LIMIT @Limit
                   """;

        // Count query for total
        var countSql = $"""
                        SELECT COUNT(*)
                        FROM processes p
                        {whereClause}
                        """;

        var offset = (query.Page - 1) * query.Limit;
        var queryParams = new DynamicParameters(parameters);
        queryParams.Add("@Offset", offset);
        queryParams.Add("@Limit", query.Limit);

        // Execute both queries
        var totalCountTask = connection.QuerySingleAsync<int>(countSql, parameters);
        
        var processesTask = connection.QueryAsync<ProcessResponse, ProcessSettingsModel, ProcessResponse>(
            sql,
            (proc, settings) =>
            {
                proc.ProcessSettings = settings;
                return proc;
            },
            queryParams,
            splitOn: "Unit");

        await Task.WhenAll(totalCountTask, processesTask);

        var totalCount = await totalCountTask;
        var processes = (await processesTask).ToList();

        var result = PagedResult<ProcessResponse>.Create(
            processes,
            totalCount,
            query.Page,
            query.Limit);

        return Result.Ok(result);
    }

    private static (string whereClause, object parameters) BuildWhereClause(GetProcessesQuery query)
    {
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        // Enhanced search term filter using optimized fuzzy search
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            // Use pg_trgm similarity with ranking for better performance and results
            // Combine exact match (ILIKE), similarity search, and word matching
            conditions.Add(@"
                (
                    p.name ILIKE @ExactPattern OR 
                    similarity(p.name, @SearchTerm) > 0.2 OR
                    p.name % @SearchTerm OR
                    to_tsvector('english', p.name) @@ plainto_tsquery('english', @SearchTerm)
                )");
            parameters.Add("@SearchTerm", query.SearchTerm);
            parameters.Add("@ExactPattern", $"%{query.SearchTerm}%");
        }

        // Optimized connection filter using the functional index
        switch (query.ConnectionFilter)
        {
            case ProcessConnectionFilter.WithConnections:
                // Use the functional index for better performance
                conditions.Add(@"
                    (CASE 
                        WHEN EXISTS (
                            SELECT 1 
                            FROM jsonb_array_elements(p.graph->'nodes') AS node 
                            WHERE jsonb_array_length(COALESCE(node->'connections', '[]'::jsonb)) > 0
                        ) THEN true 
                        ELSE false 
                    END) = true");
                break;
            case ProcessConnectionFilter.WithoutConnections:
                conditions.Add(@"
                    (CASE 
                        WHEN EXISTS (
                            SELECT 1 
                            FROM jsonb_array_elements(p.graph->'nodes') AS node 
                            WHERE jsonb_array_length(COALESCE(node->'connections', '[]'::jsonb)) > 0
                        ) THEN true 
                        ELSE false 
                    END) = false");
                break;
            case ProcessConnectionFilter.Any:
            default:
                // No filtering
                break;
        }

        var whereClause = conditions.Count > 0 
            ? "WHERE " + string.Join(" AND ", conditions)
            : "";

        return (whereClause, parameters);
    }

    private static string BuildOrderByClause(ProcessSortBy sortBy, SortDirection sortDirection)
    {
        var direction = sortDirection == SortDirection.Ascending ? "ASC" : "DESC";
        
        return sortBy switch
        {
            ProcessSortBy.Name => $"ORDER BY p.name COLLATE \"C\" {direction}", // Use C collation for better index performance
            ProcessSortBy.CreatedAt => $"ORDER BY p.created_at {direction}", // Will use IX_processes_created_at index
            _ => "ORDER BY p.created_at DESC" // Default to newest first
        };
    }

    /// <summary>
    /// Analyzes query performance and returns the execution plan.
    /// This method is intended for performance testing and optimization.
    /// </summary>
    public async Task<string> AnalyzeQueryPerformanceAsync(GetProcessesQuery query, CancellationToken cancellationToken = default)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        var (whereClause, parameters) = BuildWhereClause(query);
        var orderByClause = BuildOrderByClause(query.SortBy, query.SortDirection);
        
        var offset = (query.Page - 1) * query.Limit;
        var queryParams = new DynamicParameters(parameters);
        queryParams.Add("@Offset", offset);
        queryParams.Add("@Limit", query.Limit);

        // Get query execution plan
        var explainSql = $"""
                         EXPLAIN (ANALYZE, BUFFERS, FORMAT JSON)
                         SELECT 
                             p.id as Id,
                             p.name as Name,
                             p.description as Description,
                             p.status as Status,
                             p.graph as Graph,
                             p.created_at as CreatedAt,
                             p.last_modified_at as LastModifiedAt,
                             p.settings_unit AS Unit
                         FROM processes p
                         {whereClause}
                         {orderByClause}
                         OFFSET @Offset LIMIT @Limit
                         """;

        var plan = await connection.QuerySingleAsync<string>(explainSql, queryParams);
        return plan;
    }
}
