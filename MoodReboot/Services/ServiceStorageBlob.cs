using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using MvcCoreStorage.Models;

namespace MoodReboot.Services
{
    public class ServiceStorageBlob
    {
        private readonly BlobServiceClient blobServiceClient;

        public ServiceStorageBlob(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }

        public async Task<List<string>> GetContainersAsync()
        {
            List<string> containers = new();
            await foreach (var container in this.blobServiceClient.GetBlobContainersAsync())
            {
                containers.Add(container.Name);
            }
            return containers;
        }

        public async Task CreateContainerAsync(string containerName, bool isPublic)
        {
            if (isPublic == true)
            {
                await this.blobServiceClient.CreateBlobContainerAsync(containerName, Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            }
            else
            {
                await this.blobServiceClient.CreateBlobContainerAsync(containerName, Azure.Storage.Blobs.Models.PublicAccessType.None);
            }
        }

        public async Task DeleteContainerAsync(string containerName)
        {
            await this.blobServiceClient.DeleteBlobContainerAsync(containerName);
        }

        public async Task<List<BlobModel>> GetBlobsAsync(string containerName)
        {
            BlobContainerClient blobContainerClient = this.blobServiceClient.GetBlobContainerClient(containerName);

            List<BlobModel> blobModels = new();

            await foreach (var blob in blobContainerClient.GetBlobsAsync())
            {
                BlobClient blobClient = blobContainerClient.GetBlobClient(blob.Name);

                blobModels.Add(new()
                {
                    Contenedor = containerName,
                    Name = blob.Name,
                    Url = blobClient.Uri.AbsoluteUri
                });
            }

            return blobModels;
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient blobContainerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.DeleteBlobAsync(blobName);
        }

        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient =
                this.blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }

        public async Task UpdateBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient =
                this.blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(stream, true);
        }

        public async Task<string> GetBlobUriAsync(string container, string blobName)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            var response = await containerClient.GetPropertiesAsync();
            var properties = response.Value;

            // Will be private if it's None
            if (properties.PublicAccess == Azure.Storage.Blobs.Models.PublicAccessType.None)
            {
                Uri imageUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddSeconds(3600));
                return imageUri.ToString();
            }

            return blobClient.Uri.AbsoluteUri.ToString();
        }
    }
}
