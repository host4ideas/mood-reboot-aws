using Microsoft.AspNetCore.Mvc;
using NugetMoodReboot.Models;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using NugetMoodReboot.Interfaces;
using Newtonsoft.Json;
using System.Security.Claims;

namespace APIMoodReboot.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ContentController : ControllerBase
    {
        private readonly IRepositoryContent repositoryContent;
        private readonly IRepositoryUsers repositoryUser;
        private readonly HtmlSanitizer sanitizer;

        public ContentController(IRepositoryContent repositoryContent, IRepositoryUsers repositoryUser, HtmlSanitizer sanitizer)
        {
            this.repositoryContent = repositoryContent;
            this.repositoryUser = repositoryUser;
            this.sanitizer = sanitizer;
        }

        [HttpGet]
        public async Task<ActionResult<int>> GetMaxContent()
        {
            return await this.repositoryContent.GetMaxContentAsync();
        }

        [HttpGet("{groupId}")]
        public async Task<ActionResult<List<Content>>> GetContentByGroup(int groupId)
        {
            return await this.repositoryContent.GetContentByGroupAsync(groupId);
        }

        [HttpDelete("{contentId}")]
        public async Task<ActionResult> DeleteContent(int contentId)
        {
            await this.repositoryContent.DeleteContentAsync(contentId);

            var content = this.repositoryContent.FindContentAsync(contentId);
            if (content == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> AddContent(CreateContentModelApi createContent)
        {
            Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claim.Value;
            AppUser user = JsonConvert.DeserializeObject<AppUser>(jsonUser);

            if (createContent.File != null)
            {
                // Insert file in DB
                int fileId = await this.repositoryUser.InsertFileAsync(createContent.File.Name, createContent.File.MimeType, user.Id);
                // Update Content
                await this.repositoryContent.CreateContentFileAsync(contentGroupId: createContent.GroupId, fileId: fileId);
            }
            else if (createContent.UnsafeHtml != null)
            {
                string html = createContent.UnsafeHtml;
                string sanitized = this.sanitizer.Sanitize(html);

                await this.repositoryContent.CreateContentAsync(createContent.GroupId, sanitized);
            }

            return CreatedAtAction(null, null);
        }

        [HttpGet("{contentId}")]
        public async Task<ActionResult<Content?>> FindContent(int contentId)
        {
            return await this.repositoryContent.FindContentAsync(contentId);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateContent([FromBody] UpdateContentApiModel updateContent)
        {
            Content? content = await this.repositoryContent.FindContentAsync(updateContent.ContentId);

            Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claim.Value;
            AppUser user = JsonConvert.DeserializeObject<AppUser>(jsonUser);

            if (content == null)
            {
                return NotFound();
            }

            if (updateContent.File != null)
            {
                if (content.FileId == null)
                {
                    // Update DB
                    int fileId = await this.repositoryUser.InsertFileAsync(updateContent.File.Name, updateContent.File.MimeType, user.Id);
                    // Update Content
                    await this.repositoryContent.UpdateContentAsync(id: updateContent.ContentId, fileId: fileId);
                }
                else
                {
                    // Update DB
                    int fileId = content.FileId.Value;
                    await this.repositoryUser.UpdateFileAsync(fileId, updateContent.File.Name, updateContent.File.MimeType, user.Id);
                    // Update Content
                    await this.repositoryContent.UpdateContentAsync(id: updateContent.ContentId, fileId: fileId);
                }
            }
            else if (updateContent.UnsafeHtml != null)
            {
                string sanitized = this.sanitizer.Sanitize(updateContent.UnsafeHtml);

                await this.repositoryContent.UpdateContentAsync(id: updateContent.ContentId, text: sanitized);
            }

            return NoContent();
        }
    }
}
