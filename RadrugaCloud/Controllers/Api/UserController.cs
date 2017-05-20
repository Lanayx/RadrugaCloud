namespace RadrugaCloud.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.NonDomainModels;
    using Core.Tools.CopyHelper;

    using RadrugaCloud.Helpers;
    using RadrugaCloud.Models.Api;
    using RadrugaCloud.Models.Attributes;

    using Services.AuthorizationServices;
    using Services.BL;
    using Services.DomainServices;

    /// <summary>
    ///     Class UserController
    /// </summary>
    [Authorize]
    public class UserController : ApiController
    {
        private readonly ImageService _imageService;

        private readonly RatingService _ratingService;

        private readonly UserIdentityService _userIdentityService;

        private readonly UserService _userService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserController" /> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="imageService">The image service.</param>
        /// <param name="ratingService">The rating service.</param>
        /// <param name="userIdentityService">The user identity service.</param>
        public UserController(
            UserService userService,
            ImageService imageService,
            RatingService ratingService,
            UserIdentityService userIdentityService)
        {
            _userService = userService;
            _imageService = imageService;
            _ratingService = ratingService;
            _userIdentityService = userIdentityService;
        }

        /// <summary>
        ///     Changes the nickname.
        /// </summary>
        /// <param name="nickName">Nickname.</param>
        /// <returns>Task{OperationResult}.</returns>
        [HttpPost]
        public async Task<OperationResult> ChangeNickName([FromBody] string nickName)
        {
            var id = this.GetCurrentUserId();
            if (nickName.Length
                > ((MaxLengthAttribute)
                   typeof(User).GetProperty("NickName").GetCustomAttributes(typeof(MaxLengthAttribute), false).First())
                      .Length)
            {
                return new OperationResult(OperationResultStatus.Error, "Nickname is too long");
            }

            return await _userService.ChangeNickName(id, nickName);
        }

        /// <summary>
        ///     Changes notifications settings.
        /// </summary>
        /// <param name="notificationsEnabled">if set to <c>true</c> [notifications enabled].</param>
        /// <returns>
        ///     Task{OperationResult}.
        /// </returns>
        [HttpPost]
        public async Task<OperationResult> ChangeNotificationsSettings([FromBody] bool notificationsEnabled)
        {
            var id = this.GetCurrentUserId();

            return await _userService.ChangeNotificationsSettings(id, notificationsEnabled);
        }

        /// <summary>
        ///     Gets this instance.
        /// </summary>
        /// <returns>Task{User}.</returns>
        public async Task<UserInfo> Get()
        {
            var userInfo = new UserInfo();
            var user = await _userService.GetUser(this.GetCurrentUserId());
            if (user == null)
            {
                return null;
            }
            user.CopyTo(userInfo);
            await FillIdentityFields(userInfo);
            return userInfo;
        }


        /// <summary>
        /// Gets user the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<UserViewInfo> GetById(string id)
        {
            var userViewInfo = new UserViewInfo();
            var user = await _userService.GetUser(id);
            if (user == null)
            {
                return null;
            }
            var userRanks = await _ratingService.GetUserRanks(user);
            user.CopyTo(userViewInfo);
            FillViewFields(user, userViewInfo, userRanks);
            return userViewInfo;
        }


        /// <summary>
        ///     Gets the achievements.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<List<Achievement>> GetAchievements()
        {
            var userId = this.GetCurrentUserId();
            return _userService.GetAchievements(userId);
        }

        /// <summary>
        ///     Gets the ratings (top 10 + 2 neighbours).
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/User/GetRatings/{type}")]
        public async Task<RatingsWithUserCount> GetRatings(RatingType type)
        {
            var userId = this.GetCurrentUserId();
            var ratings = await _ratingService.GetRatings(userId, type);
            return ratings;
        }

        /// <summary>
        ///     Posts the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{AddResult}.</returns>
        public Task<IdResult> Post([FromBody] User user)
        {
            user.Id = this.GetCurrentUserId();
            return _userService.AddNewUser(user);
        }

        /// <summary>
        ///     Uploads the avatar.
        /// </summary>
        /// <returns>Task{OperationResult}.</returns>
        [HttpPost]
        [ValidateMimeMultipartContentFilter]
        public async Task<UrlResult> UploadAvatar()
        {
            var oneFileData = await FileUploadHelper.GetOneFileContent(Request);
            if (oneFileData.ErrorType == MultipartDataError.FileDataEmpty)
            {
                return new UrlResult(OperationResultStatus.Error, "Avatar is empty");
            }

            var avatarPath = await _imageService.SaveAvatarImage(this.GetCurrentUserId(), oneFileData.FileData);
            var result = await _userService.ChangeUserAvatar(this.GetCurrentUserId(), avatarPath);
            if (result.Status != OperationResultStatus.Success)
            {
                return new UrlResult(result.Status, result.Description);
            }
            return new UrlResult(avatarPath);
        }

        /// <summary>
        ///     Vks the repost done.
        /// </summary>
        /// <returns>Task{OperationResult}.</returns>
        [HttpPost]
        [Authorize]
        public Task<OperationResult> VkRepostDone()
        {
            var userId = this.GetCurrentUserId();
            return _userService.IncreaseVkRepostCount(userId);
        }

        private async Task FillIdentityFields(UserInfo userInfo)
        {
            var userIdentity = await _userIdentityService.GetUserIdentity(this.GetCurrentUserId());
            userInfo.HasEmail = !string.IsNullOrEmpty(userIdentity.LoginEmail);
            userInfo.HasVk = userIdentity.VkIdentity != null && userIdentity.VkIdentity.Id > 0;
        }


        private void FillViewFields(User user, UserViewInfo userInfo, Tuple<long,long,long>  userRanks)
        {
            userInfo.CompletedMissionIdsCount = user.CompletedMissionIds?.Count ?? 0;
            userInfo.FailedMissionIdsCount = user.FailedMissionIds?.Count ?? 0;
            userInfo.FiveRepostsBadge = user.VkRepostCount >= 5;
            userInfo.FiveSetsBadge = user.CompletedMissionSetIds?.Count >= 5;
            userInfo.KindScaleBadge = user.KindScaleHighMaxDays >= 5;
            userInfo.RatingGrowthBadge = user.UpInRatingMaxDays >= 5;
            userInfo.ThreeFiveStarsBadge = user.ThreeStarsMissionSpreeMaxCount >= 5;
            userInfo.GlobalRank = userRanks.Item1;
            userInfo.CountryRank = userRanks.Item2;
            userInfo.CityRank = userRanks.Item3;
        }
    }
}