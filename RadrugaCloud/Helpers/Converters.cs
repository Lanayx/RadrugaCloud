namespace RadrugaCloud.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    using Core.DomainModels;
    using Core.NonDomainModels;

    using RadrugaCloud.Areas.Admin.Models;
    using RadrugaCloud.Models;
    using Core.Tools.CopyHelper;
    /// <summary>
    ///     Class Converters
    /// </summary>
    public static class Converters
    {
        #region Methods

        internal static MissionDraftUi ConvertToUi(this MissionDraft missionDraftDomain)
        {
            if (missionDraftDomain == null)
            {
                return null;
            }

            return new MissionDraftUi
                       {
                           AddDate = missionDraftDomain.AddDate,
                           AgeFrom = missionDraftDomain.AgeFrom,
                           AgeTo = missionDraftDomain.AgeTo,
                           Author = missionDraftDomain.Author,
                           CorrectAnswers = missionDraftDomain.CorrectAnswers,
                           Description = missionDraftDomain.Description,
                           Difficulty = missionDraftDomain.Difficulty,
                           Id = missionDraftDomain.Id,
                           Name = missionDraftDomain.Name,
                           PhotoUrl = missionDraftDomain.PhotoUrl,
                           PersonQualitiesWithScores =
                               missionDraftDomain.PersonQualities?.Select(
                                   p =>
                                   new PersonQualityIdWithScore
                                       {
                                           PersonQualityId = p.PersonQualityId,
                                           Score = p.Score
                                       }).ToList(),
                       };
        }

        internal static MissionUi ConvertToUi(this Mission missionDomain)
        {
            if (missionDomain == null)
            {
                return null;
            }

            return new MissionUi
                       {
                           // Common
                           Id = missionDomain.Id,
                           Name = missionDomain.Name,
                           PhotoUrl = missionDomain.PhotoUrl,
                           Description = missionDomain.Description,
                           Difficulty = missionDomain.Difficulty,
                           AgeFrom = missionDomain.AgeFrom,
                           AgeTo = missionDomain.AgeTo,
                           ExecutionType = missionDomain.ExecutionType,
                           DependsOn = missionDomain.DependsOn,
                           PersonQualitiesWithScores =
                               missionDomain.PersonQualities?.Select(
                                   p =>
                                   new PersonQualityIdWithScore
                                       {
                                           PersonQualityId = p.PersonQualityId,
                                           Score = p.Score
                                       }).ToList(),
                           Hints = missionDomain.Hints,
                           IsFinal = missionDomain.IsFinal,
                           MessageAfterCompletion = missionDomain.MessageAfterCompletion,
                           MissionSetId = missionDomain.MissionSetId,

                           // Right answer type
                           AnswersCount = missionDomain.AnswersCount,
                           ExactAnswer = missionDomain.ExactAnswer,
                           CorrectAnswers = missionDomain.CorrectAnswers,
                           TriesFor1Star = missionDomain.TriesFor1Star,
                           TriesFor2Stars = missionDomain.TriesFor2Stars,
                           TriesFor3Stars = missionDomain.TriesFor3Stars,

                           // Text Creation
                           MinCharsCount = missionDomain.MinCharsCount,

                           // Photo Creation
                           NumberOfPhotos = missionDomain.NumberOfPhotos,

                           // GeoCoordinates + GeoPath
                           AccuracyRadius = missionDomain.AccuracyRadius,
                           UserCoordinatesCalculationFunction =
                               missionDomain.UserCoordinatesCalculationFunction,
                           CalculationFunctionParameters = missionDomain.CalculationFunctionParameters,
                           SecondsFor1Star = missionDomain.SecondsFor1Star,
                           SecondsFor2Stars = missionDomain.SecondsFor2Stars,
                           SecondsFor3Stars = missionDomain.SecondsFor3Stars,
                           CommonPlaceAlias = missionDomain.CommonPlaceAlias,
                           InitialCommonPlaceAlias = missionDomain.CommonPlaceAlias,
                       };
        }

        internal static MissionSetUI ConvertToUi(this MissionSet missionSetDomain)
        {
            if (missionSetDomain == null)
            {
                return null;
            }

            return new MissionSetUI
                       {
                           Id = missionSetDomain.Id,
                           Name = missionSetDomain.Name,
                           Missions =
                               missionSetDomain.Missions?.OrderBy(m => m.Order)
                               .Select(p => p.Mission.Id).ToList(),
                       };
        }

        internal static QuestionUi ConvertToUi(this Question questionDomain)
        {
            if (questionDomain == null)
            {
                return null;
            }

            var questionOptions = questionDomain.Options.GetQuestionOptions();

            return new QuestionUi
                       {
                           Id = questionDomain.Id, 
                           Name = questionDomain.Name, 
                           Text = questionDomain.Text, 
                           QuestionOptions = questionOptions
                       };
        }

        private static List<QuestionOptionUI> GetQuestionOptions(this List<QuestionOption> options)
        {
            if (options == null)
            {
                return null;
            }

            var result = options.Select(
                qo =>
                    {
                        var quiestionOptionsUI = new QuestionOptionUI
                                                     {
                                                         Number = qo.Number, 
                                                         Text = qo.Text, 
                                                         NextQuestionId = qo.NextQuestionId, 
                                                     };

                        if (qo.PersonQualitiesWithScores != null)
                        {
                            quiestionOptionsUI.PersonQualitiesWithScores =
                                qo.PersonQualitiesWithScores.Select(
                                    ptws =>
                                    new PersonQualityIdWithScore
                                        {
                                            Score = ptws.Score, 
                                            PersonQualityId = ptws.PersonQualityId
                                        }).ToList();
                        }
                        return quiestionOptionsUI;
                    }).ToList();

            return result;
        }


        public static Models.Api.KindAction ConvertToApi(this KindAction kindAction, string currentUserId) {
           
            var kindActionApi = new Models.Api.KindAction();
            kindAction.CopyTo(kindActionApi);
            kindActionApi.LikesCount = kindAction.Likes?.Count ?? 0;
            kindActionApi.DislikesCount = kindAction.Dislikes?.Count ?? 0;
            kindActionApi.AlreadyLiked = (kindAction.Likes?.Contains(currentUserId) ?? false)
                                         || (kindAction.Dislikes?.Contains(currentUserId) ?? false);
            return kindActionApi;
        }

        #endregion
    }
}