using Microsoft.EntityFrameworkCore;
using TechLibrary.Data.Entities;

namespace TechLibrary.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            // NoOp
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<ErrorStore> ErrorStore { get; set; }
    }
}
