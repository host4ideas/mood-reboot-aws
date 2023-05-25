using Microsoft.EntityFrameworkCore;
using APIMoodReboot.Data;
using NugetMoodReboot.Models;
using NugetMoodReboot.Interfaces;

namespace APIMoodReboot.Repositories
{
    public class RepositoryCntGroupsSql : IRepositoryContentGroups
    {
        private readonly MoodRebootContext context;

        public RepositoryCntGroupsSql(MoodRebootContext context)
        {
            this.context = context;
        }

        public async Task<ContentGroup?> FindContentGroupAsync(int id)
        {
            return await this.context.ContentGroups.FindAsync(id);
        }

        public Task<List<ContentGroup>> GetCourseContentGroupsAsync(int courseId)
        {
            var consulta = from datos in this.context.ContentGroups
                           where datos.CourseID == courseId
                           select datos;
            return consulta.ToListAsync();
        }

        private async Task<int> GetMaxContentGroupAsync()
        {
            if (!await this.context.ContentGroups.AnyAsync())
            {
                return 1;
            }
            return await this.context.ContentGroups.MaxAsync(x => x.ContentGroupId) + 1;
        }

        public async Task CreateContentGroupAsync(string name, int courseId, bool isVisible = false)
        {
            await this.context.ContentGroups.AddAsync(new()
            {
                ContentGroupId = await this.GetMaxContentGroupAsync(),
                Contents = new(),
                CourseID = courseId,
                IsVisible = isVisible,
                Name = name,
            });

            await this.context.SaveChangesAsync();
        }

        public async Task UpdateContentGroupAsync(int id, string name, Boolean isVisible)
        {
            ContentGroup? contentGroup = await this.FindContentGroupAsync(id);
            if (contentGroup != null)
            {
                contentGroup.Name = name;
                contentGroup.IsVisible = isVisible;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task DeleteContentGroupAsync(int id)
        {
            ContentGroup? group = await this.FindContentGroupAsync(id);
            if (group != null)
            {
                this.context.ContentGroups.Remove(group);
                await this.context.SaveChangesAsync();
            }
        }
    }
}
