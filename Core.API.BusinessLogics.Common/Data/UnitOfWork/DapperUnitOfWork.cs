using Core.Library.ArivuTharavuThalam.Repository;
using Core.Library.ArivuTharavuThalam;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Core.API.BusinessLogics.Common.Data.UnitOfWork
{
    public class DapperUnitOfWork : IUnitOfWork
    {
        private readonly DbContext sqlDbContext;

        public IGenericRepository<Person> PersonRepository { get; }

        public IGenericRepository<Book> BookRepository { get; }

        public DapperUnitOfWork(DapperAppContext sqlDbContext, IGenericRepository<Person> personRepository)
        {
            this.sqlDbContext = sqlDbContext;
            this.PersonRepository = personRepository;
        }

        public virtual Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            return sqlDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
