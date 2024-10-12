using Core.Library.ArivuTharavuThalam;
using Microsoft.EntityFrameworkCore;
using System;

namespace Core.API.BusinessLogics.Common
{
    public class SqlDataBaseDataContext : DbContext
    {
        public SqlDataBaseDataContext(DbContextOptions<SqlDataBaseDataContext> options)
            : base(options)
        {
            //THIS LINE OF Code only to FIX Date time issue with postgreSQL
            //https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<Person> Persons { get; set; }
    }
}
