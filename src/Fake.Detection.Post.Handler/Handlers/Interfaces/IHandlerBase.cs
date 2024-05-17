using Fake.Detection.Post.Bridge.Contracts;

namespace Fake.Detection.Post.Handler.Handlers.Interfaces;

public interface IHandlerBase
{
    Task HandleAsync(Item item, CancellationToken cancellationToken);
}