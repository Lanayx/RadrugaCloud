using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Redis
{
    using System.Diagnostics;
    using System.Web.Configuration;

    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;

    using StackExchange.Redis;

    /// <summary>
    /// Redis rating repository
    /// </summary>
    public class RatingRepository : IRatingRepository
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly int _dbNumber;

        private const string SortedRatingsSetPrefix = "zur:";
        private const string SortedRatingsSetCommon = "zur:common";
        private const string UsersDetailsPrefix = "ud:";


        /// <summary>
        /// Initializes a new instance of the <see cref="RatingRepository"/> class.
        /// </summary>
        /// <param name="connectionMultiplexer">The clients manager.</param>
        public RatingRepository(ConnectionMultiplexer connectionMultiplexer)
        {
            _connection = connectionMultiplexer;
            int.TryParse(WebConfigurationManager.AppSettings["RedisDatabaseIndex"], out _dbNumber);//defaults to 0
        }

        /// <summary>
        /// Builds the ratings from user list.
        /// </summary>
        /// <param name="users">The users.</param>
        /// <returns></returns>
        public async Task BuildRatings(IEnumerable<User> users)
        {

            var database = _connection.GetDatabase(_dbNumber);
            var server = _connection.GetServer(_connection.GetEndPoints()[0]);

            await server.FlushDatabaseAsync(_dbNumber);//clean before bulding

            foreach (var user in users)
            {
                if (!user.Points.HasValue) continue;

                var userKey = CreatedSortedSetUserKey(user);

                await database.SortedSetAddAsync(SortedRatingsSetCommon, userKey, user.Points.Value);
                if (!String.IsNullOrEmpty(user.UniqueCityId))
                {
                    await database.SortedSetAddAsync(SortedRatingsSetPrefix + user.CountryShortName, userKey, user.Points.Value);
                    await database.SortedSetAddAsync(SortedRatingsSetPrefix + user.UniqueCityId, userKey, user.Points.Value);
                }

                var kvList = new List<HashEntry>();
                if (!String.IsNullOrEmpty(user.NickName)) kvList.Add(new HashEntry("Name", user.NickName));
                if (!String.IsNullOrEmpty(user.AvatarUrl)) kvList.Add(new HashEntry("PhotoUrl", user.AvatarUrl));

                await database.HashSetAsync(UsersDetailsPrefix + user.Id, kvList.ToArray());
            }
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _connection.Dispose();
        }

        /// <summary>
        /// Gets the ratings for the current user.
        /// </summary>
        /// <param name="ratingType">Type of the rating.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public async Task<RatingsWithUserCount> GetRatings(RatingType ratingType, User user)
        {
            var newList = new List<RatingInfo>();
            var db = _connection.GetDatabase(_dbNumber);
            var sortedSetName = GetSelectedSortedSetName(ratingType, user);
            var userInLeaders = await FillLeaders(sortedSetName, user, newList, db);
            if (!userInLeaders)
            {
                await FillNeighbours(sortedSetName, user, newList, db);
            }
            var userCount = await GetUsersCount(sortedSetName);
            var result = new RatingsWithUserCount
                             {
                                 Ratings = newList,
                                 UsersCount = userCount
                             };
            return result;
        }

        /// <summary>
        /// Gets the user ranks.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public async Task<Tuple<long, long, long>> GetUserRanks(User user)
        {
            if (!user.Points.HasValue)
            {
                return new Tuple<long, long, long>(-1, -1, -1);
            }

            var db = _connection.GetDatabase(_dbNumber);
            var globalRank = await GetUserRank(GetSelectedSortedSetName(RatingType.Common, user), user,db);
            if (String.IsNullOrEmpty(user.UniqueCityId))
            {
                return new Tuple<long, long, long>(globalRank + 1, -1, -1);
            }
            var countryRank = await GetUserRank(GetSelectedSortedSetName(RatingType.Country, user), user, db);
            var cityRank = await GetUserRank(GetSelectedSortedSetName(RatingType.City, user), user, db);
            return new Tuple<long, long, long>(globalRank+1,countryRank+1, cityRank+1);
        }

        /// <summary>
        /// Updates the avatar.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public async Task UpdateAvatar(User user)
        {
            var db = _connection.GetDatabase(_dbNumber);
            await db.HashSetAsync(UsersDetailsPrefix + user.Id, "PhotoUrl", user.AvatarUrl);
        }

        /// <summary>
        /// Updates the nickname.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public async Task UpdateNickname(User user)
        {
            var db = _connection.GetDatabase(_dbNumber);
            await db.HashSetAsync(UsersDetailsPrefix + user.Id, "Name", user.NickName);
        }

        /// <summary>
        /// Updates the user rating.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="oldPoints">The old points. (not used in Redis implementation)</param>
        /// <param name="newPoints">The new points.</param>
        /// <returns></returns>
        public async Task UpdateUserRating(User user, int? oldPoints, int newPoints)
        {
            var db = _connection.GetDatabase(_dbNumber);
            if (oldPoints.HasValue)
            {
                var lastPlaceWithUserId = await GetLastPlaceWithUserId(user, oldPoints.Value, db);
                await RemoveRatingFromSortedSet(SortedRatingsSetCommon, db, lastPlaceWithUserId);
                if (!String.IsNullOrEmpty(user.UniqueCityId))
                {
                    await RemoveRatingFromSortedSet(SortedRatingsSetPrefix + user.CountryShortName, db, lastPlaceWithUserId);
                    await RemoveRatingFromSortedSet(SortedRatingsSetPrefix + user.UniqueCityId, db, lastPlaceWithUserId);
                }
            }
            else
            {
                await UpdateAvatar(user);
                await UpdateNickname(user);
            }

            var userKey = CreatedSortedSetUserKey(user);
            await db.SortedSetAddAsync(SortedRatingsSetCommon, userKey, user.Points.Value);//points can't be null on update
            if (!String.IsNullOrEmpty(user.UniqueCityId))
            {
                await db.SortedSetAddAsync(SortedRatingsSetPrefix + user.CountryShortName, userKey, user.Points.Value);//points can't be null on update
                await db.SortedSetAddAsync(SortedRatingsSetPrefix + user.UniqueCityId, userKey, user.Points.Value);//points can't be null on update
            }
        }

        private static async Task RemoveRatingFromSortedSet(string sortedSetName, IDatabase db, string lastPlaceWithUserId)
        {
            if (!(await db.SortedSetRemoveAsync(sortedSetName, lastPlaceWithUserId)))
            {
                //throw new Exception("User reload is needed"); - commented out as almost impossible
                //    //when this can happen -> when last place gets updated (e.g. by scheduler) between get user and update rating
            }
        }

        private string GetSelectedSortedSetName(RatingType ratingType, User user)
        {
            if (ratingType!= RatingType.Common && String.IsNullOrEmpty(user.UniqueCityId))
                throw new ArgumentException($"Unsupported rating type four user {user.Id}", nameof(ratingType));

            switch (ratingType)
            {
                case RatingType.Common:
                    return SortedRatingsSetCommon;
                case RatingType.Country:
                    return SortedRatingsSetPrefix + user.CountryShortName;
                case RatingType.City:
                    return SortedRatingsSetPrefix + user.UniqueCityId;
                default:
                    throw new ArgumentException("Unsupported rating type", nameof(ratingType));
            }
        }


        private async Task FillNeighbours(string sortedSetName, User user, List<RatingInfo> newList, IDatabase db)
        {
            if (!user.Points.HasValue)
            {
                newList.Add(
                    new RatingInfo
                    {
                        AvatarUrl = user.AvatarUrl,
                        NickName = user.NickName,
                        Place = -1,
                        Points = 0,
                        UserId = user.Id
                    });
                return;
            }

            var rank = await GetUserRank(sortedSetName, user, db);
            var bottomRank = rank - GameConstants.Rating.NeighboursCountOneSide;
            if (bottomRank < GameConstants.Rating.LeadersCount)
                bottomRank = GameConstants.Rating.LeadersCount;

            var neighboursResult = await db.SortedSetRangeByRankWithScoresAsync(sortedSetName,
                bottomRank, rank + GameConstants.Rating.NeighboursCountOneSide, Order.Descending);


            for (var i = 0; i < neighboursResult.Length; i++)
            {
                var result = neighboursResult[i];
                var keyArray = result.Element.ToString().Split(':');
                var userId = keyArray.Length == 1 ? result.Element.ToString() : keyArray[1];

                var details = await db.HashGetAllAsync(UsersDetailsPrefix + userId);
                newList.Add(
                    new RatingInfo
                    {
                        UserId = userId,
                        AvatarUrl = details.FirstOrDefault(detail => detail.Name == "PhotoUrl").Value,
                        NickName = details.FirstOrDefault(detail => detail.Name == "Name").Value,
                        Points = (int)result.Score,
                        Place = (int)bottomRank + i + 1
                    });
            }
        }

        private string CreatedSortedSetUserKey(User user)
        {
            var specialPrefix = user.LastRatingPlace?.ToString();
            if (!String.IsNullOrEmpty(specialPrefix))
            {
                specialPrefix = specialPrefix.Length + specialPrefix + ":";//from tip http://stackoverflow.com/questions/10410655/
            }
            var userKey = specialPrefix + user.Id;
            return userKey;
        }
        private async Task<long> GetUserRank(string sortedSetName, User user, IDatabase db)
        {
            var usersWithSamePoints =
                await db.SortedSetRangeByScoreAsync(sortedSetName, user.Points.Value, user.Points.Value);//user points should be checked earlier
            var userValue = usersWithSamePoints.First(u => u.ToString().EndsWith(user.Id));
            var rank = await db.SortedSetRankAsync(sortedSetName, userValue, Order.Descending);
            return rank.Value;
        }
      
        private async Task<int> GetUsersCount(string sortedSetName)
        {
            var db = _connection.GetDatabase(_dbNumber);
            var length = await db.SortedSetLengthAsync(sortedSetName);
            return (int)length;
        }

        private async Task<string> GetLastPlaceWithUserId(User user, int oldPoints, IDatabase db)
        {
            var usersWithSamePoints = await db.SortedSetRangeByScoreAsync(SortedRatingsSetCommon, oldPoints, oldPoints);//user points should be checked earlier
            var userValue = usersWithSamePoints.First(u => u.ToString().EndsWith(user.Id));
            return userValue;
        }

        private async Task<bool> FillLeaders(string sortedSetName, User user, List<RatingInfo> emptyList, IDatabase db)
        {
            var userInTop = false;

            var firstResults = await db.SortedSetRangeByRankWithScoresAsync(sortedSetName,
                0, GameConstants.Rating.LeadersCount - 1, Order.Descending);

            for (var i = 0; i < firstResults.Length; i++)
            {
                var result = firstResults[i];
                var keyArray = result.Element.ToString().Split(':');
                var userId = keyArray.Length == 1 ? result.Element.ToString() : keyArray[1];
                if (userId == user.Id)
                    userInTop = true;

                var details = await db.HashGetAllAsync(UsersDetailsPrefix + userId);
                emptyList.Add(
                    new RatingInfo
                    {
                        UserId = userId,
                        AvatarUrl = details.FirstOrDefault(detail => detail.Name == "PhotoUrl").Value,
                        NickName = details.FirstOrDefault(detail => detail.Name == "Name").Value,
                        Points = (int)result.Score,
                        Place = i + 1
                    });
            }
            return userInTop;
        }
    }
}
