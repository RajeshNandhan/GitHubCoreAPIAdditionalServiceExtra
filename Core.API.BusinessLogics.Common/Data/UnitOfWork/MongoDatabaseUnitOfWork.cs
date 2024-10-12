using Core.Library.ArivuTharavuThalam;
using Core.Library.ArivuTharavuThalam.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace Core.API.BusinessLogics.Common
{
    public class MongoDatabaseUnitOfWork : IUnitOfWork
    {
        public IGenericRepository<Book> BookRepository { get; }

        public IGenericRepository<Person> PersonRepository { get; }

        public MongoDatabaseUnitOfWork(IGenericRepository<Book> bookRepository, IGenericRepository<Person> personRepository)
        {
            this.BookRepository = bookRepository;
            this.PersonRepository = personRepository;
        }

        public virtual async Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            //return this.mongoClient.SaveChangesAsync(cancellationToken);
            return await Task.Run(() =>
            {
                return 1;
            }).ConfigureAwait(false);
        }
    }
}
