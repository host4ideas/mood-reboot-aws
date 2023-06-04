using Amazon;
using Amazon.Translate;
using Amazon.Translate.Model;

namespace MvcAWSTranslate.Services
{
    public class TextModerationService
    {
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
            // If the region you want to use is different from the region
            // defined for the default user, supply it as a parameter to the
            // Amazon Translate client object constructor.
            var client = new AmazonTranslateClient(RegionEndpoint.USEast1);

            var request = new TranslateTextRequest
            {
                SourceLanguageCode = "auto",
                TargetLanguageCode = "en",
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
}
