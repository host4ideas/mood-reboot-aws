using Microsoft.AspNetCore.Hosting;

namespace NugetMoodReboot.Helpers
{
    public enum Folders { Assets, CenterImages, CourseImages, ProfileImages, ContentFiles, Icons, Logos }

    public class HelperPath
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public HelperPath(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        public string MapFolderPath(Folders folder)
        {
            string carpeta = "";

            if (folder == Folders.CourseImages)
            {
                carpeta = Path.Combine("uploads", "course_images");
            }
            else if (folder == Folders.CenterImages)
            {
                carpeta = Path.Combine("uploads", "center_images");
            }
            else if (folder == Folders.ProfileImages)
            {
                carpeta = Path.Combine("uploads", "profile_images");
            }
            else if (folder == Folders.ContentFiles)
            {
                carpeta = Path.Combine("uploads", "content_files");
            }
            else if (folder == Folders.Assets)
            {
                carpeta = Path.Combine("assets");
            }
            else if (folder == Folders.Icons)
            {
                carpeta = Path.Combine("assets", "icons");
            }
            else if (folder == Folders.Logos)
            {
                carpeta = Path.Combine("assets", "logos");
            }
            return carpeta;
        }

        public string MapPath(string fileName, Folders folder)
        {
            string carpeta = MapFolderPath(folder);

            string rootPath = this.hostEnvironment.WebRootPath;
            string path = Path.Combine(rootPath, carpeta, fileName);
            return path;
        }

        public string MapPublicPath(string fileName, Folders folder, string baseUrl)
        {
            string carpeta = MapFolderPath(folder);
            string path = Path.Combine(baseUrl, carpeta, fileName);
            return path;
        }
    }
}
