using Fake.Detection.Post.Bridge.Contracts;
using Fake.Detection.Post.Handler.Handlers.Interfaces;
using Fake.Detection.Post.Handler.Integration.Services.Interfaces;
using Fake.Detection.Post.Handler.Producer;

namespace Fake.Detection.Post.Handler.Handlers.Text;

public class TextAiHandler : ITextHandler
{
    private readonly IFakeTextDetectorService _fakeTextDetectorService;
    private readonly IFeatureProducer _featureProducer;

    public TextAiHandler(
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

            var detectorResult = await _fakeTextDetectorService.CheckAiAsync(item.Data, cancellationToken);

            var feature = new Feature
            {
                ItemId = item.Id,
                Type = FeatureType.AiGenerated,
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