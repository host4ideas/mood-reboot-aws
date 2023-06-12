using Microsoft.AspNetCore.Mvc;
using Amazon.Comprehend;
using Amazon.Comprehend.Model;
using Amazon.Translate;
using Amazon.Translate.Model;
using Amazon;

namespace TextModeration.Controllers;

[Route("api/[controller]")]
public class TextModerationController : ControllerBase
{
    // POST api/values
    [HttpPost]
    public async Task<string> Post([FromBody] string input)
    {
        return await TranslatingTextAsync(input);
    }

    /// <summary>
    /// Use the Amazon Translate Service to translate the document from the
    /// source language to the specified destination language.
    /// </summary>
    /// <param name="client">The Amazon Translate Service client used to
    /// perform the translation.</param>
    /// <param name="text">A string representing the text to translate.</param>
    /// <returns>The text that has been translated to the destination
    /// language.</returns>
    public async Task<string> TranslatingTextAsync(string text)
    {
        string languageCode = await DetectTextLanguageAsync(text);

        // If the region you want to use is different from the region
        // defined for the default user, supply it as a parameter to the
        // Amazon Translate client object constructor.
        var client = new AmazonTranslateClient(RegionEndpoint.USEast1);

        if (languageCode == "es")
        {
            var requestEnglish = new TranslateTextRequest
            {
                SourceLanguageCode = "es",
                TargetLanguageCode = "en",
                Text = text,
                Settings = new TranslationSettings()
                {
                    Profanity = Profanity.MASK,
                    Formality = Formality.INFORMAL
                }
            };

            var responseEnglish = await client.TranslateTextAsync(requestEnglish);

            var requestSpanish = new TranslateTextRequest
            {
                SourceLanguageCode = "en",
                TargetLanguageCode = "es",
                Text = responseEnglish.TranslatedText,
                Settings = new TranslationSettings()
                {
                    Profanity = Profanity.MASK,
                    Formality = Formality.INFORMAL
                }
            };

            var responseSpanish = await client.TranslateTextAsync(requestSpanish);
            return responseSpanish.TranslatedText;
        }
        else
        {
            var request = new TranslateTextRequest
            {
                SourceLanguageCode = "auto",
                TargetLanguageCode = "es",
                Text = text,
                Settings = new TranslationSettings()
                {
                    Profanity = Profanity.MASK,
                    Formality = Formality.INFORMAL
                }
            };

            var response = await client.TranslateTextAsync(request);
            return response.TranslatedText;
        }
    }

    public async Task<string> DetectTextLanguageAsync(string text)
    {
        string detectedLanguage = "";
        var comprehendClient = new AmazonComprehendClient(RegionEndpoint.USEast1);

        // Detect language
        var detectDominantLanguageRequest = new DetectDominantLanguageRequest()
        {
            Text = text
        };
        var detectDominantLanguageResponse = await comprehendClient.DetectDominantLanguageAsync(detectDominantLanguageRequest);

        float highestScore = 0;

        foreach (var dl in detectDominantLanguageResponse.Languages)
        {
            if (dl.Score > highestScore)
            {
                highestScore = dl.Score;
                detectedLanguage = dl.LanguageCode;
            }
        }

        return detectedLanguage;
    }

}