using FluentAssertions;
using FluentResults;
using ProcessGraph.Application.Models;
using ProcessGraph.Application.Processes.GetProcess;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Integration.Tests.Data;
using Reqnroll;

namespace ProcessGraph.Integration.Tests.Processes;

[Binding]
[Scope(Tag = "get_process")]
public class GetProcessStepDefinitions(ProcessesDriver processesDriver)
{
    private Result<ProcessResponse>? _response;
    private Process? _process;

    [Given("a process exists in the system")]
    public async Task GivenAProcessWithGuidExistsInTheSystemAsync()
    {
        _process = await processesDriver.DbContext.AddProcessAsync();
    }

    [When("I request the process with a valid GUID")]
    public async Task WhenIRequestAProcessWithGuidAsync()
    {
        _response = await processesDriver.GetProcessAsync(_process.Id);
    }

    [Then("I should receive process details")]
    public void ThenIShouldReceiveAResponseWithStatusCode200()
    {
        _response?.Value.Should().NotBeNull();
        _response?.Value.Should().BeEquivalentTo(new ProcessResponse
        {
            Id = _process!.Id,
            Name = _process.Name,
            Description = _process.Description,
            ProcessSettings = new ProcessSettingsModel(_process.Settings.Unit.Unit),
            Status = (int)_process.Status,
            CreatedAt = _process.CreatedAt,
            LastModifiedAt = _process.LastModifiedAt,
            Graph = GraphModel.CreateEmpty()
        }, options => options.Excluding(r => r.CreatedAt).Excluding(r => r.LastModifiedAt));
    }
}