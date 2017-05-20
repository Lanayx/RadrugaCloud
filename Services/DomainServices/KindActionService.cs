namespace Services.DomainServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.Tools;

    using Services.BL;

    /// <summary>
    ///     The KindAction service.
    /// </summary>
    public sealed class KindActionService
    {
        private readonly IKindActionRepository _kindActionRepository;

        private readonly RatingService _ratingService;

        private readonly IUserRepository _userRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="KindActionService" /> class.
        /// </summary>
        /// <param name="kindActionRepository">The KindAction repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="ratingService">The rating service.</param>
        public KindActionService(
            IKindActionRepository kindActionRepository,
            IUserRepository userRepository,
            RatingService ratingService)
        {
            _kindActionRepository = kindActionRepository;
            _userRepository = userRepository;
            _ratingService = ratingService;
        }

        /// <summary>
        ///     Adds new KindAction.
        /// </summary>
        /// <param name="kindAction">The KindAction.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<KindActionResult> AddNewKindAction(KindAction kindAction)
        {
            kindAction.DateAdded = DateTime.UtcNow;
            var kindActionResult = await _kindActionRepository.AddKindAction(kindAction);
            if (kindActionResult.Status == OperationResultStatus.Success)
            {
                var user = await _userRepository.GetUser(kindAction.UserId);
                var oldUserPoints = user.Points;
                RewardsCalculator.UpdateUserAfterKindAction(user, kindAction);
                var userResult = await _userRepository.UpdateUser(user);
                if (userResult.Status != OperationResultStatus.Success)
                    return new KindActionResult(userResult.Status, userResult.Description);

                // new points should always have value
                // ReSharper disable once PossibleInvalidOperationException
                await _ratingService.UpdateUserRating(user, oldUserPoints, user.Points.Value);
                return new KindActionResult(user.Points.Value, user.CoinsCount ?? 0, user.KindScale.Value);
            }
            return new KindActionResult(kindActionResult.Status, kindActionResult.Description);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            _kindActionRepository.Dispose();
        }

        /// <summary>
        /// Gets the KindActions.
        /// </summary>
        /// <param name="currentUserId">The current user identifier.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// Task{IEnumerable{KindAction}}.
        /// </returns>
        public Task<List<KindAction>> GetKindActions(string currentUserId, QueryOptions<KindAction> options = null)
        {
            if (options == null)
            {
                Expression<Func<KindAction, DateTime>> z = x => x.DateAdded;
                options = new QueryOptions<KindAction>
                {
                    Filter = option => option.UserId == currentUserId,
                    Top = GameConstants.KindActions.DisplayPerPage,
                    OrderBy = new List<SortDescription> { new SortDescription(z, SortDirection.Descending) }
                };
            }
            options.Top = (!options.Top.HasValue || options.Top > 50) ? 50 : options.Top;
            return _kindActionRepository.GetKindActions(options);
        }

        /// <summary>
        /// Judges the kind action.
        /// </summary>
        /// <param name="currentUserId">The current user identifier.</param>
        /// <param name="kindActionId">The kind action identifier.</param>
        /// <param name="isLike">if set to <c>true</c> [is like].</param>
        /// <returns></returns>
        public async Task<OperationResult> JudgeKindAction(string currentUserId, string targetUserId, string kindActionId, bool isLike)
        {
            var kindAction = await _kindActionRepository.GetKindAction(kindActionId, targetUserId);
            if (kindAction == null)
                return OperationResult.NotFound;
            if (kindAction.UserId == currentUserId)
                return new OperationResult(OperationResultStatus.Error, "Can't like your own kind actions");
            if((kindAction.Likes != null && kindAction.Likes.Contains(currentUserId)) ||
                (kindAction.Dislikes != null && kindAction.Dislikes.Contains(currentUserId)))
                return new OperationResult(OperationResultStatus.Error, "Only one like per kind action alowed");
            var user = await _userRepository.GetUser(targetUserId);
            if (user == null)
                return OperationResult.NotFound;
            var oldUserPoints = user.Points;
            if (isLike)
            {
                kindAction.Likes = kindAction.Likes ?? new List<string>();
                kindAction.Likes.Add(currentUserId);
                user.Points++;//Points can't be null here
            }
            else
            {
                kindAction.Dislikes = kindAction.Dislikes ?? new List<string>();
                kindAction.Dislikes.Add(currentUserId);
                user.Points--;//Points can't be null here
            }
            // ReSharper disable once PossibleInvalidOperationException
            await _ratingService.UpdateUserRating(user, oldUserPoints, user.Points.Value);


            var kindActionUpdateResult = await _kindActionRepository.UpdateLikes(kindAction);
            if (kindActionUpdateResult.Status == OperationResultStatus.Success)
            {
                user.KindActionMarksCount = (user.KindActionMarksCount ?? 0) + 1;
                return await _userRepository.UpdateUser(user);
            }
            return kindActionUpdateResult;
        }
    }
}