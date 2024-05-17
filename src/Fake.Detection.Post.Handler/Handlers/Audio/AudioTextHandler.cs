using Fake.Detection.Post.Bridge.Contracts;
using Fake.Detection.Post.Handler.Handlers.Interfaces;
using Fake.Detection.Post.Handler.Integration.Services.Interfaces;
using Fake.Detection.Post.Handler.Producer;

namespace Fake.Detection.Post.Handler.Handlers.Audio;

public class AudioTextHandler : IAudioHandler
{
    private readonly IFeatureProducer _featureProducer;
    private readonly IFakeTextDetectorService _fakeTextDetectorService;
    private readonly IEnumerable<ITextHandler> _textHandlers;

    public AudioTextHandler(
        IFeatureProducer featureProducer,
        IFakeTextDetectorService fakeTextDetectorService,
        IEnumerable<ITextHandler> textHandlers)
    {
        _featureProducer = featureProducer;
        _fakeTextDetectorService = fakeTextDetectorService;
        _textHandlers = textHandlers;
    }

    public async Task HandleAsync(Item item, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(item.Data))
                return;

            var result = await _fakeTextDetectorService.GetTextAudio(item.Data, cancellationToken);

            var feature = new Feature
            {
                ItemId = item.Id,
                Type = FeatureType.Transcription,
                Text = result.Text
            };

            await _featureProducer.Produce(feature, cancellationToken);

            try
            {
                await Task.WhenAll(_textHandlers.Select(handler => handler.HandleAsync(new Item
                {
                    Id = item.Id,
                    PostId = item.PostId,
                    Type = ItemType.Text,
                    Data = result.Text
                }, cancellationToken)));
            }
            catch (Exception)
            {
                // ignored
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}