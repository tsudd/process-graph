using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline.Messaging;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Application.Processes.CreateProcess;

public sealed record CreateProcessCommand(string Name, string? Description) : ICommand<Result<Guid>>, Mediator.IRequest<Result<Guid>>;

public sealed class CreateProcessCommandHandler(IProcessRepository processRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateProcessCommand, Guid>, Mediator.IRequestHandler<CreateProcessCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(CreateProcessCommand command, CancellationToken cancellationToken = default)
    {
        var newProcess = Process.Create(command.Name, command.Description ?? string.Empty,
            ProcessSettings.CreateDefault());

        processRepository.Add(newProcess);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return newProcess.Id.ToResult();
    }

    public async ValueTask<Result<Guid>> Handle(CreateProcessCommand request, CancellationToken cancellationToken)
    {
        return await HandleAsync(request, cancellationToken);
    }
}