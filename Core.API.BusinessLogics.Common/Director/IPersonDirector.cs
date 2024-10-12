using Core.Library.ArivuTharavuThalam;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Core.API.BusinessLogics.Common
{
    public interface IPersonDirector
    {
        public Task<IEnumerable<Person>> GetPersonMany(bool isCacheRequired, CancellationToken cancellationToken);

        public Task<Person> GetPersonByPredicate(Expression<Func<Person, bool>> predicate, CancellationToken cancellationToken);

        public Task<IEnumerable<Person>> GetPersonManyByPredicate(Expression<Func<Person, bool>> predicate, CancellationToken cancellationToken);

        public Task<long> UpdatePersonByPredicate(Expression<Func<Person, bool>> predicate, Person person, CancellationToken cancellationToken);

        public Task<long> UpdatePersonManyByPredicate(Expression<Func<Person, bool>> predicate, IEnumerable<Person> persons, CancellationToken cancellationToken);

        public Task<Person> CreatePerson(Person person, CancellationToken cancellationToken, int isMongoDB = 0);

        public Task<IEnumerable<Person>> CreatePersonMany(IEnumerable<Person> persons, CancellationToken cancellationToken, int isMongoDB = 0);

        public Task<long> DeletePersonByPredicate(Expression<Func<Person, bool>> predicate, bool isCacheRequired, CancellationToken cancellationToken);

        public Task<long> DeletePersonAll(bool isCacheRequired, CancellationToken cancellationToken);

        public Task<IEnumerable<Person>> LoadAllPersonForNewDatabase(CancellationToken cancellationToken, int isMongoDB = 0);
    }
}
