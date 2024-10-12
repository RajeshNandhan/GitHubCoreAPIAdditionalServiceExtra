using Core.Library.ArivuTharavuThalam;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Core.API.BusinessLogics.Common
{
    public interface IBookDirector
    {
        Task<IEnumerable<Book>> GetBookMany(CancellationToken cancellationToken, bool isCacheRequired);

        Task<Book> GetBookByPredicate(Expression<Func<Book, bool>> predicate, CancellationToken cancellationToken);

        Task<IEnumerable<Book>> GetBookManyByPredicate(Expression<Func<Book, bool>> predicate, CancellationToken cancellationToken);

        Task<long> UpdateBookByPredicate(Expression<Func<Book, bool>> predicate, Book book, CancellationToken cancellationToken);

        Task<long> UpdateBookManyByPredicate(Expression<Func<Book, bool>> predicate, IEnumerable<Book> books, CancellationToken cancellationToken);

        Task<Book> CreateBook(Book book, CancellationToken cancellationToken, int isMongoDB);

        Task<IEnumerable<Book>> CreateBookMany(IEnumerable<Book> books, CancellationToken cancellationToken, int isMongoDB);

        Task<long> DeleteBookByPredicate(Expression<Func<Book, bool>> predicate, bool isCacheRequired, CancellationToken cancellationToken);

        Task<long> DeleteBookAll(bool isCacheRequired, CancellationToken cancellationToken);

        Task<IEnumerable<Book>> LoadAllBookForNewDatabase(CancellationToken cancellationToken, int isMongoDB);
    }
}
