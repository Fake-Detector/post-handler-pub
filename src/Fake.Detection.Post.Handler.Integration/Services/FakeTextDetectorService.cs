using Fake.Detection.Post.Handler.Integration.Services.Interfaces;
using Fake.Detection.Post.Handler.Integration.Services.Models;
using FakeDetector;
using Google.Protobuf;

namespace Fake.Detection.Post.Handler.Integration.Services;

public class FakeTextDetectorService : IFakeTextDetectorService
{
    private readonly FakeDetector.FakeDetector.FakeDetectorClient _client;
    private static readonly Semaphore SemaphoreTrust = new(1, 1);
    private static readonly Semaphore SemaphoreAi = new(1, 1);
    private static readonly Semaphore SemaphoreTags = new(1, 1);
    private static readonly Semaphore SemaphoreMood = new(1, 1);
    private static readonly Semaphore SemaphoreLinks = new(1, 1);
    private static readonly Semaphore SemaphoreImage = new(1, 1);
    private static readonly Semaphore SemaphoreAudio = new(1, 1);

    public FakeTextDetectorService(FakeDetector.FakeDetector.FakeDetectorClient client) => _client = client;

    public async Task<TrustResult> CheckTrustAsync(string text, CancellationToken token)
    {
        SemaphoreTrust.WaitOne();
        var request = new CheckTrustRequest { Text = text };

        var response = await _client.CheckTrustAsync(request, cancellationToken: token);
        SemaphoreTrust.Release();

        if (response is null)
            throw new ArgumentNullException(nameof(response), "Response is null");

        return new TrustResult(Result: response.CheckingResult);
    }

    public async Task<AiResult> CheckAiAsync(string text, CancellationToken token)
    {
        SemaphoreAi.WaitOne();
        var request = new CheckAITrustRequest { Text = text };

        var response = await _client.CheckAITrustAsync(request, cancellationToken: token);
        SemaphoreAi.Release();

        if (response is null)
            throw new ArgumentNullException(nameof(response), "Response is null");

        return new AiResult(
            Result: new JsonFormatter(JsonFormatter.Settings.Default.WithFormatDefaultValues(true)).Format(response));
    }

    public async Task<TagsResult> GetTagsAsync(string text, CancellationToken token)
    {
        SemaphoreTags.WaitOne();
        var request = new GenerateTagsRequest { Text = text };

        var response = await _client.GenerateTagsAsync(request, cancellationToken: token);
        SemaphoreTags.Release();

        if (response is null)
            throw new ArgumentNullException(nameof(response), "Response is null");

        return new TagsResult(Tags: response.Tags.ToArray());
    }

    public async Task<MoodResult> GetMoodAsync(string text, CancellationToken token)
    {
        SemaphoreMood.WaitOne();
        var request = new CheckMoodRequest { Text = text };

        var response = await _client.CheckMoodAsync(request, cancellationToken: token);
        SemaphoreMood.Release();

        if (response is null)
            throw new ArgumentNullException(nameof(response), "Response is null");

        return new MoodResult(Mood: response.Mood);
    }

    public async Task<LinkResult> GetLinksAsync(string text, CancellationToken token)
    {
        SemaphoreLinks.WaitOne();
        var request = new CheckSourcesRequest { Text = text };

        var response = await _client.CheckSourcesAsync(request, cancellationToken: token);
        SemaphoreLinks.Release();

        if (response is null)
            throw new ArgumentNullException(nameof(response), "Response is null");

        return new LinkResult(
            Result: new JsonFormatter(JsonFormatter.Settings.Default.WithFormatDefaultValues(true)).Format(response));
    }

    public async Task<TranscribeResult> GetTextImage(string url, CancellationToken token)
    {
        try
        {
            SemaphoreImage.WaitOne();
            var request = new GetTextImageRequest { Url = url };

            var response = await _client.GetTextImageAsync(request, cancellationToken: token);
            SemaphoreImage.Release();

            return new TranscribeResult(response.Text.Trim());
        }
        catch (Exception)
        {
            return new TranscribeResult("");
        }
    }

    public async Task<TranscribeResult> GetTextAudio(string url, CancellationToken token)
    {
        try
        {
            SemaphoreAudio.WaitOne();
            var request = new GetTextAudioRequest { Url = url };

            var response = await _client.GetTextAudioAsync(request, cancellationToken: token);
            SemaphoreAudio.Release();

            return new TranscribeResult(response.Text.Trim());
        }
        catch (Exception)
        {
            return new TranscribeResult("");
        }
    }
}