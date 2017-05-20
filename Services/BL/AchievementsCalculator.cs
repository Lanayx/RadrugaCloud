using System.Collections.Generic;
using System.Linq;

namespace Services.BL
{
    using Core.Constants;
    using Core.DomainModels;
    using Core.NonDomainModels;

    public static class AchievementsCalculator
    {
        public static List<Achievement> GetAchievements(this User user)
        {
            return new List<Achievement>
                           {
                               GetFiveThreeStarsAchievement(user),
                               GetFiveSetsAchievement(user),
                               GetFiveDaysKindScaleAchievement(user),
                               GetFiveRepostsAchievement(user),
                               GetFiveDaysUpInRating(user)
                           };
        }

        private static Achievement GetFiveDaysUpInRating(User user)
        {
            var percentage = ((user.UpInRatingMaxDays ?? 0) / 5.0) * 100;
            return new Achievement
                       {
                           Description = "Пятидневный рост рейтинга",
                           Percentage = percentage > 100 ? (byte)100 : (byte)percentage
                       };
        }

        private static Achievement GetFiveRepostsAchievement(User user)
        {
            var percentage = ((user.VkRepostCount ?? 0) / 5.0) * 100;
            return new Achievement
                       {
                           Description = "Пять репостов пройденных миссий",
                           Percentage = percentage > 100 ? (byte)100 : (byte)percentage
                       };
        }

        private static Achievement GetFiveDaysKindScaleAchievement(User user)
        {
            var percentage = ((user.KindScaleHighMaxDays ?? 0) / 5.0) * 100;
            return new Achievement
                       {
                           Description = "Шкала добра не менее " + GameConstants.KindScale.AchievementLimit +  "% в течение пяти дней",
                           Percentage = percentage > 100 ? (byte)100 : (byte)percentage
                       };
        }

        private static Achievement GetFiveSetsAchievement(User user)
        {
            var percentage = ((user.CompletedMissionSetIds?.Count ?? 0) / 5.0) * 100;
            return new Achievement
                       {
                           Description = "Завершить пять сетов миссий",
                           Percentage = percentage > 100 ? (byte)100 : (byte)percentage
                       };
        }

        private static Achievement GetFiveThreeStarsAchievement(User user)
        {
            var percentage = ((user.ThreeStarsMissionSpreeMaxCount ?? 0) / 5.0) * 100;
            return new Achievement
                       {
                           Description = "Пять миссий подряд на три звезды",
                           Percentage = percentage > 100 ? (byte)100 : (byte)percentage
                       };
        }
    }
}
