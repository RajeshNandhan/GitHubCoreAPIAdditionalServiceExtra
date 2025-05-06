using Core.Library.ArivuTharavuThalam;
using Core.Library.ArivuTharavuThalam.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Core.API.BusinessLogics.Common
{
    public class SqlDatabaseUnitOfWork : IUnitOfWork
    {
        private readonly DbContext SqlDbContext;

        public IGenericRepository<Book> BookRepository { get; }

        public IGenericRepository<Person> PersonRepository { get; }

        public SqlDatabaseUnitOfWork(SqlDataBaseDataContext sqlDbContext,
            IGenericRepository<Book> bookRepository,
            IGenericRepository<Person> personRepository)
        {
            SqlDbContext = sqlDbContext;
            BookRepository = bookRepository;
            PersonRepository = personRepository;
        }

        public virtual Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            return SqlDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
