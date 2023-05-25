using Microsoft.EntityFrameworkCore;
using APIMoodReboot.Data;
using NugetMoodReboot.Models;
using NugetMoodReboot.Interfaces;

namespace APIMoodReboot.Repositories
{
    public class RepositoryContentSql : IRepositoryContent
    {
        private readonly MoodRebootContext context;

        public RepositoryContentSql(MoodRebootContext context)
        {
            this.context = context;
        }

        public async Task<int> GetMaxContentAsync()
        {
            if (!context.Contents.Any())
            {
                return 1;
            }

            return await this.context.Contents.MaxAsync(x => x.Id) + 1;
        }

        public async Task<Content?> FindContentAsync(int id)
        {
            return await this.context.Contents.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<Content>> GetContentByGroupAsync(int groupId)
        {
            return this.context.Contents.Where(x => x.ContentGroupId == groupId).ToListAsync();
        }

        public async Task CreateContentAsync(int contentGroupId, string text)
        {
            Content content = new()
            {
                Id = await this.GetMaxContentAsync(),
                Text = text,
                ContentGroupId = contentGroupId,
            };

            await this.context.Contents.AddAsync(content);
            await this.context.SaveChangesAsync();
        }

        public async Task CreateContentFileAsync(int contentGroupId, int fileId)
        {
            Content content = new()
            {
                Id = await this.GetMaxContentAsync(),
                ContentGroupId = contentGroupId,
                FileId = fileId
            };

            await this.context.Contents.AddAsync(content);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteContentAsync(int id)
        {
            Content? content = await this.FindContentAsync(id);
            if (content != null)
            {
                this.context.Contents.Remove(content);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task UpdateContentAsync(int id, string? text = null, int? fileId = null)
        {
            Content? oldContent = await this.FindContentAsync(id);
            if (oldContent != null)
            {
                oldContent.Text = text;
                oldContent.FileId = fileId;
                await this.context.SaveChangesAsync();
            }
        }
    }
}
