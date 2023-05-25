using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using MoodReboot.Helpers;
using MoodReboot.Services;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Models;

namespace MoodReboot.Controllers
{
    public class ContentController : Controller
    {
        private readonly ServiceApiContents serviceContents;
        private readonly ServiceApiUsers serviceUsers;
        private readonly HtmlSanitizer sanitizer;
        private readonly HelperFileAzure helperFile;

        public ContentController(ServiceApiContents serviceContents, ServiceApiUsers serviceUsers, HtmlSanitizer sanitizer, HelperFileAzure helperFile)
        {
            this.serviceContents = serviceContents;
            this.serviceUsers = serviceUsers;
            this.sanitizer = sanitizer;
            this.helperFile = helperFile;
        }

        public async Task<IActionResult> DeleteContent(int contentId, int courseId)
        {
            Content? content = await this.serviceContents.FindContentAsync(contentId);

            if (content != null)
            {
                await this.serviceContents.DeleteContentAsync(contentId);

                if (content.FileId.HasValue)
                {
                    AppFile? file = await this.serviceUsers.FindFileAsync(content.FileId.Value);
                    await this.helperFile.DeleteFileAsync(Containers.PrivateContent, file!.Name);
                    await this.serviceUsers.DeleteFileAsync(file!.Id);
                }
            }

            return RedirectToAction("CourseDetails", "Courses", new { id = courseId });
        }

        [HttpPost]
        public async Task<IActionResult> AddContent(int userId, int courseId, int groupId, string unsafeHtml, IFormFile hiddenFileInput)
        {
            if (hiddenFileInput != null)
            {
                string mimeType = hiddenFileInput.ContentType;

                string fileName = "content_file_" + await this.serviceUsers.GetMaxFileAsync();
                // Upload file
                // Try with a document
                bool isUploaded = await this.helperFile.UploadFileAsync(hiddenFileInput, Containers.PrivateContent, FileTypes.Document, fileName);

                if (isUploaded == false)
                {
                    // Try with an image
                    isUploaded = await this.helperFile.UploadFileAsync(hiddenFileInput, Containers.PrivateContent, FileTypes.Image, fileName);

                    if (isUploaded == false)
                    {
                        ViewData["ERROR"] = "Error al subir archivo. Formatos soportados: .pdf, .xlsx, .jpeg, .jpg, .png, .webp. Tamaño máximo: 10MB.";
                        return RedirectToAction("CourseDetails", "Courses", new { id = courseId });

                    }
                }
                if (isUploaded == true)
                {
                    // Insert file in DB
                    CreateFileApiModel model = new()
                    {
                        FileName = fileName,
                        MimeType = mimeType,
                        UserId = userId,
                    };

                    int fileId = await this.serviceUsers.InsertFileAsync(model);
                    // Update Content
                    await this.serviceContents.CreateContentFileAsync(contentGroupId: groupId, fileId: fileId);
                }
            }
            else if (unsafeHtml != null)
            {
                string html = unsafeHtml;
                string sanitized = this.sanitizer.Sanitize(html);

                await this.serviceContents.CreateContentAsync(groupId, sanitized);
            }

            return RedirectToAction("CourseDetails", "Courses", new { id = courseId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateContent(int userId, int courseId, int contentId, string unsafeHtml, IFormFile hiddenFileInput)
        {
            Content? content = await this.serviceContents.FindContentAsync(contentId);

            if (content != null)
            {
                if (hiddenFileInput != null)
                {
                    string mimeType = hiddenFileInput.ContentType;
                    string fileName = "content_file_" + contentId;
                    // Upload file                
                    // Try with a document
                    bool isUploaded = await this.helperFile.UpdateFileAsync(hiddenFileInput, Containers.PrivateContent, FileTypes.Document, fileName);

                    if (isUploaded == false)
                    {
                        // Try with an image
                        isUploaded = await this.helperFile.UpdateFileAsync(hiddenFileInput, Containers.PrivateContent, FileTypes.Image, fileName);

                        if (isUploaded == false)
                        {
                            ViewData["ERROR"] = "Error al subir archivo. Formatos soportados: .pdf, .xlsx, .jpeg, .jpg, .png, .webp. Tamaño máximo: 10MB.";
                            return RedirectToAction("CourseDetails", "Courses", new { id = courseId });

                        }
                    }

                    if (isUploaded == true)
                    {
                        if (content.FileId == null)
                        {
                            // Update DB
                            CreateFileApiModel model = new()
                            {
                                FileName = fileName,
                                MimeType = mimeType,
                                UserId = userId,
                            };

                            int fileId = await this.serviceUsers.InsertFileAsync(model);
                            // Update Content
                            await this.serviceContents.UpdateContentAsync(id: contentId, fileId: fileId);
                        }
                        else
                        {
                            // Update DB
                            int fileId = content.FileId.Value;

                            UpdateFileApiModel model = new()
                            {
                                FileId = fileId,
                                FileName = fileName,
                                MimeType = mimeType,
                                UserId = userId,
                            };

                            await this.serviceUsers.UpdateFileAsync(model);
                            // Update Content
                            await this.serviceContents.UpdateContentAsync(id: contentId, fileId: fileId);
                        }
                    }
                }
                else if (unsafeHtml != null)
                {
                    string sanitized = this.sanitizer.Sanitize(unsafeHtml);

                    await this.serviceContents.UpdateContentAsync(id: contentId, text: sanitized);
                }
            }

            return RedirectToAction("CourseDetails", "Courses", new { id = courseId });
        }
    }
}
