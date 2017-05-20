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
    using Core.NonDomainModels;
    using Core.Tools.CopyHelper;
    using InfrastructureTools;

    /// <summary>
    /// The mission draft repository.
    /// </summary>
    public sealed class MissionDraftRepository : IMissionDraftRepository
    {
        #region Static Fields

        /// <summary>
        /// The all mission drafts.
        /// </summary>
        private readonly List<MissionDraft> _allMissionDrafts = new List<MissionDraft>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MissionDraftRepository"/> class.
        /// </summary>
        public MissionDraftRepository()
        {
            if (!_allMissionDrafts.Any())
            {
                _allMissionDrafts.Add(
                    new MissionDraft
                        {
                            Id = "Id_MissionDraft1",
                            Name = "First MissionDraft",
                            Description = "In this missionDraft you need to put your ass in the air.",
                            PhotoUrl = "http://www.bubblews.com/assets/images/news/283489062_1358762112.png"
                        });
                _allMissionDrafts.Add(
                    new MissionDraft
                        {
                            Id = "Id_MissionDraft2",
                            Name = "Second MissionDraft",
                            Description = "In this missionDraft you have to byte your elbow.",
                            PhotoUrl = "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                            AgeFrom = 10,
                            Author = "Odin",
                            PersonQualities =
                                new List<PersonQualityIdWithScore>
                                    {
                                        new PersonQualityIdWithScore
                                            {
                                                Score = 0,
                                                PersonQualityId ="PersonQuality1"
                                            },
                                         new PersonQualityIdWithScore
                                            {
                                                Score = 1,
                                                PersonQualityId ="PersonQuality2"
                                            },
                                    }
                        });
                _allMissionDrafts.Add(
                    new MissionDraft
                        {
                            Id = "Id_MissionDraft3",
                            Name = "Third MissionDraft",
                            AddDate = DateTime.UtcNow,
                            Description = "In this missionDraft you have to byte your elbow.",
                            PhotoUrl = " http://cs540104.vk.me/c540103/v540103018/14e6d/ZNqbo3CDv7Y.jpg",
                            AgeFrom = 18,
                            Author = "Odin",
                            PersonQualities =
                                new List<PersonQualityIdWithScore>
                                    {
                                        new PersonQualityIdWithScore
                                            {
                                                Score = 0,
                                                PersonQualityId = "Id_young"
                                            }
                                    }
                        });
            }
        }

        #endregion
       
        #region Public Methods and Operators

        /// <summary>
        /// The add mission draft.
        /// </summary>
        /// <param name="missionDraft">
        /// The mission draft.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IdResult> AddMissionDraft(MissionDraft missionDraft)
        {
            missionDraft.Id = Guid.NewGuid().ToString();
            await Task.Factory.StartNew(() => _allMissionDrafts.Add(missionDraft));
            return new IdResult(missionDraft.Id);
        }

        /// <summary>
        /// The delete mission draft.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<OperationResult> DeleteMissionDraft(string id)
        {
            await Task.FromResult(_allMissionDrafts.RemoveAll(md => md.Id == id));
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
        /// The get mission draft.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<MissionDraft> GetMissionDraft(string id)
        {
            return Task.Factory.StartNew(() => _allMissionDrafts.Find(missionDraft => missionDraft.Id == id).Copy());
        }

        /// <summary>
        /// The get mission drafts.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<List<MissionDraft>> GetMissionDrafts(QueryOptions<MissionDraft> options)
        {
            if (options == null)
            {
                return Task.Factory.StartNew(() => _allMissionDrafts);
            }

            return Task.Factory.StartNew(() => options.SimpleApply(_allMissionDrafts.AsQueryable()).ToList());
        }

        /// <summary>
        /// The update mission draft.
        /// </summary>
        /// <param name="missionDraft">
        /// The mission draft.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<OperationResult> UpdateMissionDraft(MissionDraft missionDraft)
        {
            var existingMissionDraft = await Task.FromResult(_allMissionDrafts.FirstOrDefault(md => md.Id == missionDraft.Id));
            if (existingMissionDraft == null)
            {
                return OperationResult.NotFound;
            }

            missionDraft.CopyTo(existingMissionDraft);
            return new OperationResult(OperationResultStatus.Success);
        }

        #endregion

        /// <summary>
        ///     Removes the type of the links to deleted person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> RemoveLinksToDeletedPersonQuality(string personQualityId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Updates the type of the links to person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <param name="personQualityName">Name of the person quality.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> UpdateLinksToPersonQuality(string personQualityId, string personQualityName)
        {
            throw new NotImplementedException();
        }
    }
}