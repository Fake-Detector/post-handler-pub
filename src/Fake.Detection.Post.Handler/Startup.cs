using Common.Library.Kafka.Common.Extensions;
using Common.Library.Kafka.Consumer.Extensions;
using Common.Library.Kafka.Producer.Extensions;
using Fake.Detection.Post.Bridge.Contracts;
using Fake.Detection.Post.Handler.Configure;
using Fake.Detection.Post.Handler.Handlers.Audio;
using Fake.Detection.Post.Handler.Handlers.Interfaces;
using Fake.Detection.Post.Handler.Handlers.Photo;
using Fake.Detection.Post.Handler.Handlers.Text;
using Fake.Detection.Post.Handler.Handlers.Video;
using Fake.Detection.Post.Handler.Integration.Extensions;
using Fake.Detection.Post.Handler.Producer;
using Fake.Detection.Post.Handler.Services;
using Fake.Detection.Post.Monitoring.Client.Extensions;

namespace Fake.Detection.Post.Handler;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<FeatureProducerOptions>(_configuration.GetSection(nameof(FeatureProducerOptions)));

        services.AddCommonKafka(_configuration);
        services.AddConsumerHandler<Item, TextConsumerOptions, TextConsumerHandler>(_configuration);
        services.AddConsumerHandler<Item, AudioConsumerOptions, AudioConsumerHandler>(_configuration);
        services.AddConsumerHandler<Item, VideoConsumerOptions, VideoConsumerHandler>(_configuration);
        services.AddConsumerHandler<Item, PhotoConsumerOptions, PhotoConsumerHandler>(_configuration);
        services.AddProducerHandler<Feature>();

        services.AddMonitoring(_configuration);

        services.AddIntegration(_configuration);

        services.AddSingleton<IFeatureProducer, FeatureProducer>();
        services.AddSingleton<ITextHandler, TextTrustHandler>();
        services.AddSingleton<ITextHandler, TextAiHandler>();
        services.AddSingleton<ITextHandler, TextMoodHandler>();
        services.AddSingleton<ITextHandler, TextTagsHandler>();
        services.AddSingleton<ITextHandler, TextLinkHandler>();
        services.AddSingleton<IAudioHandler, AudioTextHandler>();
        services.AddSingleton<IVideoHandler, VideoTextHandler>();
        services.AddSingleton<IPhotoHandler, PhotoTextHandler>();
    }

    public void Configure()
    {
    }
}