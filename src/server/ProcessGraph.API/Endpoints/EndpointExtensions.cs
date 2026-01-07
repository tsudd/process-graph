namespace ProcessGraph.API.Endpoints;

public static class EndpointExtensions
{
    /// <summary>
    /// Registers all API endpoints organized by aggregates
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        var apiGroup = app.MapGroup(Constants.ApiV1);
        apiGroup.MapProcessEndpoints();

        return app;
    }
}