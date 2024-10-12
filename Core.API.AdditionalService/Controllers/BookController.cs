using System.Collections.Generic;
using System.Threading.Tasks;
using Core.API.BusinessLogics.Common;
using Core.Library.ArivuTharavuThalam;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Core.API.AdditionalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookDirector bookDirector;
        private readonly IConfiguration configuration;

        public BookController(IBookDirector bookDirector, IConfiguration configuration)
        {
            this.bookDirector = bookDirector;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<IEnumerable<Book>> Get()
        {

            IEnumerable<Book> books = await bookDirector.GetBookMany(default, configuration.IsCacheRequired()).ConfigureAwait(false);
            return books;
        }

        [HttpGet("{bookId}")]
        public async Task<Book> Get(int bookId)
        {
            var result = await bookDirector.GetBookByPredicate(bookexpression => bookexpression.bookId == bookId, default).ConfigureAwait(false);
            return result;
        }

        [HttpPut("{bookId}")]
        public async Task<long> Put(int bookId, Book book)
        {
            var result = await bookDirector.UpdateBookByPredicate(bookexpression => bookexpression.bookId == bookId, book, default).ConfigureAwait(false);
            return result;
        }

        [HttpPut("Many")]
        public async Task<long> PutMany(IEnumerable<Book> books)
        {
            var result = await bookDirector.UpdateBookManyByPredicate(bookexpression => true, books, default).ConfigureAwait(false);
            return result;
        }

        [HttpPost]
        public async Task<Book> Post(Book book)
        {
            var bookresult = await bookDirector.CreateBook(book, default, configuration.IsMongoDB()).ConfigureAwait(false);
            return bookresult;
        }

        [HttpPost("Many")]
        public async Task<IEnumerable<Book>> PostMany(IEnumerable<Book> books)
        {
            var bookresult = await bookDirector.CreateBookMany(books, default, configuration.IsMongoDB()).ConfigureAwait(false);
            return bookresult;
        }

        [HttpDelete("{bookId}")]
        public async Task<long> Delete(int bookId)
        {
            var result = await bookDirector.DeleteBookByPredicate(bookexpression => bookexpression.bookId == bookId, configuration.IsCacheRequired(), default).ConfigureAwait(false);
            return result;
        }

        [HttpDelete("Many")]
        public async Task<long> DeleteAll()
        {
            var result = await bookDirector.DeleteBookAll(configuration.IsCacheRequired(), default).ConfigureAwait(false);
            return result;
        }

        [HttpGet("LoadAllBookForNewDatabase")]
        public async Task<IEnumerable<Book>> LoadAllBookForNewDatabase()
        {
            var result = await bookDirector.LoadAllBookForNewDatabase(default, configuration.IsMongoDB()).ConfigureAwait(false);
            return result;
        }
    }
}
