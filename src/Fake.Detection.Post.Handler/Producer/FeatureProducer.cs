using Common.Library.Kafka.Producer.Interfaces;
using Fake.Detection.Post.Bridge.Contracts;
using Fake.Detection.Post.Handler.Configure;
using Fake.Detection.Post.Monitoring.Client.Services.Interfaces;
using Fake.Detection.Post.Monitoring.Messages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Fake.Detection.Post.Handler.Producer;

public class FeatureProducer : IFeatureProducer
{
    private readonly IProducerHandler<Feature> _resultProducer;
    private readonly IOptionsMonitor<FeatureProducerOptions> _resultProducerOptions;
    private readonly IMonitoringClient _monitoringClient;
    private readonly ILogger<FeatureProducer> _logger;

    public FeatureProducer(
        IProducerHandler<Feature> resultProducer,
        IOptionsMonitor<FeatureProducerOptions> resultProducerOptions,
        IMonitoringClient monitoringClient,
        ILogger<FeatureProducer> logger)
    {
        _resultProducer = resultProducer;
        _resultProducerOptions = resultProducerOptions;
        _monitoringClient = monitoringClient;
        _logger = logger;
    }


    public async Task Produce(Feature feature, CancellationToken cancellationToken)
    {
        try
        {
            var topic = _resultProducerOptions.CurrentValue.TopicName;

            await _resultProducer.Produce(topic: topic, message: feature, cancellationToken);

            await _monitoringClient.SendToMonitoring(MonitoringType.Info, JsonConvert.SerializeObject(feature));
        }
        catch (Exception e)
        {
            await _monitoringClient.SendToMonitoring(
                MonitoringType.Error,
                JsonConvert.SerializeObject(feature),
                JsonConvert.SerializeObject(e));

            _logger.LogError(e, "Error while handling: {Message}", JsonConvert.SerializeObject(feature));
        }
    }
}