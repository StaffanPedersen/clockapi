namespace ClockApi.Contexts;

using Microsoft.EntityFrameworkCore;
using Models;


public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<User> Users { get; set; } 
}
