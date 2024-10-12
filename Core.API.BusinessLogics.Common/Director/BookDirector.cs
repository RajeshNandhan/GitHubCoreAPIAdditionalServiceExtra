using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Library.ArivuTharavuThalam;
using Core.Library.ArivuTharavuThalam.DataCaching;

namespace Core.API.BusinessLogics.Common
{
    public class BookDirector : IBookDirector
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IDataCache cacheService;
        private string CacheKeyForBook = "Core.Net.BookDirector";
        //MAKE THIS ZERO TO GET CACHE 7 DAYS
        private int CacheExpirationTimeInSeconds = 5 * 60;

        public BookDirector(IUnitOfWork unitOfWork, IDataCache cacheService)
        {
            this.unitOfWork = unitOfWork;
            this.cacheService = cacheService;
        }

        public async Task<IEnumerable<Book>> GetBookMany(CancellationToken cancellationToken, bool isCacheRequired)
        {
            var books = await cacheService.GetStringAsync<IEnumerable<Book>>(CacheKeyForBook, cancellationToken).ConfigureAwait(false);

            if (books == null || books.Count() == 0)
            {
                books = await unitOfWork.BookRepository.GetAllAsync(cancellationToken);

                if (isCacheRequired)
                {
                    await cacheService.SetStringAsync(CacheKeyForBook, books, CacheExpirationTimeInSeconds, cancellationToken).ConfigureAwait(false);
                }
            }

            return books.OrderBy(book => book.bookId);
        }

        public async Task<Book> GetBookByPredicate(Expression<Func<Book, bool>> predicate, CancellationToken cancellationToken)
        {
            var book = await unitOfWork.BookRepository.FindOneAsync(predicate, cancellationToken).ConfigureAwait(false);
            return book;
        }

        public async Task<IEnumerable<Book>> GetBookManyByPredicate(Expression<Func<Book, bool>> predicate, CancellationToken cancellationToken)
        {
            var books = await unitOfWork.BookRepository.FindAllAsync(predicate, cancellationToken).ConfigureAwait(false);
            return books;
        }

        public async Task<long> UpdateBookByPredicate(Expression<Func<Book, bool>> predicate, Book book, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.BookRepository.UpdateAsync(predicate, book, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<long> UpdateBookManyByPredicate(Expression<Func<Book, bool>> predicate, IEnumerable<Book> books, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.BookRepository.UpdateManyAsync(predicate, books, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<Book> CreateBook(Book book, CancellationToken cancellationToken, int isMongoDB = 0)
        {
            if (book != null)
            {
                if (isMongoDB != 1)
                {
                    book.Id = Guid.NewGuid().ToString();
                }

                book.dateCreated = DateTime.Now;

                await unitOfWork.BookRepository.InsertAsync(book, cancellationToken).ConfigureAwait(false);
            }

            return book;
        }

        public async Task<IEnumerable<Book>> CreateBookMany(IEnumerable<Book> books, CancellationToken cancellationToken, int isMongoDB = 0)
        {
            if (books != null)
            {
                foreach (Book book in books)
                {
                    if (isMongoDB != 1)
                    {
                        book.Id = Guid.NewGuid().ToString();
                    }
                    book.dateCreated = DateTime.Now;
                }

                await unitOfWork.BookRepository.InsertManyAsync(books, cancellationToken).ConfigureAwait(false);
            }

            return books;
        }

        public async Task<long> DeleteBookByPredicate(Expression<Func<Book, bool>> predicate, bool isCacheRequired, CancellationToken cancellationToken)
        {
            if (isCacheRequired)
            {
                await cacheService.RemoveAsync(CacheKeyForBook, cancellationToken).ConfigureAwait(false);
            }

            var result = await unitOfWork.BookRepository.DeleteAsync(predicate, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<long> DeleteBookAll(bool isCacheRequired, CancellationToken cancellationToken)
        {
            if (isCacheRequired)
            {
                await cacheService.RemoveAsync(CacheKeyForBook, cancellationToken).ConfigureAwait(false);
            }
            var result = await unitOfWork.BookRepository.DeleteAllAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<IEnumerable<Book>> LoadAllBookForNewDatabase(CancellationToken cancellationToken, int isMongoDB = 0)
        {
            IEnumerable<Book> books = DatabaseInitializerBook.GetBooks();
            var result = await CreateBookMany(books, cancellationToken, isMongoDB).ConfigureAwait(false);
            return result;
        }
    }
}
