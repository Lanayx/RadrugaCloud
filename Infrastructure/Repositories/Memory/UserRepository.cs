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

    using Infrastructure.InfrastructureTools;

    /// <summary>
    ///     The user repository.
    /// </summary>
    public sealed class UserRepository : IUserRepository
    {
        /// <summary>
        ///     The all users.
        /// </summary>
        private readonly List<User> _allUsers = new List<User>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserRepository" /> class.
        /// </summary>
        public UserRepository()
        {
            if (!_allUsers.Any())
            {
                var personQuality = new PersonQuality
                                        {
                                            Id = GameConstants.PersonQuality.ActivityQualityId,
                                            Name = "Активность"
                                        };
                var personQualityWithScore = new PersonQualityIdWithScore
                                                 {
                                                     PersonQualityId = personQuality.Id,
                                                     Score = -2
                                                 };
                var personQuality2 = new PersonQuality
                                         {
                                             Id = GameConstants.PersonQuality.CommunicationQualityId,
                                             Name = "Коммуникативность"
                                         };
                var personQualityWithScore2 = new PersonQualityIdWithScore
                                                  {
                                                      PersonQualityId = personQuality2.Id,
                                                      Score = 2
                                                  };

                _allUsers.Add(
                    new User
                        {
                            Id = "User1Id",
                            AvatarUrl = "http://cs540104.vk.me/c540102/v540102420/e88c/P64liS_pPNk.jpg",
                            DateOfBirth = new DateTime(1990, 1, 1),
                            NickName = "John",
                            HomeCoordinate = new GeoCoordinate(53.9, 27.56667, 199),
                            BaseSouthCoordinate = new GeoCoordinate(53.897863, 27.566649),
                            BaseWestCoordinate = new GeoCoordinate(53.899994, 27.562583),
                            BaseEastCoordinate = new GeoCoordinate(53.900019, 27.570415),
                            BaseNorthCoordinate = new GeoCoordinate(53.902503, 27.566606),
                            Selected2BaseCoordinates = new List<string> { "East", "North" },
                            RadarCoordinate = new GeoCoordinate(53.900493, 27.567513),
                            KindScale = 87,
                            Points = 120,
                            EnablePushNotifications = true,
                            Level = 7,
                            LevelPoints = 50,
                            Sex = Sex.Male,
                            RadrugaColor = "e27b2f",
                            CityShortName = "Brest",
                            CountryShortName = "Belarus",
                            PersonQualitiesWithScores =
                                new List<PersonQualityIdWithScore>
                                    {
                                        personQualityWithScore,
                                        personQualityWithScore2
                                    },
                            ActiveMissionIds =
                                new List<MissionIdWithSetId>
                                    {
                                        new MissionIdWithSetId { MissionId = "Mission1" },
                                        new MissionIdWithSetId { MissionId = "fe9b1f3a-059b-4b4e-8f08-0a22a21a1ed0" },
                                        new MissionIdWithSetId { MissionId = "3c27b903-cf03-447e-9682-f756e70ca908" },
                                        new MissionIdWithSetId { MissionId = "33f075f4-4e8c-47bc-b0d7-9661f4ecdd5d" },
                                        new MissionIdWithSetId { MissionId = "33f075f4-4e8c-47bc-b0d7-9661f4ecdd6d" },
                                        new MissionIdWithSetId { MissionId = "33f075f4-4e8c-47bc-b0d7-9661f4ecdd7d" },
                                        new MissionIdWithSetId { MissionId = "33f075f4-4e8c-47bc-b0d7-9661f4ecdd8d" },
                                        new MissionIdWithSetId { MissionId = "2ddf9168-b030-4b6c-a038-72593e7a75f1" },
                                        new MissionIdWithSetId { MissionId = "2ddf9168-b030-4b6c-a038-72593e7a75f2" },
                                        new MissionIdWithSetId { MissionId = "d061155a-9504-498d-a6e7-bcc20c295cde" },
                                        new MissionIdWithSetId { MissionId = "8ab5fa9b-15a7-49ec-8d5f-2a47d4affd52" },
                                        new MissionIdWithSetId { MissionId = "71130d2e-e513-4b29-ad12-7a368f4d927a" },
                                        new MissionIdWithSetId { MissionId = "19e18c3f-2daa-4e18-bf32-2a1f77fdc73f" },
                                        new MissionIdWithSetId { MissionId = "b23f79fe-08d3-4d24-a293-51f67e2131be" },
                                        new MissionIdWithSetId { MissionId = "524b9f98-88b2-460a-ae3f-e15df4cef2ae" },
                                        new MissionIdWithSetId { MissionId = "2c187275-97c0-4dcf-a1f0-89d7018a03e2" },
                                        new MissionIdWithSetId { MissionId = "3be2d03c-f100-42be-bc9f-54166e108d43" },
                                        new MissionIdWithSetId { MissionId = "8c67c3de-5458-4c6a-a09e-be2d06cdcb2e" },
                                        new MissionIdWithSetId { MissionId = "68d75498-e44f-4bf8-a2ee-13b4998c17d3" },
                                        new MissionIdWithSetId { MissionId = "68d75498-e44f-4bf8-a2ee-13b4998c17d4" },
                                        new MissionIdWithSetId { MissionId = "68d75498-e44f-4bf8-a2ee-13b4998c17d5" },
                                        new MissionIdWithSetId { MissionId = "68d75498-e44f-4bf8-a2ee-13b4998c17d6" },
                                        new MissionIdWithSetId { MissionId = "68d75498-e44f-4bf8-a2ee-13b4998c17d7" },
                                        new MissionIdWithSetId { MissionId = "68d75498-e44f-4bf8-a2ee-13b4998c1725" },
                                        new MissionIdWithSetId { MissionId = "5823973e-ac62-4efe-9c5d-1755df874d99" }
                                    },
                            ActiveMissionSetIds =
                                new List<string>
                                    {
                                        GameConstants.MissionSet.FirstSetId, GameConstants.MissionSet.SecondSetId,
                                        GameConstants.MissionSet.ThirdSetId
                                    },
                            MissionSetIds =
                                new List<MissionSetIdWithOrder>
                                    {
                                        new MissionSetIdWithOrder
                                            {
                                                MissionSetId =
                                                    GameConstants.MissionSet.FirstSetId,
                                                Order = 0
                                            },
                                        new MissionSetIdWithOrder
                                            {
                                                MissionSetId =
                                                    GameConstants.MissionSet.SecondSetId,
                                                Order = 1
                                            },
                                        new MissionSetIdWithOrder
                                            {
                                                MissionSetId =
                                                    GameConstants.MissionSet.ThirdSetId,
                                                Order = 2
                                            }
                                    }
                        });
                _allUsers.Add(
                    new User
                        {
                            Id = "User2Id",
                            AvatarUrl = "https://pp.vk.me/c622726/v622726000/276d6/g6W5sDwP0jc.jpg",
                            DateOfBirth = new DateTime(1990, 1, 1),
                            NickName = "Jim",
                            HomeCoordinate = new GeoCoordinate(55.5554, 28.7778),
                            KindScale = 87,
                            Points = 60,
                            EnablePushNotifications = true,
                            Level = 7,
                            LevelPoints = 50,
                            Sex = Sex.Male,
                            RadrugaColor = "e27b2f",
                            PersonQualitiesWithScores =
                                new List<PersonQualityIdWithScore>
                                    {
                                        personQualityWithScore,
                                        personQualityWithScore2
                                    },
                            ActiveMissionIds =
                                new List<MissionIdWithSetId>
                                    {
                                        new MissionIdWithSetId { MissionId = "Mission1" },
                                        new MissionIdWithSetId { MissionId = "Mission2" }
                                    },
                            ActiveMissionSetIds =
                                new List<string>
                                    {
                                        GameConstants.MissionSet.FirstSetId, GameConstants.MissionSet.SecondSetId,
                                        GameConstants.MissionSet.ThirdSetId
                                    },
                            MissionSetIds =
                                new List<MissionSetIdWithOrder>
                                    {
                                        new MissionSetIdWithOrder
                                            {
                                                MissionSetId =
                                                    GameConstants.MissionSet.FirstSetId,
                                                Order = 0
                                            },
                                        new MissionSetIdWithOrder
                                            {
                                                MissionSetId =
                                                    GameConstants.MissionSet.SecondSetId,
                                                Order = 1
                                            }
                                    }
                        });
            }
        }

        /// <summary>
        ///     The add user.
        /// </summary>
        /// <param name="user">
        ///     The user.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<IdResult> AddUser(User user)
        {
            await Task.Factory.StartNew(() => _allUsers.Add(user));
            return new IdResult(user.Id);
        }

        public Task<OperationResult> DecreaseKindActionScales()
        {
            foreach (var user in _allUsers)
            {
                if (user.KindScale < GameConstants.KindScale.DailyRegression)
                {
                    user.KindScale = 0;
                    user.KindScaleHighCurrentDays = 0;
                }
                else
                {
                    if (user.KindScale > GameConstants.KindScale.AchievementLimit)
                    {
                        user.KindScaleHighCurrentDays = (user.KindScaleHighCurrentDays ?? 0) + 1;
                        if (!user.KindScaleHighMaxDays.HasValue
                            || user.KindScaleHighCurrentDays > user.KindScaleHighMaxDays)
                        {
                            user.KindScaleHighMaxDays = user.KindScaleHighCurrentDays;
                        }
                    }
                    user.KindScale -= GameConstants.KindScale.DailyRegression;
                }
            }
            return Task.Factory.StartNew(() => new OperationResult(OperationResultStatus.Success));
        }

        /// <summary>
        ///     The delete user.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<OperationResult> DeleteUser(string id)
        {
            var user = await GetUser(id);
            _allUsers.Remove(user);
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
        ///     The get user.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task<User> GetUser(string id)
        {
            return Task.Factory.StartNew(() => _allUsers.Find(user => user.Id == id));
        }

        /// <summary>
        ///     Gets the users.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public Task<IEnumerable<User>> GetUsers(QueryOptions<User> options)
        {
            if (options == null)
            {
                return Task.Factory.StartNew(() => _allUsers.AsEnumerable());
            }

            return Task.Factory.StartNew(() => options.SimpleApply(_allUsers.AsQueryable()).AsEnumerable());
        }

        public Task<OperationResult> UpdateLastRatingsPlaces()
        {
            var orderedUsers = _allUsers.OrderBy(u => u.Points).ThenBy(u => u.LastRatingPlace).ToArray();
            for (var index = 0; index < orderedUsers.Length; index++)
            {
                var user = orderedUsers[index];
                if (index < user.LastRatingPlace)
                {
                    user.UpInRatingCurrentDays = (user.UpInRatingCurrentDays ?? 0) + 1;
                    if (user.UpInRatingCurrentDays > user.UpInRatingMaxDays)
                    {
                        user.UpInRatingMaxDays = user.UpInRatingMaxDays;
                    }
                }
                user.LastRatingPlace = index;
            }
            return Task.Factory.StartNew(() => new OperationResult(OperationResultStatus.Success));
        }

        /// <summary>
        ///     Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="replace">if set to <c>true</c> [replace].</param>
        /// <returns>Task{OperationResult}.</returns>
        /// <exception cref="System.ArgumentNullException">Invalid input</exception>
        public async Task<OperationResult> UpdateUser(User user, bool replace)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Invalid input");
            }

            var existingUser = await GetUser(user.Id);
            if (existingUser == null)
            {
                return new OperationResult(OperationResultStatus.Error, "User was not found.");
            }

            user.CopyTo(existingUser, false);
            return new OperationResult(OperationResultStatus.Success);
        }

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