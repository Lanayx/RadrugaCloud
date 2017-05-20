namespace Infrastructure.Repositories.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.Tools.CopyHelper;
    using InfrastructureTools;

    /// <summary>
    /// The personQuality repository.
    /// </summary>
    public sealed class PersonQualityRepository : IPersonQualityRepository
    {
        #region Static Fields

        /// <summary>
        /// The all personQualities.
        /// </summary>
        private readonly List<PersonQuality> _allPersonQualities = new List<PersonQuality>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonQualityRepository"/> class.
        /// </summary>
        public PersonQualityRepository()
        {
            if (!_allPersonQualities.Any())
            {
                _allPersonQualities.Add(
                   new PersonQuality
                   {
                       Id = "Id_target",
                       Name = "Target guy",
                       Description = "This is a Person type for age testing"
                   });
                _allPersonQualities.Add(
                    new PersonQuality
                    {
                        Id = "Id_young",
                        Name = "Young guy",
                        Description = "This is a second Person type for age testing"
                    });
                _allPersonQualities.Add(
                    new PersonQuality
                    {
                        Id = "Id_old",
                        Name = "Old guy",
                        Description = "This is a thirgd Person type for age testing"
                    });
                _allPersonQualities.Add(
                    new PersonQuality
                        {
                            Id = "PersonQuality1", 
                            Name = "First PersonQuality", 
                            Description = "This is a Person type for testing"
                        });
                _allPersonQualities.Add(
                    new PersonQuality
                        {
                            Id = "PersonQuality2", 
                            Name = "Second PersonQuality",
                            Description = "This is a second Person type for testing"
                        });
               
               
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add personQuality.
        /// </summary>
        /// <param name="personQuality">
        /// The personQuality.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IdResult> AddPersonQuality(PersonQuality personQuality)
        {
            personQuality.Id = Guid.NewGuid().ToString();
            await Task.Factory.StartNew(() => _allPersonQualities.Add(personQuality));
            return new IdResult(personQuality.Id);
        }

        /// <summary>
        /// The delete personQuality.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<OperationResult> DeletePersonQuality(string id)
        {
            var personQuality = await GetPersonQuality(id);
            _allPersonQualities.Remove(personQuality);
            return new OperationResult(OperationResultStatus.Success);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            // nothing to dispose
        }

        /// <summary>
        /// The get personQuality.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<PersonQuality> GetPersonQuality(string id)
        {
            return Task.Factory.StartNew(() => _allPersonQualities.Find(personQuality => personQuality.Id == id));
        }

        /// <summary>
        /// The get personQualities.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<List<PersonQuality>> GetPersonQualities(QueryOptions<PersonQuality> options)
        {
            if (options == null)
            {
                return Task.Factory.StartNew(() => _allPersonQualities);
            }
            return Task.Factory.StartNew(() => options.SimpleApply(_allPersonQualities.AsQueryable()).ToList());
        }
       

        /// <summary>
        /// The update personQuality.
        /// </summary>
        /// <param name="personQuality">
        /// The personQuality.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<OperationResult> UpdatePersonQuality(PersonQuality personQuality)
        {
            var existingPersonQuality = await GetPersonQuality(personQuality.Id);
            if (existingPersonQuality == null)
            {
                return OperationResult.NotFound;
            }

            personQuality.CopyTo(existingPersonQuality);
            return new OperationResult(OperationResultStatus.Success);
        }

        #endregion
    }
}