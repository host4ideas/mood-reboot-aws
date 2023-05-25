using Microsoft.EntityFrameworkCore;
using NugetMoodReboot.Models;

namespace APIMoodReboot.Data
{
    public class MoodRebootContext : DbContext
    {
        public MoodRebootContext(DbContextOptions<MoodRebootContext> options) : base(options) { }
        public DbSet<Message> Messages { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Center> Centers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ContentGroup> ContentGroups { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<AppFile> Files { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<UserChatGroup> UserChatGroups { get; set; }
        public DbSet<UserCenter> UserCenters { get; set; }
        public DbSet<UserAction> UserActions { get; set; }
    }
}
