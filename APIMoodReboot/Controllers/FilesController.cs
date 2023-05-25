using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NugetMoodReboot.Interfaces;
using NugetMoodReboot.Models;

namespace APIMoodReboot.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IRepositoryUsers repositoryUsers;

        public FilesController(IRepositoryUsers repositoryUsers)
        {
            this.repositoryUsers = repositoryUsers;
        }

        [HttpGet]
        public async Task<ActionResult<int>> GetMaxFile()
        {
            return await this.repositoryUsers.GetMaxFileAsync();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateFile(UpdateFileApiModel model)
        {
            if (model.UserId.HasValue)
            {
                await this.repositoryUsers.UpdateFileAsync(model.FileId, model.FileName, model.MimeType, model.UserId.Value);
                return NoContent();
            }
            await this.repositoryUsers.UpdateFileAsync(model.FileId, model.FileName, model.MimeType);
            return NoContent();
        }

        [HttpGet("{fileId}")]
        public async Task<ActionResult<AppFile?>> FindFile(int fileId)
        {
            return await this.repositoryUsers.FindFileAsync(fileId);
        }

        [HttpPost]
        public async Task<ActionResult<int>> InsertFile(CreateFileApiModel model)
        {
            if (model.UserId.HasValue)
            {
                return await this.repositoryUsers.InsertFileAsync(model.FileName, model.MimeType, model.UserId.Value);
            }
            return await this.repositoryUsers.InsertFileAsync(model.FileName, model.MimeType);
        }

        [HttpDelete("{fileId}")]
        public async Task<ActionResult> DeleteFile(int fileId)
        {
            await this.repositoryUsers.DeleteFileAsync(fileId);
            return NoContent();
        }
    }
}
