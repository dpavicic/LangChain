using OpenAI.Audio;
using OpenAI.Constants;
using OpenAI.Models;

namespace LangChain.Providers.OpenAI;

public partial class OpenAiModel : ITextToSpeechModel<OpenAiTextToSpeechSettings?>
{
    #region Methods

    /// <inheritdoc/>
    public async Task<byte[]> GenerateSpeechAsync(
        string input,
        OpenAiTextToSpeechSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));
        settings ??= OpenAiTextToSpeechSettings.Default;
        
        var response = await Api.AudioEndpoint.CreateSpeechAsync(
            request: new SpeechRequest(
                input: input,
                model: new Model(settings.Value.Model),
                voice: settings.Value.Voice,
                responseFormat: settings.Value.ResponseFormat,
                speed: settings.Value.Speed),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            PriceInUsd = TextToSpeechPrices.TryGet(
                model: settings.Value.Model,
                characters: input.Length) ?? 0.0,
        };
        lock (_usageLock)
        {
            TotalUsage += usage;
        }

        return response.ToArray();
    }

    #endregion
}