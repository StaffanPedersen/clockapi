using Microsoft.EntityFrameworkCore;

namespace ClockApi.Contexts
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }

        public DbSet<Models.User> Users { get; set; }
    }
}