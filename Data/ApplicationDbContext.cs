using Microsoft.EntityFrameworkCore;


namespace WORLDGAMDEVELOPMENT
{
    internal sealed class ApplicationDbContext : DbContext
    {
        #region Properties
        
        public DbSet<AppUser> Users { get; set; }

        #endregion


        #region ClassLifeCycles
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #endregion
    }
}