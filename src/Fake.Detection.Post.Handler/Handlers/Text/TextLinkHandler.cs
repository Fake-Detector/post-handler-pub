using Fake.Detection.Post.Bridge.Contracts;
using Fake.Detection.Post.Handler.Handlers.Interfaces;
using Fake.Detection.Post.Handler.Integration.Services.Interfaces;
using Fake.Detection.Post.Handler.Producer;

namespace Fake.Detection.Post.Handler.Handlers.Text;

public class TextLinkHandler : ITextHandler
{
    private readonly IFakeTextDetectorService _fakeTextDetectorService;
    private readonly IFeatureProducer _featureProducer;

    public TextLinkHandler(
        IFakeTextDetectorService fakeTextDetectorService,
        IFeatureProducer featureProducer)
    {
        _fakeTextDetectorService = fakeTextDetectorService;
        _featureProducer = featureProducer;
    }

    public async Task HandleAsync(Item item, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(item.Data))
                return;

            var detectorResult = await _fakeTextDetectorService.GetLinksAsync(item.Data, cancellationToken);

            var feature = new Feature
            {
                ItemId = item.Id,
                Type = FeatureType.Link,
                Text = detectorResult.Result
            };

            await _featureProducer.Produce(feature, cancellationToken);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}