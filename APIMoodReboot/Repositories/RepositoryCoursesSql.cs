using Microsoft.EntityFrameworkCore;
using APIMoodReboot.Data;
using NugetMoodReboot.Models;
using NugetMoodReboot.Interfaces;

namespace APIMoodReboot.Repositories
{
    public class RepositoryCoursesSql : IRepositoryCourses
    {
        private readonly MoodRebootContext context;

        public RepositoryCoursesSql(MoodRebootContext context)
        {
            this.context = context;
        }

        public Task<List<CourseUsersModel>> GetCourseUsersAsync(int courseId)
        {
            var result = from uc in this.context.UserCourses
                         join u in this.context.Users on uc.UserId equals u.Id
                         where uc.CourseId == courseId
                         select new CourseUsersModel
                         {
                             Id = u.Id,
                             UserName = u.UserName,
                             Image = u.Image,
                             IsEditor = uc.IsEditor
                         };

            return result.ToListAsync();
        }

        public Task<UserCourse?> FindUserCourseAsync(int userId, int courseId)
        {
            return this.context.UserCourses.FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);
        }

        public async Task RemoveCourseUserAsync(int courseId, int userId)
        {
            UserCourse? userCourse = await this.FindUserCourseAsync(userId, courseId);

            if (userCourse != null)
            {
                this.context.UserCourses.Remove(userCourse);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task RemoveCourseEditorAsync(int courseId, int userId)
        {
            UserCourse? userCourse = await this.context.UserCourses.FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);

            if (userCourse != null)
            {
                userCourse.IsEditor = false;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task<int> GetMaxUserCourseAsync()
        {
            if (!context.UserCourses.Any())
            {
                return 1;
            }

            return await this.context.UserCourses.MaxAsync(z => z.Id) + 1;
        }

        public async Task AddCourseEditorAsync(int courseId, int userId)
        {
            UserCourse? userCourse = await this.context.UserCourses.FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);

            if (userCourse != null)
            {
                userCourse.IsEditor = true;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task AddUserToCourseLogicAsync(Course course, int userId, bool isEditor)
        {
            // Add user to the course
            UserCourse userCourse = new()
            {
                Id = await this.GetMaxUserCourseAsync(),
                CourseId = course.Id,
                IsEditor = isEditor,
                UserId = userId,
            };
            await this.context.UserCourses.AddAsync(userCourse);

            // Add user to the center if it's not already in
            UserCenter? userCenter = await this.context.UserCenters.FirstOrDefaultAsync(x => x.CenterId == course.CenterId && x.UserId == userId);
            // If the user isn't in the center
            if (userCenter == null)
            {
                UserCenter newUserCenter = new()
                {
                    Id = await this.context.UserCenters.MaxAsync(x => x.Id) + 1,
                    CenterId = course.CenterId,
                    IsEditor = isEditor,
                    UserId = userId,
                };

                await this.context.UserCenters.AddAsync(newUserCenter);
            }

            // Add user to the course's discussion chat group if the group exist
            if (course.GroupId.HasValue)
            {
                // In case is the first group to be created
                int newId = 1;
                if (this.context.ChatGroups.Any())
                {
                    newId = await this.context.UserChatGroups.MaxAsync(x => x.Id) + 1;
                }

                await this.context.UserChatGroups.AddAsync(new UserChatGroup()
                {
                    Id = newId,
                    GroupId = course.GroupId.Value,
                    JoinDate = DateTime.Now,
                    LastSeen = DateTime.Now,
                    UserID = userId,
                    IsAdmin = isEditor
                });
            }
            await this.context.SaveChangesAsync();
        }

        public async Task<bool> AddCourseUserAsync(int courseId, int userId, bool isEditor)
        {
            Course? course = await this.FindCourseAsync(courseId);

            if (course != null)
            {
                await this.AddUserToCourseLogicAsync(course, userId, isEditor);
                return true;
            }

            return false;
        }

        public async Task<bool> AddCourseUserAsync(int courseId, int userId, bool isEditor, string password)
        {
            Course? course = await this.FindCourseAsync(courseId);

            if (course != null)
            {
                if (course.Password == password)
                {
                    await this.AddUserToCourseLogicAsync(course, userId, isEditor);
                    return true;
                }
            }

            return false;
        }

        public async Task<int> GetMaxCourseAsync()
        {
            return await this.context.Courses.MaxAsync(x => x.Id) + 1;
        }

        public async Task DeleteCourseAsync(int id)
        {
            Course? course = await this.FindCourseAsync(id);
            if (course != null)
            {
                this.context.Courses.Remove(course);
                await this.context.SaveChangesAsync();
            }
        }

        public Task<Course?> FindCourseAsync(int id)
        {
            return this.context.Courses.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<Course>> GetAllCoursesAsync()
        {
            return this.context.Courses.ToListAsync();
        }

        /// <summary>
        /// Adds the authors to a given list of courses
        /// </summary>
        /// <param name="courseListView"></param>
        /// <returns></returns>
        public async Task<List<CourseListView>> GetCoursesAuthorsAsync(List<CourseListView> courseListView)
        {
            List<int> courseIds = new();

            foreach (CourseListView course in courseListView)
            {
                courseIds.Add(course.CourseId);
            }

            var result2 = from u in context.Users
                          join uc in context.UserCourses on u.Id equals uc.UserId
                          where courseIds.Contains(uc.CourseId) && uc.IsEditor == true
                          select new { u.UserName, u.Image, u.Id, uc.CourseId };

            var possibleAuthors = await result2.ToListAsync();

            // Filter the authors for each course
            foreach (CourseListView course in courseListView)
            {
                var courseAuthors = possibleAuthors.Where(p => p.CourseId == course.CourseId);

                if (courseAuthors.Any())
                {
                    // Convert all anonynous objects of the result to Author Model
                    List<Author> authors = courseAuthors.ToList().ConvertAll(x => new Author() { Id = x.Id, Image = x.Image, UserName = x.UserName });
                    course.Authors = authors;
                }
            }

            return courseListView;
        }

        public async Task<List<CourseListView>> GetEditorCenterCoursesAsync(int userId, int centerId)
        {
            var result = from c in context.Courses
                         join uc in context.UserCourses on c.Id equals uc.CourseId
                         join u in context.Users on uc.UserId equals u.Id
                         join ct in context.Centers on c.CenterId equals ct.Id
                         where uc.UserId == userId && c.CenterId == centerId && uc.IsEditor == true
                         select new CourseListView
                         {
                             CourseId = c.Id,
                             DatePublished = c.DatePublished,
                             DateModified = c.DateModified,
                             Description = c.Description,
                             Image = c.Image,
                             CourseName = c.Name,
                             CenterName = ct.Name,
                             IsEditor = uc.IsEditor,
                             IsVisible = c.IsVisible,
                         };

            // Courses without authors
            List<CourseListView> courseListView = await result.ToListAsync();

            // Courses with authors
            List<CourseListView> coursesAuthors = await this.GetCoursesAuthorsAsync(courseListView);

            return coursesAuthors;
        }

        /// <summary>
        /// Get a user's courses
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<CourseListView>> GetUserCoursesAsync(int userId)
        {
            var result = from c in context.Courses
                         join uc in context.UserCourses on c.Id equals uc.CourseId
                         join u in context.Users on uc.UserId equals u.Id
                         join ct in context.Centers on c.CenterId equals ct.Id
                         where uc.UserId == userId
                         select new CourseListView
                         {
                             CourseId = c.Id,
                             DatePublished = c.DatePublished,
                             DateModified = c.DateModified,
                             Description = c.Description,
                             Image = c.Image,
                             CourseName = c.Name,
                             CenterName = ct.Name,
                             IsEditor = uc.IsEditor
                         };

            // Courses without authors
            List<CourseListView> courseListView = await result.ToListAsync();

            // Courses with authors
            List<CourseListView> coursesAuthors = await this.GetCoursesAuthorsAsync(courseListView);

            return coursesAuthors;
        }

        public async Task<List<CourseListView>> CenterCoursesListViewAsync(int centerId)
        {
            // Courses without authors
            List<CourseListView> courseListView = await this.GetCenterCoursesAsync(centerId);

            // Courses with authors
            List<CourseListView> coursesAuthors = await this.GetCoursesAuthorsAsync(courseListView);

            return coursesAuthors;
        }

        /// <summary>
        /// Get a center's courses
        /// </summary>
        /// <param name="centerId"></param>
        /// <returns></returns>
        public Task<List<CourseListView>> GetCenterCoursesAsync(int centerId)
        {
            var result = from cr in this.context.Courses
                         join ct in this.context.Centers on cr.CenterId equals ct.Id
                         where ct.Id == centerId
                         select new CourseListView
                         {
                             CourseId = cr.Id,
                             CourseName = cr.Name,
                             DatePublished = cr.DatePublished,
                             DateModified = cr.DateModified,
                             Description = cr.Description,
                             Image = cr.Image,
                             CenterName = ct.Name,
                             Authors = new List<Author>()
                         };

            return result.ToListAsync();
        }

        public async Task UpdateCourseVisibilityAsync(int courseId)
        {
            Course? course = await this.FindCourseAsync(courseId);
            if (course != null)
            {
                if (course.IsVisible == false)
                {
                    course.IsVisible = true;
                }
                else
                {
                    course.IsVisible = false;
                }
            }
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateCourseAsync(int id, string description, string image, string name, bool isVisible)
        {
            Course? course = await this.FindCourseAsync(id);

            if (course != null)
            {
                course.Description = description;
                course.IsVisible = isVisible;
                course.Image = image;
                course.Name = name;
                course.DateModified = DateTime.Now;
                await this.context.SaveChangesAsync();
            }
        }
    }
}
