using DotNetEnv;
using Eladei.Architecture.Tests.EntityFramework.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace Eladei.BookRating.IntegrationTests;

internal class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
#if (DEBUG)
        Env.Load();
#endif
        var connectionString = Environment.GetEnvironmentVariable("SERVER_CONNECTION_STRING")
            ?? throw new InvalidOperationException("SERVER_CONNECTION_STRING not defined");

        services.AddSingleton(new NpgsqlConnectionParams
        {
            ConnectionString = connectionString
        });
    }
}