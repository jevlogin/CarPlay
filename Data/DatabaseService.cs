using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace WORLDGAMDEVELOPMENT
{
    internal sealed class DatabaseService
    {
        #region Fields

        private readonly AppConfig _appConfig;
        private readonly ApplicationDbContext _dbContext;

        #endregion


        #region ClassLifeCycles

        public DatabaseService(AppConfig appConfig, ApplicationDbContext dbContext)
        {
            _appConfig = appConfig;
            _dbContext = dbContext;
        }

        #endregion


        #region Methods

        internal async Task<Dictionary<long, AppUser>> LoadUserListAsync()
        {
            try
            {
                var users = await _dbContext.Users.ToDictionaryAsync(user => user.Id, user => user);
                return users ?? [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error DataBase LoadUser: {ex}");
                throw;
            }
        }

        internal async Task MigrateAsync()
        {
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Any())
                {
                    await _dbContext.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error DataBase Migrate: {ex}");
                throw;
            }
            await AddFirstAdminAsync();
        }

        private async Task AddFirstAdminAsync()
        {
            var firstAdmin = await _dbContext.Users.FindAsync(_appConfig.FirstAdmin);
            if (firstAdmin == null)
            {
                var newAdmin = new AppUser
                {
                    Id = _appConfig.FirstAdmin,
                    IsAdmin = true,
                    FirstName = "Administrator"
                };
                _dbContext.Users.Add(newAdmin);
                Console.WriteLine($"Пользователь с Id {newAdmin.Id} был добавлен как Администратор.");
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                if (!firstAdmin.IsAdmin)
                {
                    firstAdmin.IsAdmin = true;
                    _dbContext.Entry(firstAdmin).CurrentValues.SetValues(firstAdmin); 
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        internal async Task AddUserAsync(AppUser user)
        {
            var userInServer = await _dbContext.Users.FindAsync(user.Id);
            if (userInServer == null)
            {
                await _dbContext.Users.AddAsync(user);
            }
            else
            {
                _dbContext.Entry(userInServer).CurrentValues.SetValues(user);
            }

            await _dbContext.SaveChangesAsync();
        }

        internal async Task<Dictionary<long, AppUser>> LoadAdminListAsync()
        {
            try
            {
                var tempAdmin = await _dbContext.Users
                    .Where(admin => admin.IsAdmin)
                    .ToDictionaryAsync(admin => admin.Id, admin => admin);
                return tempAdmin ?? [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error DataBase LoadAdmin: {ex}");
                throw;
            }
        }

        #endregion
    }
}
