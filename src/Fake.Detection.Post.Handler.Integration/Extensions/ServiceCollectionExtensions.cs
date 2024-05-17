using Fake.Detection.Post.Handler.Integration.Configure;
using Fake.Detection.Post.Handler.Integration.Services;
using Fake.Detection.Post.Handler.Integration.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fake.Detection.Post.Handler.Integration.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIntegration(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<GrpcOptions>(config.GetSection(nameof(GrpcOptions)));

        services.AddGrpcClient<FakeDetector.FakeDetector.FakeDetectorClient>((provider, options) =>
        {
            options.Address = new Uri(provider.GetRequiredService<IOptions<GrpcOptions>>().Value.ServerUrl);
        });

        services.AddSingleton<IFakeTextDetectorService, FakeTextDetectorService>();

        return services;
    }
}