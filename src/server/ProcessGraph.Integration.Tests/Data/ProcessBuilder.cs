using Bogus;
using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Integration.Tests.Data;

public class ProcessBuilder
{
    private static readonly Faker Faker = new();
    
    private string? _name;
    private string? _description;
    private ProcessSettings _settings = ProcessSettings.CreateDefault();

    public ProcessBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProcessBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ProcessBuilder WithSettings(ProcessSettings settings)
    {
        _settings = settings;
        return this;
    }

    public Process Build()
    {
        var name = _name ?? Faker.Commerce.ProductName();
        var description = _description ?? Faker.Lorem.Paragraph();
        
        return Process.Create(name, description, _settings);
    }
}