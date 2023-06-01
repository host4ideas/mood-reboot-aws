using MoodReboot.Services;
using MvcCoreAWSS3.Services;
using NugetMoodReboot.Helpers;

namespace MoodReboot.Helpers
{
    public class HelperFileAWS
    {
        private readonly ServiceStorageS3 serviceStorage;
        private readonly ServiceContentModerator contentModerator;

        public HelperFileAWS(ServiceStorageS3 serviceStorage, ServiceContentModerator contentModerator)
        {
            this.serviceStorage = serviceStorage;
            this.contentModerator = contentModerator;
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
                    string urlFile = this.GetBlobUri(container, fileName);

                    var result = await this.contentModerator.ModerateImageAsync(urlFile);

                    if (result.ImageModeration.IsImageAdultClassified == true ||
                    result.ImageModeration.IsImageRacyClassified == true)
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
                    string urlFile = this.GetBlobUri(container, fileName);

                    var result = await this.contentModerator.ModerateImageAsync(urlFile);

                    if (result.ImageModeration.IsImageAdultClassified == true ||
                    result.ImageModeration.IsImageRacyClassified == true)
                    {
                        await this.DeleteFileAsync(container, fileName);
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public string GetBlobUri(Containers container, string blobName)
        {
            string containerAzure = HelperPathAWS.MapBucketName(container);
            return $"https://{containerAzure}.s3.us-east-1.amazonaws.com/" + blobName;
        }

        public async Task DeleteFileAsync(Containers container, string blobName)
        {
            await this.serviceStorage.DeleteFileAsync(blobName, container);
        }
    }
}
