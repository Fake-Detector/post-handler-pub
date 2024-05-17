using System.Globalization;
using Fake.Detection.Post.Bridge.Contracts;
using Fake.Detection.Post.Handler.Handlers.Interfaces;
using Fake.Detection.Post.Handler.Integration.Services.Interfaces;
using Fake.Detection.Post.Handler.Producer;

namespace Fake.Detection.Post.Handler.Handlers.Text;

public class TextTrustHandler : ITextHandler
{
    private readonly IFakeTextDetectorService _fakeTextDetectorService;
    private readonly IFeatureProducer _featureProducer;

    public TextTrustHandler(
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

            var detectorResult = await _fakeTextDetectorService.CheckTrustAsync(item.Data, cancellationToken);

            var feature = new Feature
            {
                ItemId = item.Id,
                Type = FeatureType.Trust,
                Text = Convert.ToString(detectorResult.Result, CultureInfo.InvariantCulture)
            };

            await _featureProducer.Produce(feature, cancellationToken);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}