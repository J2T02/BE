using Microsoft.EntityFrameworkCore;

namespace SWP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<Account>()
            //    .HasOne(a => a.Role)
            //    .WithMany(r => r.Accounts)
            //    .HasForeignKey(a => a.Role_ID);



        }

        
    }


}
