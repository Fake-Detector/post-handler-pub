﻿using Common.Library.Kafka.Consumer.Interfaces;
using Confluent.Kafka;
using Fake.Detection.Post.Bridge.Contracts;
using Fake.Detection.Post.Handler.Handlers.Interfaces;
using Fake.Detection.Post.Monitoring.Client.Services.Interfaces;
using Fake.Detection.Post.Monitoring.Messages;
using Newtonsoft.Json;

namespace Fake.Detection.Post.Handler.Services;

public class PhotoConsumerHandler : IConsumerHandler<Item>
{
    private readonly IEnumerable<IPhotoHandler> _photoHandlers;
    private readonly IMonitoringClient _monitoringClient;
    private readonly ILogger<PhotoConsumerHandler> _logger;

    public PhotoConsumerHandler(
        IEnumerable<IPhotoHandler> photoHandlers,
        IMonitoringClient monitoringClient,
        ILogger<PhotoConsumerHandler> logger)
    {
        _photoHandlers = photoHandlers;
        _monitoringClient = monitoringClient;
        _logger = logger;
    }

    public async Task HandleMessage(ConsumeResult<string, Item> message, CancellationToken cancellationToken)
    {
        var item = message.Message.Value;

        if (item is null || item.Type != ItemType.Image && item.Type != ItemType.ImageUrl)
            return;

        try
        {
            await Task.WhenAll(_photoHandlers.Select(handler => handler.HandleAsync(item, cancellationToken)));
        }
        catch (Exception e)
        {
            await _monitoringClient.SendToMonitoring(
                MonitoringType.Error,
                JsonConvert.SerializeObject(item),
                JsonConvert.SerializeObject(e));

            _logger.LogError(e, "Error while handling: {Message}", JsonConvert.SerializeObject(item));
        }
    }
}