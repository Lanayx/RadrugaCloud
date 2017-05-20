namespace Core.Interfaces.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Core.DomainModels;
    using Core.Enums;
    using Core.NonDomainModels;

    public interface IRatingRepository: IDisposable
    {
        Task<RatingsWithUserCount> GetRatings(RatingType ratingType, User user);

        Task UpdateUserRating(User user, int? oldPoints, int newPoints);

        Task UpdateAvatar(User user);

        Task UpdateNickname(User user);

        Task BuildRatings(IEnumerable<User> users);

        Task<Tuple<long, long, long>> GetUserRanks(User user);
    }
}
