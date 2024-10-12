using Core.Library.ArivuTharavuThalam;
using Core.Library.ArivuTharavuThalam.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace Core.API.BusinessLogics.Common
{
    public interface IUnitOfWork
    {
        IGenericRepository<Book> BookRepository { get; }

        IGenericRepository<Person> PersonRepository { get; }

        /// <summary>
        /// Commits all work to data store.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Number of rows.</returns>
        Task<int> CommitAsync(CancellationToken cancellationToken);
    }
}
