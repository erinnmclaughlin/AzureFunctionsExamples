using FunctionsApp.Database;
using FunctionsApp.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(app =>
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    })
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<MyDbContext>(o =>
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            o.UseSqlite($"Data Source={Path.Join(path, "MyDatabase.db")}");
        });
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<MyDbContext>().Database.EnsureCreatedAsync();
}

host.Run();