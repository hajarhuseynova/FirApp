using Fir.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fir.App.Context
{
    public class FirDbContext:DbContext
    {
      public DbSet<Category> Categories { get; set; }
        public FirDbContext(DbContextOptions<FirDbContext> options) : base(options)
        {

        }
    }
}
