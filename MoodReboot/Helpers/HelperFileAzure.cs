using MoodReboot.Services;
using NugetMoodReboot.Helpers;

namespace MoodReboot.Helpers
{
    public class HelperFileAzure
    {
        private readonly ServiceStorageBlob serviceStorage;
        private readonly ServiceContentModerator contentModerator;

        public HelperFileAzure(ServiceStorageBlob serviceStorage, ServiceContentModerator contentModerator)
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
                string containerBlob = HelperPathAzure.MapContainerPath(container);

                using Stream stream = file.OpenReadStream();

                await this.serviceStorage.UploadBlobAsync(containerBlob, fileName, stream);

                if (fileType == FileTypes.Image)
                {
                    string urlFile = await this.GetBlobUriAsync(container, fileName);

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
                string containerBlob = HelperPathAzure.MapContainerPath(container);

                using Stream stream = file.OpenReadStream();
                await this.serviceStorage.UpdateBlobAsync(containerBlob, fileName, stream);

                if (fileType == FileTypes.Image)
                {
                    string urlFile = await this.GetBlobUriAsync(container, fileName);

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

        public async Task<string> GetBlobUriAsync(Containers container, string blobName)
        {
            string containerAzure = HelperPathAzure.MapContainerPath(container);
            return await this.serviceStorage.GetBlobUriAsync(containerAzure, blobName);
        }

        public async Task DeleteFileAsync(Containers container, string blobName)
        {
            string containerAzure = HelperPathAzure.MapContainerPath(container);
            await this.serviceStorage.DeleteBlobAsync(containerAzure, blobName);
        }
    }
}
