namespace Infrastructure.Repositories.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;
    using Core.Tools.CopyHelper;

    using Infrastructure.InfrastructureTools;

    /// <summary>
    ///     Class MissionSetRepository
    /// </summary>
    public sealed class MissionSetRepository : IMissionSetRepository
    {
        #region Static Fields

        /// <summary>
        /// The all missions.
        /// </summary>
        private readonly List<MissionSet> _allMissionsSets = new List<MissionSet>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MissionRepository"/> class.
        /// </summary>
        public MissionSetRepository()
        {
            if (!_allMissionsSets.Any())
            {
                _allMissionsSets.Add(
                    new MissionSet
                    {
                        AgeFrom = 10,
                        AgeTo = 25,
                        Name = "First set",
                        PersonQualities =
                                new List<PersonQualityIdWithScore>
                                    {
                                        new PersonQualityIdWithScore
                                            {
                                                PersonQualityId = "PersonQuality1",
                                                Score = 3
                                            },
                                        new PersonQualityIdWithScore
                                            {
                                                PersonQualityId = "PersonQuality2",
                                                Score = -2
                                            },
                                    },
                        Missions =
                                new List<MissionWithOrder>
                                    {
                                        new MissionWithOrder
                                            {
                                                Mission =
                                                    new Mission
                                                        {
                                                            Id = "Mission1",
                                                            Name = "First Mission",
                                                            Difficulty = 2,
                                                            Description =
                                                                "In this mission you need to put your ass in the air.",
                                                            PhotoUrl =
                                                                "http://www.bubblews.com/assets/images/news/283489062_1358762112.png"
                                                        },
                                                Order = 0
                                            },
                                        new MissionWithOrder
                                            {
                                                Mission =
                                                    new Mission
                                                        {
                                                            Id = "Mission2",
                                                            Name = "Second Mission",
                                                            Difficulty = 3,
                                                            Description =
                                                                "In this mission you have to byte your elbow.",
                                                            PhotoUrl =
                                                                "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                                                        },
                                                Order = 1
                                            }
                                    },

                        Id = GameConstants.MissionSet.FirstSetId
                    });
                _allMissionsSets.Add(
                    new MissionSet
                    {
                        AgeFrom = 10,
                        AgeTo = 25,
                        Name = "Second set",
                        PersonQualities =
                            new List<PersonQualityIdWithScore>
                                    {
                                        new PersonQualityIdWithScore
                                            {
                                                PersonQualityId = "PersonQuality1",
                                                Score = 3
                                            },
                                        new PersonQualityIdWithScore
                                            {
                                                PersonQualityId = "PersonQuality2",
                                                Score = -2
                                            },
                                    },
                        Missions =
                            new List<MissionWithOrder>
                                    {
                                        new MissionWithOrder
                                            {
                                                Mission =
                                                    new Mission
                                                        {
                                                            Id = "Mission3",
                                                            Name = "First Mission",
                                                            Difficulty = 2,
                                                            Description =
                                                                "In this mission you need to put your ass in the air.",
                                                            PhotoUrl =
                                                                "http://www.bubblews.com/assets/images/news/283489062_1358762112.png"
                                                        },
                                                Order = 0
                                            },
                                        new MissionWithOrder
                                            {
                                                Mission =
                                                    new Mission
                                                        {
                                                            Id = "Mission4",
                                                            Name = "Second Mission",
                                                            Difficulty = 3,
                                                            Description =
                                                                "In this mission you have to byte your elbow.",
                                                            PhotoUrl =
                                                                "http://cwer.ws/media/files/u110224/04/SecondCopy.jpg",
                                                        },
                                                Order = 1
                                            }
                                    },

                        Id = GameConstants.MissionSet.SecondSetId
                    });
                _allMissionsSets.Add(
                    new MissionSet
                    {
                        Id = "RadarSet",
                        AgeFrom = 1,
                        AgeTo = 99,
                        PersonQualities =
                            new List<PersonQualityIdWithScore>
                            {new PersonQualityIdWithScore
                                            {
                                                PersonQualityId = "PersonQuality1",
                                                Score = 3
                                            }},
                        Missions =
                                              new List<MissionWithOrder>
                                    {
                                        new MissionWithOrder
                                            {
                                                Mission =
                                                    new Mission
                                                        {
                                                            Id = "b23f79fe-08d3-4d24-a293-51f67e2131be",
                                                            Name = "Radar",
                                                            Difficulty = 2
                                                        },
                                                Order = 0
                                            }
                                    }
                    });
                _allMissionsSets.Add(
                    new MissionSet
                    {
                        Id = "OutpostSet",
                        AgeFrom = 1,
                        AgeTo = 99,
                        PersonQualities =
                            new List<PersonQualityIdWithScore>
                            {new PersonQualityIdWithScore
                                            {
                                                PersonQualityId = "PersonQuality2",
                                                Score = 3
                                            }},
                        Missions =
                                              new List<MissionWithOrder>
                                    {
                                        new MissionWithOrder
                                            {
                                                Mission =
                                                    new Mission
                                                        {
                                                            Id = "8c67c3de-5458-4c6a-a09e-be2d06cdcb2e",
                                                            Name = "Outpost",
                                                            Difficulty = 2
                                                        },
                                                Order = 0
                                            }
                                    }
                    });

            }
        }

        #endregion


        #region Public Methods and Operators

        /// <summary>
        ///     Adds the MissionSet.
        /// </summary>
        /// <param name="missionSet">
        ///     The MissionSet.
        /// </param>
        /// <returns>
        ///     Task{AddResult}.
        /// </returns>
        public async Task<IdResult> AddMissionSet(MissionSet missionSet)
        {
            missionSet.Id = Guid.NewGuid().ToString();
            await Task.Factory.StartNew(() => _allMissionsSets.Add(missionSet));
            return new IdResult(missionSet.Id);
        }

        /// <summary>
        ///     Deletes the MissionSet.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     Task{OperationResult}.
        /// </returns>
        public async Task<OperationResult> DeleteMissionSet(string id)
        {
            await Task.FromResult(_allMissionsSets.RemoveAll(m => m.Id == id));
            return new OperationResult(OperationResultStatus.Success);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // nothing to dispose
        }

        /// <summary>
        ///     Gets the MissionSet.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     Task{MissionSet}.
        /// </returns>
        public Task<MissionSet> GetMissionSet(string id)
        {
            return Task.Factory.StartNew(() => _allMissionsSets.Find(mission => mission.Id == id).Copy());
        }

        /// <summary>
        ///     Gets the MissionSets.
        /// </summary>
        /// <param name="options">
        ///     The options.
        /// </param>
        /// <returns>
        ///     Task{IEnumerable{MissionSet}}.
        /// </returns>
        public Task<List<MissionSet>> GetMissionSets(QueryOptions<MissionSet> options)
        {
            if (options == null)
            {
                return Task.Factory.StartNew(() => _allMissionsSets);
            }
            return Task.Factory.StartNew(() => options.SimpleApply(_allMissionsSets.AsQueryable()).ToList());
        }


        /// <summary>
        ///     Updates the MissionSet.
        /// </summary>
        /// <param name="missionSet">
        ///     The MissionSet.
        /// </param>
        /// <returns>
        ///     Task{OperationResult}.
        /// </returns>
        public async Task<OperationResult> UpdateMissionSet(MissionSet missionSet)
        {
            var existingMission = await Task.FromResult(_allMissionsSets.FirstOrDefault(ms => ms.Id == missionSet.Id));
            if (existingMission == null)
            {
                return OperationResult.NotFound;
            }

            missionSet.CopyTo(existingMission);
            return new OperationResult(OperationResultStatus.Success);
        }

        /// <summary>
        ///     Refreshes the mission dependent links.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> RefreshMissionDependentLinks(string id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods



        #endregion
    }
}