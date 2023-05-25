using Microsoft.AspNetCore.Http;

namespace NugetMoodReboot.Helpers
{
    public enum FileTypes { Image, Document, Excel, Pdf }

    public class HelperFile
    {
        private readonly HelperPath helperPath;

        public HelperFile(HelperPath helperPath)
        {
            this.helperPath = helperPath;
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

        /// <summary>
        /// Returns the file's fileName if the file was correctly uploaded, otherwise returns null
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folder"></param>
        /// <param name="fileType"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string?> UploadFileAsync(IFormFile file, Folders folder, FileTypes fileType, string? fileName = null)
        {
            string mimeType = file.ContentType;

            bool isValid = false;

            if (file == null || file.Length == 0)
            {
                return null;
            }

            long maxImageSize = 1024 * 1024 * 5;
            long maxDocumentSize = 1024 * 1024 * 20;

            if (fileType == FileTypes.Pdf || fileType == FileTypes.Excel || fileType == FileTypes.Document)
            {
                if (file.Length > maxDocumentSize)
                {
                    return null;
                }
            }
            else if (fileType == FileTypes.Image)
            {
                if (file.Length > maxImageSize)
                {
                    return null;
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
                if (fileName == null)
                {
                    fileName = file.FileName;
                }
                else
                {
                    fileName = fileName + Path.GetExtension(file.FileName);
                }

                string path = this.helperPath.MapPath(fileName, folder);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    return fileName;
                }
            }
            return null;
        }

        public async Task<List<string>> UploadFilesAsync(List<IFormFile> files, Folders folder)
        {
            List<string> paths = new();

            foreach (IFormFile file in files)
            {
                string fileName = file.FileName;
                string path = this.helperPath.MapPath(fileName, folder);
                paths.Add(path);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return paths;
        }
    }
}
