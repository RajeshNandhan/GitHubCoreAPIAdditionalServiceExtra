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
    public class PersonDirector : IPersonDirector
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IDataCache cacheService;

        //TO DO - CacheKeyForPerson needs to have algorithm and support distributed implementation
        private string CacheKeyForPerson = "Core.Net.PrincipleService.PersonDirector";
        //MAKE THIS ZERO TO GET CACHE 7 DAYS
        private int CacheExpirationTimeInSeconds = 5 * 60;

        public PersonDirector(IUnitOfWork unitOfWork, IDataCache cacheService)
        {
            this.unitOfWork = unitOfWork;
            this.cacheService = cacheService;
        }

        public async Task<IEnumerable<Person>> GetPersonMany(bool isCacheRequired, CancellationToken cancellationToken)
        {
            var cacheData = await cacheService.GetStringAsync<IEnumerable<Person>>(CacheKeyForPerson, cancellationToken).ConfigureAwait(false);

            if (cacheData == null || cacheData.Count() == 0)
            {
                cacheData = await unitOfWork.PersonRepository.GetAllAsync(cancellationToken);
                if (isCacheRequired)
                {
                    await cacheService.SetStringAsync(CacheKeyForPerson, cacheData, CacheExpirationTimeInSeconds, cancellationToken).ConfigureAwait(false);
                }
            }

            return cacheData.OrderBy(person => person.personId);
        }

        public async Task<Person> GetPersonByPredicate(Expression<Func<Person, bool>> predicate, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.PersonRepository.FindOneAsync(predicate, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<IEnumerable<Person>> GetPersonManyByPredicate(Expression<Func<Person, bool>> predicate, CancellationToken cancellationToken)
        {
            var results = await unitOfWork.PersonRepository.FindAllAsync(predicate, cancellationToken).ConfigureAwait(false);
            return results;
        }

        public async Task<long> UpdatePersonByPredicate(Expression<Func<Person, bool>> predicate, Person person, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.PersonRepository.UpdateAsync(predicate, person, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<long> UpdatePersonManyByPredicate(Expression<Func<Person, bool>> predicate, IEnumerable<Person> persons, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.PersonRepository.UpdateManyAsync(predicate, persons, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<Person> CreatePerson(Person person, CancellationToken cancellationToken, int isMongoDB = 0)
        {
            if (person != null)
            {
                if (isMongoDB != 1)
                {
                    person.Id = Guid.NewGuid().ToString();
                }

                person.dateCreated = DateTime.Now;

                await unitOfWork.PersonRepository.InsertAsync(person, cancellationToken).ConfigureAwait(false);
                //await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false); not required since save changes, but due to this intercepter not happening
            }

            return person;
        }

        public async Task<IEnumerable<Person>> CreatePersonMany(IEnumerable<Person> persons, CancellationToken cancellationToken, int isMongoDB = 0)
        {
            if (persons != null)
            {
                foreach (Person person in persons)
                {
                    if (isMongoDB != 1)
                    {
                        person.Id = Guid.NewGuid().ToString();
                    }

                    person.dateCreated = DateTime.Now;
                }

                await unitOfWork.PersonRepository.InsertManyAsync(persons, cancellationToken).ConfigureAwait(false);
            }

            return persons;
        }

        public async Task<long> DeletePersonByPredicate(Expression<Func<Person, bool>> predicate, bool isCacheRequired, CancellationToken cancellationToken)
        {
            if (isCacheRequired)
            {
                await cacheService.RemoveAsync(CacheKeyForPerson, cancellationToken).ConfigureAwait(false);
            }

            var result = await unitOfWork.PersonRepository.DeleteAsync(predicate, cancellationToken).ConfigureAwait(false);

            return result;
        }

        public async Task<long> DeletePersonAll(bool isCacheRequired, CancellationToken cancellationToken)
        {
            if (isCacheRequired)
            {
                await cacheService.RemoveAsync(CacheKeyForPerson, cancellationToken).ConfigureAwait(false);
            }

            var result = await unitOfWork.PersonRepository.DeleteAllAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }

        public async Task<IEnumerable<Person>> LoadAllPersonForNewDatabase(CancellationToken cancellationToken, int isMongoDB = 0)
        {
            IEnumerable<Person> persons = DatabaseInitializerPerson.GetPersons();
            var result = await CreatePersonMany(persons, cancellationToken, isMongoDB).ConfigureAwait(false);
            return result;
        }
    }
}
