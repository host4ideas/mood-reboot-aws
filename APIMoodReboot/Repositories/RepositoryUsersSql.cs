using Microsoft.EntityFrameworkCore;
using APIMoodReboot.Data;
using NugetMoodReboot.Models;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Interfaces;

namespace APIMoodReboot.Repositories
{
    public class RepositoryUsersSql : IRepositoryUsers
    {
        private readonly MoodRebootContext context;

        public RepositoryUsersSql(MoodRebootContext context)
        {
            this.context = context;
        }

        #region USERS

        private async Task<int> GetMaxUserActionAsync()
        {
            if (!context.UserActions.Any())
            {
                return 1;
            }

            return await this.context.UserActions.MaxAsync(x => x.Id) + 1;
        }

        public async Task<string> CreateUserActionAsync(int userId)
        {
            UserAction userAction = new()
            {
                Id = await this.GetMaxUserActionAsync(),
                Token = Guid.NewGuid().ToString(),
                UserId = userId,
                RequestDate = DateTime.Now
            };

            await this.context.UserActions.AddAsync(userAction);
            await this.context.SaveChangesAsync();

            return userAction.Token;
        }

        public Task<UserAction?> FindUserActionAsync(int userId, string token)
        {
            return this.context.UserActions.FirstOrDefaultAsync(x => x.UserId == userId && x.Token == token);
        }

        public async Task RemoveUserActionAsync(UserAction userAction)
        {
            this.context.UserActions.Remove(userAction);
            await this.context.SaveChangesAsync();
        }

        public async Task ApproveUserAsync(AppUser user)
        {
            user.Approved = true;
            await this.context.SaveChangesAsync();
        }

        public async Task ApproveUserAsync(int userId)
        {
            AppUser? user = await this.FindUserAsync(userId);
            if (user != null)
            {
                user.Approved = true;
                await this.context.SaveChangesAsync();
            }
        }

