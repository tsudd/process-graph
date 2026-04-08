using FluentResults;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ProcessGraph.Application.Processes.GetProcess;
using ProcessGraph.Infrastructure.Context;
using ProcessGraph.Integration.Tests.Infrastructure;

namespace ProcessGraph.Integration.Tests.Processes;

public sealed class ProcessesDriver
{
    private readonly IServiceScope scope;
    private readonly ISender sender;
    public readonly ProcessGraphDbContext DbContext;

    public ProcessesDriver(ProcessGraphTestApp app)
    {
        scope = app.GetScope();
        sender = scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = scope.ServiceProvider.GetRequiredService<ProcessGraphDbContext>();
    }

    public async ValueTask<Result<ProcessResponse>> GetProcessAsync(Guid id)
    {
        return await sender.Send(new GetProcessQuery(id));
    }
}
