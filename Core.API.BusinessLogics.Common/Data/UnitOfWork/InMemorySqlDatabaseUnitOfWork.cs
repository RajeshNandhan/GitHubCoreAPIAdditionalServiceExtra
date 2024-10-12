using Core.Library.ArivuTharavuThalam;
using Core.Library.ArivuTharavuThalam.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Core.API.BusinessLogics.Common
{
    public class InMemorySqlDatabaseUnitOfWork : IUnitOfWork
    {
        private readonly DbContext inMemoryContext;

        public IGenericRepository<Book> BookRepository { get; }

        public IGenericRepository<Person> PersonRepository { get; }

        public InMemorySqlDatabaseUnitOfWork(InMemorySqlDatabaseDataContext inMemoryContext, IGenericRepository<Book> bookRepository, IGenericRepository<Person> personRepository)
        {
            this.inMemoryContext = inMemoryContext;
            this.BookRepository = bookRepository;
            this.PersonRepository = personRepository;
        }

        public virtual Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            return inMemoryContext.SaveChangesAsync(cancellationToken);
        }
    }
}
