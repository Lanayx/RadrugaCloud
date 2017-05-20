using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Memory
{
    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;
    using Core.Tools.CopyHelper;

    public class RatingRepository: IRatingRepository
    {
        private readonly SortedList<int, Dictionary<string, RatingInfo>> _userRatings =
            new SortedList<int, Dictionary<string, RatingInfo>>();

        public Task<RatingsWithUserCount> GetRatings(RatingType ratingType, User user)
        {
            var newList = new List<RatingInfo>();

            var userInLeaders = FillLeaders(user, newList);
            if (!userInLeaders)
            {
                FillNeighbours(user, newList);
            }

            return Task.Factory.StartNew(() => new RatingsWithUserCount
                                                   {
                                                       Ratings = newList
                                                   });
        }

        public async Task UpdateUserRating(User user, int? oldPoints, int newPoints)
        {
            var ratingInfo = new RatingInfo
            {
                LastPlace = user.LastRatingPlace,
                AvatarUrl = user.AvatarUrl,
                NickName = user.NickName
            };
            if (oldPoints.HasValue)
            {
                _userRatings[oldPoints.Value].Remove(user.Id);
                if (_userRatings[oldPoints.Value].Values.Count == 0)
                {
                    _userRatings.Remove(oldPoints.Value);
                }
            }

            Dictionary<string, RatingInfo> dict;
            if (!_userRatings.TryGetValue(newPoints, out dict))
            {
                _userRatings.Add(newPoints, new Dictionary<string, RatingInfo> { { user.Id, ratingInfo } });
            }
            else
            {
                dict.Add(user.Id, ratingInfo);
            }
            await Task.Yield();
        }

        public async Task UpdateAvatar(User user)
        {
            if (_userRatings.Count == 0)
            {
                await Task.Yield();
                return;
            }

            var pointsRecord = _userRatings.FirstOrDefault(ur => ur.Value.ContainsKey(user.Id));
            if (!pointsRecord.Equals(default(KeyValuePair<int, Dictionary<string, RatingInfo>>)))
            {
                pointsRecord.Value[user.Id].AvatarUrl = user.AvatarUrl;
            }
        }

        public async Task UpdateNickname(User user)
        {
            if (_userRatings.Count == 0)
            {
                await Task.Yield();
                return;
            }

            var pointsRecord = _userRatings.FirstOrDefault(ur => ur.Value.ContainsKey(user.Id));
            if (!pointsRecord.Equals(default(KeyValuePair<int, Dictionary<string, RatingInfo>>)))
            {
                pointsRecord.Value[user.Id].NickName = user.NickName;
            }
        }

        public async Task BuildRatings(IEnumerable<User> users)
        {
            _userRatings.Clear();
            foreach (var user in users)
            {
                PushRatingToCache(user);
            }
            await Task.Yield();
        }

        public Task<Tuple<long, long, long>> GetUserRanks(User user)
        {
            throw new NotImplementedException();
        }

        private void PushRatingToCache(User user)
        {
            // TODO add thread-safe version
            if (!user.Points.HasValue)
            {
                return;
            }

            var ratingInfo = new RatingInfo
            {
                AvatarUrl = user.AvatarUrl,
                NickName = user.NickName,
                LastPlace = user.LastRatingPlace
            };
            Dictionary<string, RatingInfo> dict;
            if (!_userRatings.TryGetValue(user.Points.Value, out dict)) //user points can't be null
            {
                _userRatings.Add(user.Points.Value, new Dictionary<string, RatingInfo> { { user.Id, ratingInfo } });
            }
            else
            {
                dict.Add(user.Id, ratingInfo);
            }
        }

        private bool FillLeaders(User user, List<RatingInfo> emptyList)
        {
            var userInLeaders = false;
            var place = 1;
            for (var i = _userRatings.Count - 1; i >= 0; --i)
            {
                var sortedUsers = _userRatings.Values[i].OrderBy(u => u.Value.LastPlace);
                foreach (var userRating in sortedUsers)
                {
                    if (userRating.Key == user.Id)
                    {
                        userInLeaders = true;
                    }
                    emptyList.Add(
                        new RatingInfo
                        {
                            Points = _userRatings.Keys[i],
                            UserId = userRating.Key,
                            AvatarUrl = userRating.Value.AvatarUrl,
                            NickName = userRating.Value.NickName,
                            Place = place
                        });
                    place++;
                    if (emptyList.Count == GameConstants.Rating.LeadersCount)
                    {
                        break;
                    }
                }
                if (emptyList.Count == GameConstants.Rating.LeadersCount)
                {
                    break;
                }
            }
            return userInLeaders;
        }

        private void FillNeighbours(User user, List<RatingInfo> emptyList)
        {
            if (!user.Points.HasValue)
            {
                emptyList.Add(
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

            var userPoints = user.Points.Value;

            var currentUserRatingGroupIndex = _userRatings.IndexOfKey(userPoints);
            var currentUserRatingGroup = GenerateFilledRatings(userPoints, _userRatings[userPoints]);

            var currentUserRating = currentUserRatingGroup.First(ur => ur.Key == user.Id);
            var currentUserRatingIndex = currentUserRatingGroup.IndexOf(currentUserRating);
            var allUsersWithBetterPoints = _userRatings.Where(ur => ur.Key > userPoints).Sum(ur => ur.Value.Count);
            var currentUserPlace = allUsersWithBetterPoints + currentUserRatingIndex + 1;
            if (currentUserPlace > GameConstants.Rating.LeadersCount + 1)
            {
                if (currentUserRatingIndex > 0)
                {
                    AddRatingInfoToResultList(
                        emptyList,
                        currentUserRatingGroup,
                        currentUserRatingIndex - 1,
                        currentUserPlace - 1);
                }
                else if (currentUserRatingGroupIndex < _userRatings.Count - 1)
                {
                    var nextGroupValues = _userRatings.Values[currentUserRatingGroupIndex + 1];
                    var nextGroupPoints = _userRatings.Keys[currentUserRatingGroupIndex + 1];
                    var nextRatingGroup = GenerateFilledRatings(nextGroupPoints, nextGroupValues);
                    AddRatingInfoToResultList(
                        emptyList,
                        nextRatingGroup,
                        nextRatingGroup.Count - 1,
                        currentUserPlace - 1);
                }
            }

            AddRatingInfoToResultList(emptyList, currentUserRatingGroup, currentUserRatingIndex, currentUserPlace);

            if (currentUserRatingIndex < currentUserRatingGroup.Count - 1)
            {
                AddRatingInfoToResultList(
                    emptyList,
                    currentUserRatingGroup,
                    currentUserRatingIndex + 1,
                    currentUserPlace + 1);
            }
            else if (currentUserRatingGroupIndex > 0)
            {
                var previousGroupValues = _userRatings.Values[currentUserRatingGroupIndex - 1];
                var previousGroupPoints = _userRatings.Keys[currentUserRatingGroupIndex - 1];
                var previousRatingGroup = GenerateFilledRatings(previousGroupPoints, previousGroupValues);
                AddRatingInfoToResultList(emptyList, previousRatingGroup, 0, currentUserPlace + 1);
            }
        }

        private void AddRatingInfoToResultList(
          List<RatingInfo> emptyList,
          List<KeyValuePair<string, RatingInfo>> sortedRatings,
          int currentUserRatingIndex,
          int place)
        {
            emptyList.Add(
                new RatingInfo
                {
                    Points = sortedRatings[currentUserRatingIndex].Value.Points,
                    UserId = sortedRatings[currentUserRatingIndex].Value.UserId,
                    AvatarUrl = sortedRatings[currentUserRatingIndex].Value.AvatarUrl,
                    NickName = sortedRatings[currentUserRatingIndex].Value.NickName,
                    Place = place
                });
        }

        private static List<KeyValuePair<string, RatingInfo>> GenerateFilledRatings(
            int points,
            Dictionary<string, RatingInfo> ratingGroup)
        {
            return ratingGroup.OrderBy(dic => dic.Value.LastPlace).Select(
                dic =>
                    {
                        var info = new RatingInfo();
                        dic.Value.CopyTo(info);
                        info.Points = points;
                        info.UserId = dic.Key;
                        return new KeyValuePair<string, RatingInfo>(dic.Key, info);
                    }).ToList();
        }

        public void Dispose()
        {
            // nothing to dispose
        }
    }
}
