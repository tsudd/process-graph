using ProcessGraph.Integration.Tests.Infrastructure;
using Reqnroll;
using Reqnroll.BoDi;

namespace ProcessGraph.Integration.Tests.Support;

[Binding]
public class TestServerSupport
{
    [BeforeTestRun(Order = 1)]
    public static async Task InitializeAsync(IObjectContainer objectContainer)
    {
        var app = new ProcessGraphTestApp();
        objectContainer.RegisterInstanceAs(app);
        await app.InitializeAsync();
    }

    [AfterTestRun(Order = 1)]
    public static async Task DisposeAsync(IObjectContainer objectContainer)
    {
        await objectContainer.Resolve<ProcessGraphTestApp>().DisposeAsync();
    }
}