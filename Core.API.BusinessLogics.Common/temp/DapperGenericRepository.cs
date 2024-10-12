using Microsoft.EntityFrameworkCore;

namespace Core.API.BusinessLogics.Common
{
    public class DapperAppContext : DbContext
    {
        public DapperAppContext() { }
        public DapperAppContext(DbContextOptions<DapperAppContext> options) : base(options) { }
    }

    //public class DapperGenericRepository<TEntity> : IGenericRepository<TEntity>
    //   where TEntity : class
    //{
    //    private readonly IConfiguration _config;
    //    private string Connectionstring = "SQLDBContext";
    //    public DapperGenericRepository(IConfiguration config)
    //    {
    //        _config = config;
    //    }
    //    public Task<long> DeleteAllAsync(CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<long> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    /// <summary>
    //    /// https://www.c-sharpcorner.com/article/using-dapper-in-asp-net-core-web-api/
    //    /// </summary>
    //    /// <param name="cancellationToken"></param>
    //    /// <returns></returns>
    //    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    //    {
    //        string sp = "SELECT * FROM [dbo].[Persons]";
    //        DynamicParameters parms = null;
    //        CommandType commandType = CommandType.Text;
    //        using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
    //        // db.QueryAsync is the dapper extention for IDbConnection
    //        //db.QueryAsync runs RAW sql query against sql connection
    //        var result = await db.QueryAsync<TEntity>(sp,
    //                                 parms,
    //                                 commandType: commandType).ConfigureAwait(false);

    //        return result.ToList();
    //    }

    //    public Task<long> InsertAsync(TEntity entity, CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<long> InsertManyAsync(IEnumerable<TEntity> entityies, CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<long> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity, CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<long> UpdateManyAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<TEntity> entityies, CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
