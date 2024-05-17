using Fake.Detection.Post.Bridge.Contracts;

namespace Fake.Detection.Post.Handler.Producer;

public interface IFeatureProducer
{
    Task Produce(Feature feature, CancellationToken cancellationToken);
}