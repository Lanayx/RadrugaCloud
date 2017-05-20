namespace Infrastructure.Repositories.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
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
    using InfrastructureTools;

    /// <summary>
    ///     The mission request repository.
    /// </summary>
    public sealed class MissionRequestRepository : IMissionRequestRepository
    {
        #region Static Fields

        /// <summary>
        ///     The all mission requests.
        /// </summary>
        private readonly List<MissionRequest> _allMissionRequests = new List<MissionRequest>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionRequestRepository" /> class.
        /// </summary>
        public MissionRequestRepository()
        {
            if (!_allMissionRequests.Any())
            {
                _allMissionRequests.Add(
                    new MissionRequest
                        {
                            Id = "Request1",
                            MissionId = "Mission1",
                            Status = MissionRequestStatus.NotChecked,
                            UserId = "User1Id",
                            Mission = new Mission
                                          {
                                              Name = "Super Mission",
                                              Difficulty = 3
                                          },
                            User = new User
                                       {
                                           Id = "User1Id",
                                           NickName = "John",
                                           Level = 2,
                                           Points = 120,
                                           ActiveMissionIds = new List<MissionIdWithSetId> { new MissionIdWithSetId { MissionId = "Mission1" }, new MissionIdWithSetId { MissionId = "Mission2" } },
                                           ActiveMissionSetIds = new List<string> { GameConstants.MissionSet.FirstSetId, GameConstants.MissionSet.SecondSetId, GameConstants.MissionSet.ThirdSetId }
                                       },
                            LastUpdateDate = DateTime.UtcNow,
                            Proof = new MissionProof
                                        {
                                            ImageUrls = new List<string>
                                                            {
                                                                "http://cs540104.vk.me/c540102/v540102927/1dbd2/YHovj9WmxPs.jpg",
                                                                "http://cs540104.vk.me/c7007/v7007850/215bf/SXFDsiqoFzE.jpg"
                                                            }
                                        }
                                          
                        });
                _allMissionRequests.Add(
                    new MissionRequest
                        {
                            Id = "Request2",
                            MissionId = "Mission2",
                            Status = MissionRequestStatus.NotChecked,
                            UserId = "User2Id",
                            Mission = new Mission
                                          {
                                              Name = "Super Mission 2",
                                              Difficulty = 4
                                          },
                            User = new User
                            {
                                Id = "User2Id",
                                NickName = "Jim",
                                Level = 3,
                                Points = 60,
                                ActiveMissionIds = new List<MissionIdWithSetId> { new MissionIdWithSetId { MissionId = "Mission1" }, new MissionIdWithSetId { MissionId = "Mission2" } },
                                ActiveMissionSetIds = new List<string> { GameConstants.MissionSet.FirstSetId, GameConstants.MissionSet.SecondSetId, GameConstants.MissionSet.ThirdSetId }
                            },
                            Proof = new MissionProof
                                        {
                                            CreatedText = "This is a long story to describe"
                                        },
                            LastUpdateDate = DateTime.UtcNow.AddDays(-1)
                        });
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add mission request.
        /// </summary>
        /// <param name="missionRequest">
        ///     The mission request.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<IdResult> AddMissionRequest(MissionRequest missionRequest)
        {
            missionRequest.Id = Guid.NewGuid().ToString();
            await Task.Factory.StartNew(() => _allMissionRequests.Add(missionRequest));
            return new IdResult(missionRequest.Id);
        }

        /// <summary>
        ///     The delete mission request.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<OperationResult> DeleteMissionRequest(string id)
        {
            var mission = await GetMissionRequest(id);
            _allMissionRequests.Remove(mission);
            return new OperationResult(OperationResultStatus.Success);
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            // nothing to dispose
        }

        /// <summary>
        ///     The get mission request.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task<MissionRequest> GetMissionRequest(string id)
        {
            return Task.Factory.StartNew(() => _allMissionRequests.Find(mr => mr.Id == id));
        }

        /// <summary>
        ///     The get mission request.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <param name="include">
        ///     The include.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task<MissionRequest> GetMissionRequest(string id, params string[] include)
        {
            return Task.Factory.StartNew(
                () =>
                    {
                        var mission = _allMissionRequests.Find(mr => mr.Id == id);
                        mission.User = new User
                                           {
                                               Id = "User1Id", 
                                               AvatarUrl =
                                                   "http://cs540104.vk.me/c540102/v540102420/e88c/P64liS_pPNk.jpg", 
                                               DateOfBirth = new DateTime(1990, 1, 1), 
                                               NickName = "John",  
                                               HomeCoordinate = new GeoCoordinate(53.9, 27.56667, 199), 
                                               KindScale = 87, 
                                               Points = 120, 
                                               EnablePushNotifications = true, 
                                               Level = 7, 
                                               Sex = Sex.Male, 
                                               PersonQualitiesWithScores =
                                                    new List<PersonQualityIdWithScore>
                                                                {
                                                                    new PersonQualityIdWithScore {
                                                                        PersonQualityId = Guid.NewGuid().ToString(),
                                                                        Score = 2}
                                                                }
                                             };
                        return mission;
                    });
        }

        /// <summary>
        /// The get mission requests.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<List<MissionRequest>> GetMissionRequests(QueryOptions<MissionRequest> options)
        {
            if (options == null)
            {
                return Task.Factory.StartNew(() => _allMissionRequests);
            }

            return Task.Factory.StartNew(() => options.SimpleApply(_allMissionRequests.AsQueryable()).ToList());
        }

        /// <summary>
        ///     The update mission request.
        /// </summary>
        /// <param name="missionRequest">
        ///     The mission request.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<OperationResult> UpdateMissionRequest(MissionRequest missionRequest)
        {
            var existingMissionRequest = await GetMissionRequest(missionRequest.Id);
            if (existingMissionRequest == null)
            {
                return OperationResult.NotFound;
            }

            missionRequest.CopyTo(existingMissionRequest);
            return new OperationResult(OperationResultStatus.Success);
        }

        #endregion
    }
}