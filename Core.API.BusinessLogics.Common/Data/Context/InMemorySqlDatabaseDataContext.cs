using Core.Library.ArivuTharavuThalam;
using Microsoft.EntityFrameworkCore;

namespace Core.API.BusinessLogics.Common
{
    public class InMemorySqlDatabaseDataContext : DbContext
    {
        public InMemorySqlDatabaseDataContext(DbContextOptions<InMemorySqlDatabaseDataContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<Person> Persons { get; set; }
    }
}
