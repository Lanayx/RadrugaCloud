namespace Services.DomainServices
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;
    using Core.Tools;
    using Core.Tools.CopyHelper;

    using Microsoft.Practices.Unity;

    /// <summary>
    ///     Service for ratings interactions
    /// </summary>
    public sealed class RatingService
    {
        private readonly IUserRepository _userRepository;

        private readonly IRatingRepository _ratingRepository;

        private static volatile bool userRatingsInitialized;

        private static volatile bool userRatingsInitializing;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService" /> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="ratingRepository">The rating repository.</param>
        [InjectionConstructor]
        public RatingService(IUserRepository userRepository, IRatingRepository ratingRepository)
        {
            _userRepository = userRepository;
            _ratingRepository = ratingRepository;
            if (!userRatingsInitialized)
            {
                Task.Run(() => CheckInitRatingsCache(userRepository)).Wait();
            }
        }

        public RatingService(IUserRepository userRepository, IRatingRepository ratingRepository, bool clean)
        {
            _userRepository = userRepository;
            _ratingRepository = ratingRepository;
            if (clean)
            {
                WaitBeforeCacheInitialized().Wait();
                userRatingsInitialized = false;
                Task.Run(() => CheckInitRatingsCache(userRepository)).Wait();
            }
            else if (!userRatingsInitialized)
            {
                Task.Run(() => CheckInitRatingsCache(userRepository)).Wait();
            }
        }

        private static async Task WaitBeforeCacheInitialized()
        {
            while (userRatingsInitializing)
            {
                await Task.Delay(100);
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            _userRepository.Dispose();
            _ratingRepository.Dispose();
        }

        /// <summary>
        /// Gets the ratings.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="ratingType">Type of the rating.</param>
        /// <returns>
        /// Task{RatingsWithUserCount}.
        /// </returns>
        public async Task<RatingsWithUserCount> GetRatings(string userId, RatingType ratingType)
        {
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return new RatingsWithUserCount();
            }

            return await _ratingRepository.GetRatings(ratingType, user);
        }


        /// <summary>
        /// Gets the user ranks.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public async Task<Tuple<long, long, long>> GetUserRanks(User user)
        {
            return await _ratingRepository.GetUserRanks(user);
        }


        /// <summary>
        /// Updates the user rating.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="oldPoints">The old points.</param>
        /// <param name="newPoints">The new points.</param>
        /// <returns>
        /// Task.
        /// </returns>
        public async Task UpdateUserRating(User user, int? oldPoints, int newPoints)
        {
            try
            {
                await _ratingRepository.UpdateUserRating(user, oldPoints, newPoints);
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                   "Error UpdateUserRating. User: {0}, OldPoints: {1}, NewPoints: {2}, Exception: {3}",
                   user.NickName,
                   oldPoints,
                   newPoints,
                   ex.ToString());
                userRatingsInitialized = false;
                await CheckInitRatingsCache(_userRepository);
            }

        }

        private async Task CheckInitRatingsCache(IUserRepository userRepository)
        {
            //handle concurrent ratings init
            await WaitBeforeCacheInitialized();

            if (userRatingsInitialized)
            {
                return;
            }

            userRatingsInitializing = true;
            Expression<Func<User, object>> select =
                u => new { u.AvatarUrl, u.NickName, u.Points, u.Id, u.LastRatingPlace, u.CityShortName, u.CountryShortName };
            var allUsers =
                await
                userRepository.GetUsers(
                    new QueryOptions<User> { Select = select, Filter = u => u.Points < 0 || u.Points >= 0 });
            await _ratingRepository.BuildRatings(allUsers);
            userRatingsInitializing = false;
            userRatingsInitialized = true;
        }
    }
}