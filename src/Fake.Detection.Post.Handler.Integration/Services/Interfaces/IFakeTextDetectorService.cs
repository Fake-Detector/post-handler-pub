using Fake.Detection.Post.Handler.Integration.Services.Models;

namespace Fake.Detection.Post.Handler.Integration.Services.Interfaces;

public interface IFakeTextDetectorService
{
    Task<TrustResult> CheckTrustAsync(string text, CancellationToken token);
    Task<AiResult> CheckAiAsync(string text, CancellationToken token);
    Task<TagsResult> GetTagsAsync(string text, CancellationToken token);
    Task<MoodResult> GetMoodAsync(string text, CancellationToken token);
    Task<LinkResult> GetLinksAsync(string text, CancellationToken token);
    Task<TranscribeResult> GetTextImage(string url, CancellationToken token);
    Task<TranscribeResult> GetTextAudio(string url, CancellationToken token);
}