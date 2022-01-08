using GeekBurger.Users.Model;
using Microsoft.EntityFrameworkCore;

namespace GeekBurger.Users.Repository
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
