using Microsoft.AspNetCore.Mvc;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon;

namespace ImageModeration.Controllers;

[Route("api/[controller]")]
public class ImageModerationController : ControllerBase
{
    private readonly IAmazonS3 amazonS3;

    public ImageModerationController(IAmazonS3 amazonS3)
    {
        this.amazonS3 = amazonS3;
    }

    [HttpPost]
    public async Task<bool> Post([FromBody] ModerationContent content)
    {
        return await ModerateImageAsync(content.Container, content.ObjectKey);
    }

    public async Task<bool> ModerateImageAsync(Containers container, string objectKey)
    {
        using AmazonRekognitionClient rekognitionClient = new(region: RegionEndpoint.USEast1);
        // Acceder al objeto S3 utilizando el cliente de S3 y la información del evento
        using var response = await this.amazonS3.GetObjectAsync(HelperPathAWS.MapBucketName(container), objectKey);
        using var responseStream = response.ResponseStream;
        // Leer el contenido de la imagen del flujo y convertirlo en bytes
        byte[] imageBytes;
        using (var memoryStream = new MemoryStream())
        {
            await responseStream.CopyToAsync(memoryStream);
            imageBytes = memoryStream.ToArray();
        }

        // Crear una instancia de la solicitud de detección de etiquetas de moderación
        var moderationRequest = new DetectModerationLabelsRequest
        {
            Image = new Amazon.Rekognition.Model.Image
            {
                Bytes = new MemoryStream(imageBytes)
            }
        };

        // Enviar la solicitud de detección de etiquetas de moderación a Amazon Rekognition
        var moderationResponse = await rekognitionClient.DetectModerationLabelsAsync(moderationRequest);

        // Analizar los resultados de la detección de contenido explícito
        var moderationLabels = moderationResponse.ModerationLabels;
        bool isExplicit = moderationLabels.Count > 0;

        if (isExplicit)
        {
            return true;
        }

        return false;
    }
}