        public Task<List<AppUser>> GetPendingUsersAsync()
        {
            return this.context.Users.Where(x => x.Approved == false).ToListAsync();
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            int count = await this.context.Users.CountAsync(u => u.Email == email);
            if (count > 0)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> IsUsernameAvailableAsync(string userName)
        {
            int count = await this.context.Users.CountAsync(u => u.UserName == userName);
            if (count > 0)
            {
                return false;
            }
            return true;
        }

        public Task<List<Tuple<string, int>>> SearchUsersAsync(string pattern)
        {
            var result = (from u in this.context.Users
                          where u.UserName.Contains(pattern) || u.Email.Contains(pattern)
                          select new Tuple<string, int>(u.UserName, u.Id)).Take(10);
            return result.ToListAsync();
        }

        public async Task<int> GetMaxUserAsync()
        {
            if (!context.Users.Any())
            {
                return 1;
            }

            return await this.context.Users.MaxAsync(z => z.Id) + 1;
        }

        public async Task<AppUser?> FindUserAsync(int userId)
        {
            return await this.context.Users.FindAsync(userId);
        }

        public Task<List<AppUser>> GetAllUsersAsync()
        {
            return this.context.Users.ToListAsync();
        }

        public async Task<int> RegisterUserAsync(string nombre, string firstName, string lastName, string email, string password, string image)
        {
            string salt = HelperCryptography.GenerateSalt();

            int userId = await this.GetMaxUserAsync();

            AppUser user = new()
            {
                Id = userId,
                UserName = nombre,
                LastName = lastName,
                FirstName = firstName,
                SignedDate = DateTime.UtcNow,
                Role = "USER",
                Email = email,
                Image = image,
                Salt = salt,
                Password = HelperCryptography.EncryptPassword(password, salt),
                Approved = false,
                PassTest = password
            };

            await this.context.Users.AddAsync(user);
            await this.context.SaveChangesAsync();

            return userId;
        }

        public async Task<AppUser?> LoginUserAsync(string usernameOrEmail, string password)
        {
            AppUser? user = await this.context.Users.FirstOrDefaultAsync(u => u.Email == usernameOrEmail || u.UserName == usernameOrEmail);
            if (user != null)
            {
                if (password == user.PassTest)
                {
                    user.LastSeen = DateTime.Now;
                    await this.context.SaveChangesAsync();
                    return user;
                }

                // Recuperamos la password cifrada de la BBDD
                //byte[] userPass = user.Password;
                //string salt = user.Salt;
                //byte[] temp = HelperCryptography.EncryptPassword(password, salt);
                //if (HelperCryptography.CompareArrays(userPass, temp))
                //{
                //    user.LastSeen = DateTime.Now;
                //    await this.context.SaveChangesAsync();
                //    return user;
                //}
            }
            return default;
        }

        public async Task DeleteUserAsync(int userId)
        {
            AppUser? user = await this.FindUserAsync(userId);
            if (user != null)
            {
                this.context.Users.Remove(user);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task UpdateUserBasicsAsync(int userId, string userName, string firstName, string lastName, string? image = null)
        {
            AppUser? user = await this.FindUserAsync(userId);
            if (user != null)
            {
                user.UserName = userName;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.Image = image;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task UpdateUserEmailAsync(int userId, string email)
        {
            AppUser? user = await this.FindUserAsync(userId);
            if (user != null)
            {
                user.Email = email;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task UpdateUserPasswordAsync(int userId, string password)
        {
            string salt = HelperCryptography.GenerateSalt();

            AppUser? user = await this.FindUserAsync(userId);

            if (user != null)
            {
                user.Password = HelperCryptography.EncryptPassword(password, salt);
                user.Salt = salt;
                user.PassTest = password;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task DeactivateUserAsync(int userId)
        {
            AppUser? user = await this.FindUserAsync(userId);
            if (user != null)
            {
                user.Approved = false;
                await this.context.SaveChangesAsync();
            }
        }

        #endregion

        #region FILES

        public Task<AppFile?> FindFileAsync(int fileId)
        {
            return this.context.Files.FirstOrDefaultAsync(x => x.Id == fileId);
        }

        public async Task<int> GetMaxFileAsync()
        {
            if (!context.Files.Any())
            {
                return 1;
            }
            return await this.context.Files.MaxAsync(x => x.Id) + 1;
        }

        /// <summary>
        /// Inserts a new File in the File table of the database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mimeType"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> InsertFileAsync(string name, string mimeType)
        {
            int fileId = await this.GetMaxFileAsync();

            await this.context.Files.AddAsync(new()
            {
                Id = fileId,
                Name = name,
                MimeType = mimeType
            });

            await this.context.SaveChangesAsync();

            return fileId;
        }

        /// <summary>
        /// Inserts a new File in the File table of the database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mimeType"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> InsertFileAsync(string name, string mimeType, int userId)
        {
            int fileId = await this.GetMaxFileAsync();

            AppFile file = new()
            {
                Id = fileId,
                Name = name,
                MimeType = mimeType,
                UserId = userId
            };

            await this.context.Files.AddAsync(file);
            await this.context.SaveChangesAsync();
            return fileId;
        }

        public async Task UpdateFileAsync(int fileId, string fileName, string mimeType)
        {
            AppFile? appFile = await this.FindFileAsync(fileId);

            if (appFile != null)
            {
                appFile.MimeType = mimeType;
                appFile.Name = fileName;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task UpdateFileAsync(int fileId, string fileName, string mimeType, int userId)
        {
            AppFile? appFile = await this.FindFileAsync(fileId);

            if (appFile != null)
            {
                appFile.MimeType = mimeType;
                appFile.Name = fileName;
                appFile.UserId = userId;
                await this.context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes a file from the File table of the database
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task DeleteFileAsync(int fileId)
        {
            AppFile? file = await this.context.Files.FirstOrDefaultAsync(x => x.Id == fileId);

            if (file != null)
            {
                this.context.Files.Remove(file);
                await this.context.SaveChangesAsync();
            }
        }

        #endregion

        #region MESSAGES

        public async Task AddUsersToChatAsync(int chatGroupId, List<int> userIds)
        {
            // Create the chat group
            ChatGroup? chatGroup = await this.context.ChatGroups.FirstOrDefaultAsync(x => x.Id == chatGroupId);

            if (chatGroup != null)
            {
                List<ChatUserModel> users = await this.GetChatGroupUsersAsync(chatGroupId);
                List<int> alreadyUserIds = users.ConvertAll(x => x.UserID).ToList();
                List<int> noDupsNewUsers = userIds.Distinct().ToList();

                // Delete already users
                foreach (var alreadyUser in alreadyUserIds)
                {
                    foreach (var newUser in noDupsNewUsers)
                    {
                        if (newUser == alreadyUser)
                        {
                            noDupsNewUsers.Remove(alreadyUser);
                        }
                    }
                }

                int firstId = await this.GetMaxUserChatGroupAsync();

                // Add users to the chat group
                foreach (int userId in noDupsNewUsers)
                {
                    UserChatGroup userChatGroup = new()
                    {
                        Id = firstId,
                        GroupId = chatGroup.Id,
                        JoinDate = DateTime.Now,
                        LastSeen = DateTime.Now,
                        UserID = userId,
                        IsAdmin = false
                    };

                    await this.context.UserChatGroups.AddAsync(userChatGroup);
                    firstId += 1;
                }

                await this.context.SaveChangesAsync();
            }
        }

        public async Task NewChatGroupAsync(HashSet<int> userIdsNoDups)
        {

            // Create the chat group
            ChatGroup chatGroup = new()
            {
                Id = await this.GetMaxChatGroupAsync(),
                Name = "PRIVATE",
            };

            await this.context.ChatGroups.AddAsync(chatGroup);

            int firstId = await this.GetMaxUserChatGroupAsync();

            // Add users to the chat group
            foreach (int userId in userIdsNoDups)
            {
                UserChatGroup userChatGroup = new()
                {
                    Id = firstId,
                    GroupId = chatGroup.Id,
                    JoinDate = DateTime.Now,
                    LastSeen = DateTime.Now,
                    UserID = userId,
                    IsAdmin = false
                };

                await this.context.UserChatGroups.AddAsync(userChatGroup);
                firstId += 1;
            }

            await this.context.SaveChangesAsync();
        }

        public async Task<int> NewChatGroupAsync(HashSet<int> userIdsNoDups, int adminUserId, string chatGroupName)
        {
            int newId = await this.GetMaxChatGroupAsync();

            // Create the chat group
            ChatGroup chatGroup = new()
            {
                Id = newId,
                Name = chatGroupName,
            };

            await this.context.ChatGroups.AddAsync(chatGroup);

            int firstId = await this.GetMaxUserChatGroupAsync();

            // Add users to the chat group
            foreach (int userId in userIdsNoDups)
            {
                bool isAdmin = false;

                if (adminUserId == userId)
                {
                    isAdmin = true;
                }

                UserChatGroup userChatGroup = new()
                {
                    Id = firstId,
                    GroupId = chatGroup.Id,
                    JoinDate = DateTime.Now,
                    LastSeen = DateTime.Now,
                    UserID = userId,
                    IsAdmin = isAdmin
                };

                await this.context.UserChatGroups.AddAsync(userChatGroup);
                firstId++;
            }

            await this.context.SaveChangesAsync();
            return newId;
        }

        public async Task UpdateChatGroupAsync(int chatGroupId, string name)
        {
            ChatGroup? chatGroup = await this.context.ChatGroups.FirstOrDefaultAsync(x => x.Id == chatGroupId);
            if (chatGroup != null)
            {
                chatGroup.Name = name;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task RemoveChatGroupAsync(int chatGroupId)
        {
            ChatGroup? chatGroup = await this.context.ChatGroups.FirstOrDefaultAsync(x => x.Id == chatGroupId);
            if (chatGroup != null)
            {
                this.context.ChatGroups.Remove(chatGroup);
                await this.context.SaveChangesAsync();
            }
        }

        private async Task<int> GetMaxMessageAsync()
        {
            if (!this.context.Messages.Any())
            {
                return 1;
            }

            int max = await this.context.Messages.MaxAsync(x => x.MessageId) + 1;
            return max;
        }

        public async Task CreateMessageAsync(int userId, int groupChatId, string userName, string? text = null, int? fileId = null)
        {
            Message message = new()
            {
                MessageId = await this.GetMaxMessageAsync(),
                UserID = userId,
                GroupId = groupChatId,
                Text = text,
                FileId = fileId,
                DatePosted = DateTime.Now,
                UserName = userName
            };

            await this.context.Messages.AddAsync(message);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteMessageAsync(int messageId)
        {
            Message? message = await this.context.Messages.FirstOrDefaultAsync(x => x.MessageId == messageId);

            if (message != null)
            {
                this.context.Messages.Remove(message);
                await this.context.SaveChangesAsync();
            }
        }

        private async Task<int> GetMaxChatGroupAsync()
        {
            if (!this.context.ChatGroups.Any())
            {
                return 1;
            }

            int max = await this.context.ChatGroups.MaxAsync(x => x.Id);
            return max + 1;
        }

        public async Task CreateChatAsync(string name, string? image)
        {
            ChatGroup chatGroup = new()
            {
                Id = await this.GetMaxChatGroupAsync(),
                Name = name,
                Image = image
            };

            this.context.ChatGroups.Add(chatGroup);
            await this.context.SaveChangesAsync();
        }

        private async Task<int> GetMaxUserChatGroupAsync()
        {
            if (!this.context.UserChatGroups.Any())
            {
                return 1;
            }

            int max = await this.context.UserChatGroups.MaxAsync(x => x.Id);
            return max + 1;
        }

        public async Task AddUsersToChatAsync(HashSet<int> userIds, int chatGroupId)
        {
            List<UserChatGroup> userChatGroups = new();

            HashSet<int> usersNoDups = userIds;

            int firstIndex = await this.GetMaxUserChatGroupAsync();

            foreach (int userId in usersNoDups)
            {
                userChatGroups.Add(new()
                {
                    Id = firstIndex,
                    UserID = userId,
                    GroupId = chatGroupId,
                    JoinDate = DateTime.Now
                });
                firstIndex++;
            }

            await this.context.UserChatGroups.AddRangeAsync(userChatGroups);
            await this.context.SaveChangesAsync();
        }

        public Task<List<Message>> GetMessagesByGroupAsync(int chatGroupId)
        {
            return this.context.Messages.Where(x => x.GroupId == chatGroupId).ToListAsync();
        }

        public Task<List<ChatGroup>> GetUserChatGroupsAsync(int userId)
        {
            var result = from g in context.ChatGroups
                         join ug in context.UserChatGroups on g.Id equals ug.GroupId
                         where ug.UserID == userId
                         select g;

            return result.ToListAsync();
        }

        public Task<List<Message>> GetUnseenMessagesAsync(int userId)
        {
            var result = from ug in context.UserChatGroups
                         join m in context.Messages on ug.GroupId equals m.GroupId
                         where ug.UserID == userId && ug.LastSeen < m.DatePosted
                         select m;

            return result.ToListAsync();
        }

        public async Task UpdateChatLastSeenAsync(int chatGroupId, int userId)
        {
            UserChatGroup? userChatGroup = await this.context.UserChatGroups.FirstOrDefaultAsync(x => x.UserID == userId && x.GroupId == chatGroupId);
            if (userChatGroup != null)
            {
                userChatGroup.LastSeen = DateTime.Now;
                await this.context.SaveChangesAsync();
            }
        }

        public Task<List<ChatUserModel>> GetChatGroupUsersAsync(int chatGroupId)
        {
            var result = from u in this.context.Users
                         join ug in this.context.UserChatGroups on u.Id equals ug.UserID
                         where ug.GroupId == chatGroupId
                         select new ChatUserModel()
                         {
                             IsAdmin = ug.IsAdmin,
                             UserID = u.Id,
                             UserName = u.UserName
                         };
            return result.ToListAsync();
        }

        public async Task RemoveChatUserAsync(int userId, int chatGroupId)
        {
            UserChatGroup? userChat = await this.context.UserChatGroups.FirstOrDefaultAsync(x => x.GroupId == chatGroupId && x.UserID == userId);

            if (userChat != null)
            {
                this.context.UserChatGroups.Remove(userChat);
                await this.context.SaveChangesAsync();

                //List<ChatUserModel> users = await this.GetChatGroupUsers(chatGroupId);
                //// If there aren't any remaining users in the chat delete it
                //if (users.Count == 0)
                //{
                //    await this.RemoveChatGroup(chatGroupId);
                //    await this.context.SaveChangesAsync();
                //}
            }
        }

        #endregion
    }
}
