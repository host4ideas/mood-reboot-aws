using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using static System.Net.Mime.MediaTypeNames;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambdaRekognition {
    public class Function {
        IAmazonS3 S3Client { get; set; }

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function() {
            S3Client = new AmazonS3Client();
        }

        /// <summary>
        /// Constructs an instance with a preconfigured S3 client. This can be used for testing outside of the Lambda environment.
        /// </summary>
        /// <param name="s3Client"></param>
        public Function(IAmazonS3 s3Client) {
            this.S3Client = s3Client;
        }

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
        /// to respond to S3 notifications.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(S3Event evnt, ILambdaContext context) {
            if (evnt.Records.Count > 0) {
                foreach (var record in evnt.Records) {
                    var s3Event = record.S3;
                    if (s3Event == null) {
                        continue;
                    }

                    try {
                        using (var rekognitionClient = new AmazonRekognitionClient()) {
                            // Acceder al objeto S3 utilizando el cliente de S3 y la información del evento
                            using (var response = await S3Client.GetObjectAsync(s3Event.Bucket.Name, s3Event.Object.Key)) {
                                using (var responseStream = response.ResponseStream) {
                                    // Leer el contenido de la imagen del flujo y convertirlo en bytes
                                    byte[] imageBytes;
                                    using (var memoryStream = new MemoryStream()) {
                                        await responseStream.CopyToAsync(memoryStream);
                                        imageBytes = memoryStream.ToArray();
                                    }

                                    // Crear una instancia de la solicitud de detección de etiquetas de moderación
                                    var moderationRequest = new DetectModerationLabelsRequest {
                                        Image = new Amazon.Rekognition.Model.Image {
                                            Bytes = new MemoryStream(imageBytes)
                                        }
                                    };

                                    // Enviar la solicitud de detección de etiquetas de moderación a Amazon Rekognition
                                    var moderationResponse = await rekognitionClient.DetectModerationLabelsAsync(moderationRequest);

                                    // Analizar los resultados de la detección de contenido explícito
                                    var moderationLabels = moderationResponse.ModerationLabels;
                                    bool isExplicit = moderationLabels.Count > 0;

                                    // context.Logger.LogInformation($"Image {s3Event.Object.Key} moderation result: {(isExplicit ? "Explicit" : "Non-explicit")}");

                                    /* =================== ENVÍO DE MENSAJE ===================
                                    // Crear un mensaje con la información de moderación
                                    var message = new {
                                        ImageKey = s3Event.Object.Key,
                                        IsExplicit = isExplicit
                                    };
                                    var messageJson = JsonConvert.SerializeObject(message);

                                    // Enviar el mensaje al SQS
                                    var sqsClient = new AmazonSQSClient();
                                    var sqsRequest = new SendMessageRequest {
                                        QueueUrl = "",
                                        MessageBody = messageJson
                                    };
                                    await sqsClient.SendMessageAsync(sqsRequest);
                                    =========================================================== */

                                    // Eliminar elemento del Bucket S3 si este es +18
                                    if (isExplicit) {
                                        await S3Client.DeleteObjectAsync(s3Event.Bucket.Name, s3Event.Object.Key);
                                    }
                                }
                            }
                        }
                    } catch (Exception e) {
                        context.Logger.LogError($"Error processing object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
                        context.Logger.LogError(e.Message);
                        context.Logger.LogError(e.StackTrace);
                        throw;
                    }
                }
            }
        }

    }
}
