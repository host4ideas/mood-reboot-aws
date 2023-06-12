using Microsoft.Azure.CognitiveServices.ContentModerator;
using MoodReboot.Services;
using MvcCoreAWSS3.Services;
using NugetMoodReboot.Helpers;

namespace MoodReboot.Helpers
{
    public class HelperFileAWS
    {
        private readonly ServiceStorageS3 serviceStorage;
        private readonly ServiceImageModeration imageModeration;

        public HelperFileAWS(ServiceStorageS3 serviceStorage, ServiceImageModeration imageModeration)
        {
            this.serviceStorage = serviceStorage;
            this.imageModeration = imageModeration;
        }

        public Task DeleteFile(int fileId)
        {
            throw new NotImplementedException();
        }

        public bool IsImage(string contentType)
        {
            if (contentType.Contains("image/jpeg") || contentType.Contains("image/png") || contentType.Contains("image/webp"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsExcel(string contentType)
        {
            if (contentType.Contains("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") || contentType.Contains("application/vnd.ms-excel"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsPdf(string contentType)
        {
            if (contentType.Contains("application/pdf"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UploadFileAsync(IFormFile file, Containers container, FileTypes fileType, string fileName)
        {
            string mimeType = file.ContentType;

            bool isValid = false;

            if (file == null || file.Length == 0)
            {
                return false;
            }

            long maxImageSize = 1024 * 1024 * 5;
            long maxDocumentSize = 1024 * 1024 * 20;

            if (fileType == FileTypes.Pdf || fileType == FileTypes.Excel || fileType == FileTypes.Document)
            {
                if (file.Length > maxDocumentSize)
                {
                    return false;
                }
            }
            else if (fileType == FileTypes.Image)
            {
                if (file.Length > maxImageSize)
                {
                    return false;
                }
            }

            switch (fileType)
            {
                case FileTypes.Excel:
                    if (this.IsExcel(mimeType))
                    {
                        isValid = true;
                    }
                    break;

                case FileTypes.Pdf:
                    if (this.IsPdf(mimeType))
                    {
                        isValid = true;
                    }
                    break;

                case FileTypes.Image:
                    if (this.IsImage(mimeType))
                    {
                        isValid = true;
                    }
                    break;

                case FileTypes.Document:
                    if (this.IsExcel(mimeType) || this.IsPdf(mimeType))
                    {
                        isValid = true;
                    }
                    break;
            }

            if (isValid)
            {
                using Stream stream = file.OpenReadStream();

                await this.serviceStorage.UploadFileAsync(fileName, stream, container);

                if (fileType == FileTypes.Image)
                {
                    bool isExplicit = await this.imageModeration.ModerateImageAsync(container, fileName);
                    if (isExplicit)
                    {
                        await this.DeleteFileAsync(container, fileName);
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateFileAsync(IFormFile file, Containers container, FileTypes fileType, string fileName)
        {
            string mimeType = file.ContentType;

            bool isValid = false;

            if (file == null || file.Length == 0)
            {
                return false;
            }

            long maxImageSize = 1024 * 1024 * 5;
            long maxDocumentSize = 1024 * 1024 * 20;

            if (fileType == FileTypes.Pdf || fileType == FileTypes.Excel || fileType == FileTypes.Document)
            {
                if (file.Length > maxDocumentSize)
                {
                    return false;
                }
            }
            else if (fileType == FileTypes.Image)
            {
                if (file.Length > maxImageSize)
                {
                    return false;
                }
            }

            switch (fileType)
            {
                case FileTypes.Excel:
                    if (this.IsExcel(mimeType))
                    {
                        isValid = true;
                    }
                    break;

                case FileTypes.Pdf:
                    if (this.IsPdf(mimeType))
                    {
                        isValid = true;
                    }
                    break;

                case FileTypes.Image:
                    if (this.IsImage(mimeType))
                    {
                        isValid = true;
                    }
                    break;

                case FileTypes.Document:
                    if (this.IsExcel(mimeType) || this.IsPdf(mimeType))
                    {
                        isValid = true;
                    }
                    break;
            }

            if (isValid)
            {
                using Stream stream = file.OpenReadStream();
                await this.serviceStorage.UploadFileAsync(fileName, stream, container);

                if (fileType == FileTypes.Image)
                {
                    bool isExplicit = await this.imageModeration.ModerateImageAsync(container, fileName);
                    if (isExplicit)
                    {
                        await this.DeleteFileAsync(container, fileName);
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Only for public objects
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public string GetBlobPublicUri(Containers container, string blobName)
        {
            string containerAzure = HelperPathAWS.MapBucketName(container);
            return $"https://{containerAzure}.s3.amazonaws.com/" + blobName;
        }

        /// <summary>
        /// Retrieves the Base64 encoded content of an object
        /// </summary>
        /// <param name="containers"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<string> GetBlobBase64(Containers containers, string blobName)
        {
            string fileContent;
            using (Stream imageStream = await this.serviceStorage.GetFileAsync(blobName, containers))
            {
                using MemoryStream memoryStream = new();
                await imageStream.CopyToAsync(memoryStream);
                byte[] bytes = memoryStream.ToArray();
                string base64Image = Convert.ToBase64String(bytes);
                fileContent = "data:;base64," + base64Image;
            }
            return fileContent;
        }

        public async Task DeleteFileAsync(Containers container, string blobName)
        {
            await this.serviceStorage.DeleteFileAsync(blobName, container);
        }
    }
}
