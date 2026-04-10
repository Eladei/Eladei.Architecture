using Eladei.BookRating.Api.Services;

namespace Eladei.BookRating.Api;

public class Program
{
    private const string JustLocalhostAnswer
        = "Communication with gRPC endpoints must be made through a gRPC client...";

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        CompositionRoot.DefineDependencies(builder);

        var app = builder.Build();

        app.MapGrpcService<BookRatingServiceV1>();

        // Добавляем Grpc-рефлексию
        if (app.Environment.IsDevelopment())
        {
            app.MapGrpcReflectionService();
        }

        app.MapGet("/", () => JustLocalhostAnswer);

        await app.RunAsync();
    }
}