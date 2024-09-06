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

        #endregion
    }
}
