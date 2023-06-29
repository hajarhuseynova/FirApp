using Fir.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fir.App.Context
{
    public class FirDbContext:DbContext
    {
      public DbSet<Category> Categories { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public FirDbContext(DbContextOptions<FirDbContext> options) : base(options)
        {

        }
    }
}
