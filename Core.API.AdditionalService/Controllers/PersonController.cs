using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Core.Library.ArivuTharavuThalam;
using Core.API.BusinessLogics.Common;

namespace Core.API.AdditionalService.Controllers
{
    /// <summary>
    /// Get() -> Get All persons
    /// Get(int personId) -> Get Person By Id
    /// Put(int personId, Person person) -> Update ONE person by personId
    /// PutMany(IEnumerable[Person] persons) -> Update MANY person
    /// Post(Person person) -> Create ONE person
    /// Postmany(IEnumerable[Person] persons) -> Create MANY person
    /// Delete(int personId) Delete ONE person
    /// DeleteMany(int personId) Delete Many person
    /// LoadAllPersonForNewDatabase() -> Load person from variable and create it in database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonDirector personDirector;
        private readonly IConfiguration configuration;

        public PersonController(IPersonDirector personDirector, IConfiguration configuration)
        {
            this.personDirector = personDirector;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<IEnumerable<Person>> Get()
        {
            var result = await personDirector.GetPersonMany(configuration.IsCacheRequired(), default).ConfigureAwait(false);
            return result;
        }

        [HttpGet("{personId}")]
        public async Task<Person> Get(int personId)
        {
            var result = await personDirector.GetPersonByPredicate(person => person.personId == personId, default).ConfigureAwait(false);
            return result;
        }

        [HttpPut("{personId}")]
        public async Task<long> Put(int personId, Person person)
        {
            var result = await personDirector.UpdatePersonByPredicate(person => person.personId == personId, person, default).ConfigureAwait(false);
            return result;
        }

        [HttpPut("Many")]
        public async Task<long> PutMany(IEnumerable<Person> persons)
        {
            var result = await personDirector.UpdatePersonManyByPredicate(persons => true, persons, default).ConfigureAwait(false);
            return result;
        }

        [HttpPost]
        public async Task<Person> Post(Person person)
        {
            var personresult = await personDirector.CreatePerson(person, default, configuration.IsMongoDB()).ConfigureAwait(false);
            return personresult;
        }

        [HttpPost("Many")]
        public async Task<IEnumerable<Person>> PostMany(IEnumerable<Person> persons)
        {
            var personresult = await personDirector.CreatePersonMany(persons, default, configuration.IsMongoDB()).ConfigureAwait(false);
            return personresult;
        }

        [HttpDelete("{personId}")]
        public async Task<long> Delete(int personId)
        {
            var result = await personDirector.DeletePersonByPredicate(person => person.personId == personId, configuration.IsCacheRequired(), default).ConfigureAwait(false);
            return result;
        }

        [HttpDelete("Many")]
        public async Task<long> DeleteMany()
        {
            var result = await personDirector.DeletePersonAll(configuration.IsCacheRequired(), default).ConfigureAwait(false);
            return result;
        }

        [HttpGet("LoadAllPersonForNewDatabase")]
        public async Task<IEnumerable<Person>> LoadAllPersonForNewDatabase()
        {
            var result = await personDirector.LoadAllPersonForNewDatabase(default, configuration.IsMongoDB()).ConfigureAwait(false);
            return result;
        }
    }
}
